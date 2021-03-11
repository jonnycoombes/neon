#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using JCS.Neon.Glow.Exceptions;
using JCS.Neon.Glow.Logging;
using JCS.Neon.Glow.Types;
using JCS.Neon.Glow.Types.Extensions;
using Serilog;

#endregion

namespace JCS.Neon.Glow.Cryptography
{
    /// <summary>
    ///     Static class containing a bunch of methods for dealing with AES-based symmetric encryption
    /// </summary>
    public static class SymmetricEncryptionHelper
    {
        /// <summary>
        ///     Enumeration of input encoding types TODO - Refactor out common symmetric options to single class
        /// </summary>
        public enum InputEncodingOption
        {
            None,
            Base64,
            Base64Url
        }

        /// <summary>
        ///     Options used for unwrapping during certain AES-based encryption methods
        /// </summary>
        public enum KeyUnwrappingOption
        {
            /// <summary>
            ///     Unwrap using an asymmetric public key
            /// </summary>
            UnwrapWithPublicKey,

            /// <summary>
            ///     Unwrap using an asymmetric private key
            /// </summary>
            UnwrapWithPrivateKey
        }

        /// <summary>
        ///     Options enum used within certain AES-based encryption methods
        /// </summary>
        public enum KeyWrappingOption
        {
            /// <summary>
            ///     Wrap keys using an asymmetric public key
            /// </summary>
            WrapWithPublicKey,

            /// <summary>
            ///     Wrap keys using an asymmetric private key
            /// </summary>
            WrapWithPrivateKey
        }

        /// <summary>
        ///     Enumeration of output encoding types
        /// </summary>
        public enum OutputEncodingOption
        {
            None,
            Base64,
            Base64Url
        }

        /// <summary>
        ///     Enumeration of the symmetric algorithms available for use
        /// </summary>
        public enum SymmetricAlgorithmOption
        {
            Aes,
            Rc2,
            TripleDes
        }

        /// <summary>
        ///     Specifies which direction an AES transformation goes in
        /// </summary>
        public enum TransformDirectionOption
        {
            /// <summary>
            ///     Encrypt transform
            /// </summary>
            Encrypt,

            /// <summary>
            ///     Decrypt transform
            /// </summary>
            Decrypt
        }

        /// <summary>
        ///     Static logger for this class
        /// </summary>
        private static readonly ILogger _log = Log.ForContext(typeof(SymmetricEncryptionHelper));

        /// <summary>
        ///     Validates that a given key size is valid
        /// </summary>
        /// <param name="algorithm">An instance of <see cref="SymmetricAlgorithm" /></param>
        /// <param name="size">The required key size</param>
        /// <returns></returns>
        private static bool ValidateKeySize(SymmetricAlgorithm algorithm, int size)
        {
            LogHelper.MethodCall(_log);
            return algorithm.LegalKeySizes.Any(_ => size.Equals(size));
        }

        /// <summary>
        ///     Creates and initialises a new instance of  a given <see cref="SymmetricAlgorithm" />
        /// </summary>
        /// <param name="options">A set of options for the cipher instance</param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        /// <exception cref="SymmetricEncryptionHelperException">If the specified options are invalid</exception>
        private static SymmetricAlgorithm InitialiseCipher(SymmetricEncryptionOptions options, byte[]? key = null, byte[]? iv = null)
        {
            LogHelper.MethodCall(_log);
            LogHelper.Verbose(_log,
                $"Initialising {options.SymmetricAlgorithmOption.ToString()} with a suggested config of ({options.KeySize}, {options.Mode})");
            var algorithm = SymmetricAlgorithm.Create(options.SymmetricAlgorithmOption.ToString());
            if (algorithm == null)
            {
                throw ExceptionHelper.LoggedException<SymmetricEncryptionHelperException>(_log,
                    $"An invalid or unavailable symmetric encryption algorithm has been selected - \"{options.SymmetricAlgorithmOption}\"");
            }

            if (!ValidateKeySize(algorithm, options.KeySize))
            {
                throw ExceptionHelper.LoggedException<SymmetricEncryptionHelperException>(_log,
                    $"An invalid symmetric key size was specified - {options.KeySize} bits");
            }

            algorithm.KeySize = options.KeySize;
            algorithm.Mode = options.Mode;
            if (key != null)
            {
                algorithm.Key = key;
            }
            else
            {
                algorithm.GenerateKey();
            }

            if (iv != null)
            {
                algorithm.IV = iv;
            }
            else
            {
                algorithm.GenerateIV();
            }

            return algorithm;
        }

