/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using JCS.Neon.Glow.Types;
using JCS.Neon.Glow.Types.Extensions;
using Serilog;

#endregion

namespace JCS.Neon.Glow.Statics.Crypto
{
    /// <summary>
    ///     Static class containing a bunch of methods for dealing with AES-based symmetric encryption
    /// </summary>
    public static class SymmetricEncryption
    {
        /// <summary>
        ///     Static logger for this class
        /// </summary>
        private static readonly ILogger _log = Log.ForContext(typeof(SymmetricEncryption));

        /// <summary>
        ///     Validates that a given key size is valid
        /// </summary>
        /// <param name="algorithm">An instance of <see cref="SymmetricAlgorithm" /></param>
        /// <param name="size">The required key size</param>
        /// <returns></returns>
        private static bool ValidateKeySize(SymmetricAlgorithm algorithm, int size)
        {
            Logging.MethodCall(_log);
            return algorithm.LegalKeySizes.Any(_ => size.Equals(size));
        }

        /// <summary>
        ///     Creates and initialises a new instance of  a given <see cref="SymmetricAlgorithm" />
        /// </summary>
        /// <param name="options">A set of options for the cipher instance</param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        /// <exception cref="SymmetricEncryptionException">If the specified options are invalid</exception>
        private static SymmetricAlgorithm InitialiseCipher(SymmetricEncryptionOptions options, byte[]? key = null, byte[]? iv = null)
        {
            Logging.MethodCall(_log);
            Logging.Debug(_log,
                $"Initialising {options.SymmetricAlgorithmOption.ToString()} with a suggested config of ({options.KeySize}, {options.Mode})");
            var algorithm = SymmetricAlgorithm.Create(options.SymmetricAlgorithmOption.ToString());
            if (algorithm == null)
            {
                throw Exceptions.LoggedException<SymmetricEncryptionException>(_log,
                    $"An invalid or unavailable symmetric encryption algorithm has been selected - \"{options.SymmetricAlgorithmOption}\"");
            }

            if (!ValidateKeySize(algorithm, options.KeySize))
            {
                throw Exceptions.LoggedException<SymmetricEncryptionException>(_log,
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
            Logging.MethodCall(_log);
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
            Logging.MethodCall(_log);
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
        /// <exception cref="SymmetricEncryptionException"></exception>
        public static Pair<byte[], byte[]> EncryptAndWrap(byte[] input, X509Certificate2 certificate,
            Action<SymmetricEncryptionOptionsBuilder> configureAction)
        {
            Logging.MethodCall(_log);

            // perform option configuration
            var builder = new SymmetricEncryptionOptionsBuilder();
            configureAction(builder);
            var options = builder.Build();

            Logging.Debug(_log, $"Attempting encryption and then wrapping key using \"{options.SymmetricKeyWrappingOption}\"");
            Logging.Debug(_log, $"Certificate being used for wrap has thumbprint \"{certificate.Thumbprint}\"");
            
            // check we have a private key if required
            if (options.SymmetricKeyWrappingOption == KeyWrappingOption.WrapWithPrivateKey && !certificate.HasPrivateKey)
            {
                throw Exceptions.LoggedException<SymmetricEncryptionException>(_log,
                    "Private key specified for key wrapping operations, but no private key present");
            }

            // check that the certificate has an RSA key type
            if (certificate.PublicKey.Key.SignatureAlgorithm != null
                && !certificate.PublicKey.Key.SignatureAlgorithm.Equals("RSA", StringComparison.InvariantCultureIgnoreCase))
            {
                throw Exceptions.LoggedException<SymmetricEncryptionException>(_log,
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
                throw Exceptions.LoggedException<SymmetricEncryptionException>(_log,
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
            Logging.MethodCall(_log);

            // perform option configuration
            var builder = new SymmetricEncryptionOptionsBuilder();
            configureAction(builder);
            var options = builder.Build();

            Logging.Debug(_log, $"Attempting encryption and then wrapping key using \"{options.SymmetricKeyWrappingOption}\"");
            Logging.Debug(_log, $"Certificate being used for unwrap has thumbprint \"{certificate.Thumbprint}\"");
            // check we have a private key if required
            if (options.KeyUnwrappingOption == KeyUnwrappingOption.UnwrapWithPrivateKey && !certificate.HasPrivateKey)
            {
                throw Exceptions.LoggedException<SymmetricEncryptionException>(_log,
                    "Private key specified for key unwrapping operations, but no private key present");
            }

            // check that the certificate has an RSA key type
            if (certificate.PublicKey.Key.SignatureAlgorithm != null
                && !certificate.PublicKey.Key.SignatureAlgorithm.Equals("RSA", StringComparison.InvariantCultureIgnoreCase))
            {
                throw Exceptions.LoggedException<SymmetricEncryptionException>(_log,
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
                throw Exceptions.LoggedException<SymmetricEncryptionException>(_log,
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
            Logging.MethodCall(_log);
            using var cryptor =
                directionOption == TransformDirectionOption.Encrypt ? cipher.CreateEncryptor() : cipher.CreateDecryptor();
            Logging.Debug(_log, "Encrypting larger data, using stream transform");
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
                        throw Exceptions.LoggedException<SymmetricEncryptionException>(_log,
                            "Unreachable exception - here due to non-exhaustive pattern matching");
                }
            }
            catch (Exception ex)
            {
                throw Exceptions.LoggedException<SymmetricEncryptionException>(_log, "Stream-based encryption failed", ex);
            }
        }
    }
}