        /// <summary>
        ///     Packs an AES key and the IV into a single array
        /// </summary>
        /// <param name="cipher">The current cipher instance</param>
        /// <returns>An array containing the key and then the IV in sequence</returns>
        private static byte[] PackKeyAndIv(SymmetricAlgorithm cipher)
        {
            LogHelper.MethodCall(_log);
            var packed = BitConverter.GetBytes(cipher.KeySize);
            return packed.Concatenate(cipher.Key).Concatenate(cipher.IV);
        }

        /// <summary>
        ///     Unpacks an AES key and IV from a single array
        /// </summary>
        /// <param name="packed"></param>
        /// <returns>A <see cref="Pair{S,T}" />, with the leftmost entry being the unpacked key, the rightmost the IV</returns>
        private static Pair<byte[], byte[]> UnpackKeyAndIv(IReadOnlyCollection<byte> packed)
        {
            LogHelper.MethodCall(_log);
            var keySize = BitConverter.ToInt32(packed.Take(4).ToArray());
            var keyLength = keySize / 8;
            var key = packed.Skip(4).Take(keyLength).ToArray();
            var iv = packed.Skip(4 + keyLength).Take(packed.Count - 4 - keyLength).ToArray();
            return new Pair<byte[], byte[]>(key, iv);
        }

        /// <summary>
        ///     Encrypts the source byte array using a new AES key, and then uses the supplied certificate key material to wrap the
        ///     key, IV and randomly generated salt value
        /// </summary>
        /// <param name="input">The source byte array to encrypt</param>
        /// <param name="certificate">A valid <see cref="X509Certificate2" /> which containing a public and private key pair</param>
        /// <param name="configureAction">Sets the encryption and wrapping options</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns>
        ///     A <see cref="Pair{S,T}" />, where the left value is a byte array containing a wrapped key, salt and IV, and the
        ///     right value contains the encrypted source
        /// </returns>
        /// <exception cref="SymmetricEncryptionHelperException"></exception>
        public static Pair<byte[], byte[]> EncryptAndWrap(byte[] input, X509Certificate2 certificate,
            Action<SymmetricEncryptionOptionsBuilder> configureAction)
        {
            LogHelper.MethodCall(_log);

            // perform option configuration
            var builder = new SymmetricEncryptionOptionsBuilder();
            configureAction(builder);
            var options = builder.Options;

            LogHelper.Verbose(_log, $"Attempting encryption and then wrapping key using \"{options.SymmetricKeyWrappingOption}\"");

            // check we have a private key if required
            if (options.SymmetricKeyWrappingOption == KeyWrappingOption.WrapWithPrivateKey && !certificate.HasPrivateKey)
            {
                throw ExceptionHelper.LoggedException<SymmetricEncryptionHelperException>(_log,
                    "Private key specified for key wrapping operations, but no private key present");
            }

            // check that the certificate has an RSA key type
            if (certificate.PublicKey.Key.SignatureAlgorithm != null
                && !certificate.PublicKey.Key.SignatureAlgorithm.Equals("RSA", StringComparison.InvariantCultureIgnoreCase))
            {
                throw ExceptionHelper.LoggedException<SymmetricEncryptionHelperException>(_log,
                    "Specified certificate doesn't have a supported key type");
            }

            try
            {
                // initialise the cipher, encrypt, pack the key and IV and perform any output encoding
                using var cipher = InitialiseCipher(options);
                var encrypted = Transform(input, cipher, TransformDirectionOption.Encrypt);
                var packed = PackKeyAndIv(cipher);
                switch (options.SymmetricKeyWrappingOption)
                {
                    case KeyWrappingOption.WrapWithPublicKey:
                        var rsaPublic = certificate.GetRSAPublicKey();
                        var wrappedPublic = rsaPublic!.Encrypt(packed, RSAEncryptionPadding.Pkcs1);
                        return new Pair<byte[], byte[]>(wrappedPublic, encrypted);
                    case KeyWrappingOption.WrapWithPrivateKey:
                        var rsaPrivate = certificate.GetRSAPrivateKey();
                        var wrappedPrivate = rsaPrivate!.Encrypt(packed, RSAEncryptionPadding.Pkcs1);
                        return new Pair<byte[], byte[]>(wrappedPrivate, encrypted);
                    default:
                        throw new ArgumentOutOfRangeException($"{nameof(options.SymmetricKeyWrappingOption)}");
                }
            }
            catch (Exception ex)
            {
                throw ExceptionHelper.LoggedException<SymmetricEncryptionHelperException>(_log,
                    "Exception during crypto operation - could lead to a security issue", ex);
            }
        }

        /// <summary>
        ///     Takes a pair consisting of a wrapped, packed key and IV along with an encrypted payload, and attempts to
        ///     unwrap the key material using the supplied certificate and then decrypt the payload
        /// </summary>
        /// <param name="input">A pair where the leftmost is a wrapped, packed key and IV.  The rightmost is the encrypted payload</param>
        /// <param name="certificate">The x509 certificate to use for obtaining key unwrapping material from</param>
        /// <param name="configureAction">The <see cref="SymmetricEncryptionOptions" /> to use</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public static byte[] UnwrapAndDecrypt(Pair<byte[], byte[]> input, X509Certificate2 certificate,
            Action<SymmetricEncryptionOptionsBuilder> configureAction)
        {
            LogHelper.MethodCall(_log);

            // perform option configuration
            var builder = new SymmetricEncryptionOptionsBuilder();
            configureAction(builder);
            var options = builder.Options;

            LogHelper.Verbose(_log, $"Attempting encryption and then wrapping key using \"{options.SymmetricKeyWrappingOption}\"");
            // check we have a private key if required
            if (options.KeyUnwrappingOption == KeyUnwrappingOption.UnwrapWithPrivateKey && !certificate.HasPrivateKey)
            {
                throw ExceptionHelper.LoggedException<SymmetricEncryptionHelperException>(_log,
                    "Private key specified for key unwrapping operations, but no private key present");
            }

            // check that the certificate has an RSA key type
            if (certificate.PublicKey.Key.SignatureAlgorithm != null
                && !certificate.PublicKey.Key.SignatureAlgorithm.Equals("RSA", StringComparison.InvariantCultureIgnoreCase))
            {
                throw ExceptionHelper.LoggedException<SymmetricEncryptionHelperException>(_log,
                    "Specified certificate doesn't have a supported key type");
            }

            try
            {
                // firstly, unwrap the key and IV
                byte[] unwrappedKeyParams;
                switch (options.KeyUnwrappingOption)
                {
                    case KeyUnwrappingOption.UnwrapWithPrivateKey:
                        var rsaPrivate = certificate.GetRSAPrivateKey();
                        unwrappedKeyParams = rsaPrivate!.Decrypt(input.Left, RSAEncryptionPadding.Pkcs1);
                        break;
                    case KeyUnwrappingOption.UnwrapWithPublicKey:
                        var rsaPublic = certificate.GetRSAPublicKey();
                        unwrappedKeyParams = rsaPublic!.Decrypt(input.Left, RSAEncryptionPadding.Pkcs1);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"{nameof(options.SymmetricKeyWrappingOption)}");
                }

                // unpack the key and IV
                var unpackedKeyParams = UnpackKeyAndIv(unwrappedKeyParams);
                using var aes = InitialiseCipher(options, unpackedKeyParams.Left, unpackedKeyParams.Right);
                return Transform(input.Right, aes, TransformDirectionOption.Decrypt);
            }
            catch (Exception ex)
            {
                throw ExceptionHelper.LoggedException<SymmetricEncryptionHelperException>(_log,
                    "Exception during crypto operation - could lead to a security issue", ex);
            }
        }

        /// <summary>
        ///     Encrypts a series of blocks using the supplied Aes cipher.  If the size of the
        /// </summary>
        /// <param name="source">The source string to encrypt</param>
        /// <param name="cipher">The (already initialised) cipher instance</param>
        /// <param name="directionOption">Specifies the direction of the transform</param>
        /// <returns></returns>
        private static byte[] Transform(byte[] source, SymmetricAlgorithm cipher, TransformDirectionOption directionOption)
        {
            LogHelper.MethodCall(_log);
            using var cryptor =
                directionOption == TransformDirectionOption.Encrypt ? cipher.CreateEncryptor() : cipher.CreateDecryptor();
            LogHelper.Verbose(_log, "Encrypting larger data, using stream transform");
            try
            {
                switch (directionOption)
                {
                    case TransformDirectionOption.Encrypt:
                    {
                        using var bufferStream = new MemoryStream();
                        using (var cryptoStream = new CryptoStream(bufferStream, cryptor, CryptoStreamMode.Write))
                        {
                            using (var writer = new BinaryWriter(cryptoStream))
                            {
                                writer.Write(source);
                            }
                        }

                        var encrypted = bufferStream.ToArray();
                        return encrypted;
                    }
                    case TransformDirectionOption.Decrypt:
                    {
                        using var bufferStream = new MemoryStream(source);
                        using var cryptoStream = new CryptoStream(bufferStream, cryptor, CryptoStreamMode.Read);
                        using var reader = new BinaryReader(cryptoStream);
                        var decrypted = reader.ReadBytes(source.Length);
                        return decrypted;
                    }
                    default:
                        throw ExceptionHelper.LoggedException<SymmetricEncryptionHelperException>(_log,
                            "Unreachable exception - here due to non-exhaustive pattern matching");
                }
            }
            catch (Exception ex)
            {
                throw ExceptionHelper.LoggedException<SymmetricEncryptionHelperException>(_log, "Stream-based encryption failed", ex);
            }
        }

        #region Exceptions

        public class SymmetricEncryptionHelperException : Exception
        {
            public SymmetricEncryptionHelperException(string? message) : base(message)
            {
            }

            public SymmetricEncryptionHelperException(string? message, Exception? innerException) : base(message, innerException)
            {
            }
        }

        #endregion

        /// <summary>
        ///     A class containing common options for AES encryption operations
        /// </summary>
        public class SymmetricEncryptionOptions
        {
            /// <summary>
            ///     The encryption algorithm to use
            /// </summary>
            public SymmetricAlgorithmOption SymmetricAlgorithmOption { get; set; } = SymmetricAlgorithmOption.Aes;

            /// <summary>
            ///     If the key is going to be wrapped using an asymmetric key, then what kind of key to use
            /// </summary>
            public KeyWrappingOption SymmetricKeyWrappingOption { get; set; } = KeyWrappingOption.WrapWithPublicKey;

            /// <summary>
            ///     If the key is going to be unwrapped using an asymmetric key, then what kind of key to use
            /// </summary>
            public KeyUnwrappingOption KeyUnwrappingOption { get; set; } =
                KeyUnwrappingOption.UnwrapWithPrivateKey;

            /// <summary>
            ///     The key size to use - default to 256
            /// </summary>
            public int KeySize { get; set; } = 256;

            /// <summary>
            ///     The encryption mode, defaults to cipher block chaining
            /// </summary>
            public CipherMode Mode { get; set; } = CipherMode.CBC;

            /// <summary>
            ///     The output encoding to use
            /// </summary>
            public OutputEncodingOption SymmetricOutputEncodingOption { get; set; } = OutputEncodingOption.None;

            /// <summary>
            ///     The input encoding to use
            /// </summary>
            public InputEncodingOption InputEncodingOption { get; set; } = InputEncodingOption.None;
        }

        /// <summary>
        ///     Builder for encryption options
        /// </summary>
        public class SymmetricEncryptionOptionsBuilder
        {
            /// <summary>
            ///     The actual options
            /// </summary>
            public SymmetricEncryptionOptions Options = new();

            /// <summary>
            ///     Sets the encryption algorithm to use
            /// </summary>
            /// <param name="option">A value from the <see cref="SymmetricAlgorithmOption" /> enumeration</param>
            /// <returns>The current builder instance</returns>
            public SymmetricEncryptionOptionsBuilder SetSymmetricAlgorithm(SymmetricAlgorithmOption option)
            {
                Options.SymmetricAlgorithmOption = option;
                return this;
            }

            /// <summary>
            ///     Set the <see cref="KeyWrappingOption" />
            /// </summary>
            /// <param name="option"></param>
            /// <returns></returns>
            public SymmetricEncryptionOptionsBuilder SetKeyWrappingOption(KeyWrappingOption option)
            {
                Options.SymmetricKeyWrappingOption = option;
                return this;
            }

            /// <summary>
            ///     Set the <see cref="KeyUnwrappingOption" />
            /// </summary>
            /// <param name="option"></param>
            /// <returns></returns>
            public SymmetricEncryptionOptionsBuilder SetKeyUnwrappingOption(KeyUnwrappingOption option)
            {
                Options.KeyUnwrappingOption = option;
                return this;
            }

            /// <summary>
            ///     Set the key size
            /// </summary>
            /// <param name="keySize"></param>
            /// <returns></returns>
            public SymmetricEncryptionOptionsBuilder SetKeySize(int keySize)
            {
                Options.KeySize = keySize;
                return this;
            }

            /// <summary>
            ///     Set the cipher mode
            /// </summary>
            /// <param name="mode"></param>
            /// <returns></returns>
            public SymmetricEncryptionOptionsBuilder SetCipherMode(CipherMode mode)
            {
                Options.Mode = mode;
                return this;
            }

            /// <summary>
            ///     Set the outbound encoding
            /// </summary>
            /// <param name="encodingOption"></param>
            /// <returns></returns>
            public SymmetricEncryptionOptionsBuilder SetOutputEncoding(OutputEncodingOption encodingOption)
            {
                Options.SymmetricOutputEncodingOption = encodingOption;
                return this;
            }

            /// <summary>
            ///     Sets in the inbound encoding
            /// </summary>
            /// <param name="encodingOption"></param>
            /// <returns></returns>
            public SymmetricEncryptionOptionsBuilder SetInputEncoding(InputEncodingOption encodingOption)
            {
                Options.InputEncodingOption = encodingOption;
                return this;
            }
        }
    }
}