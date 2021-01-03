using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using JCS.Neon.Glow.Types;
using Serilog;
using static JCS.Neon.Glow.Helpers.General.LogHelpers;

namespace JCS.Neon.Glow.Helpers.Crypto
{
    /// <summary>
    /// Static class containing a bunch of methods for dealing with AES-based symmetric encryption
    /// </summary>
    public class AesHelpers
    {
        /// <summary>
        /// Static logger for this class
        /// </summary>
        private static ILogger _log = Log.ForContext(typeof(AesHelpers));

        #region Exceptions

        public class AesHelperException : Exception
        {
            public AesHelperException(string? message) : base(message)
            {
            }

            public AesHelperException(string? message, Exception? innerException) : base(message, innerException)
            {
            }
        }

        #endregion

        /// <summary>
        /// Enumeration of output encoding types
        /// </summary>
        public enum AesOutputEncoding
        {
            None,
            Base64,
            Base64Url
        }

        /// <summary>
        /// Enumeration of input encoding types
        /// </summary>
        public enum AesInputEncoding
        {
            None,
            Base64,
            Base64Url
        }

        /// <summary>
        /// Options enum used within certain AES-based encryption methods
        /// </summary>
        public enum AesKeyWrappingOptions
        {
            /// <summary>
            /// Wrap keys using an asymmetric public key
            /// </summary>
            WrapWithPublicKey,

            /// <summary>
            /// Unwrap using an asymmetric public key
            /// </summary>
            UnwrapWithPublicKey,

            /// <summary>
            /// Wrap keys using an asymmetric private key
            /// </summary>
            WrapWithPrivateKey,

            /// <summary>
            /// Unwrap using an asymmetric private key
            /// </summary>
            UnwrapWithPrivateKey
        }

        /// <summary>
        /// A class containing common options for AES encryption operations
        /// </summary>
        public class AesEncryptionOptions
        {
            /// <summary>
            /// If the key is going to be wrapped using an asymmetric key, then what kind of key to use
            /// </summary>
            public AesKeyWrappingOptions KeyWrappingOptions { get; set; } = AesKeyWrappingOptions.WrapWithPublicKey;

            /// <summary>
            /// The key size to use - default to 256
            /// </summary>
            public int KeySize { get; set; } = 256;

            /// <summary>
            /// The encryption mode, defaults to cipher block chaining
            /// </summary>
            public CipherMode Mode { get; set; } = CipherMode.CBC;

            /// <summary>
            /// The output encoding to use
            /// </summary>
            public AesOutputEncoding OutputEncoding { get; set; } = AesOutputEncoding.Base64Url;

            /// <summary>
            /// The input encoding to use
            /// </summary>
            public AesInputEncoding InputEncoding { get; set; } = AesInputEncoding.Base64Url;
        }

        /// <summary>
        /// Validates that a given key size is valid
        /// </summary>
        /// <param name="aes">An instance of <see cref="Aes"/></param>
        /// <param name="size">The required key size</param>
        /// <returns></returns>
        private static bool ValidateKeySize(Aes aes, int size)
        {
            LogMethodCall(_log);
            foreach (var keySize in aes.LegalKeySizes)
            {
                if (size.Equals(size))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Creates and initialises a new instance of <see cref="Aes"/>
        /// </summary>
        /// <param name="options">A set of options for the cipher instance</param>
        /// <returns></returns>
        /// <exception cref="AesHelperException">If the specified options are invalid</exception>
        private static Aes InitialiseAesCipher(AesEncryptionOptions options)
        {
            LogMethodCall(_log);
            LogVerbose(_log, $"Initialising AES with a suggested config of ({options.KeySize}, {options.Mode})");
            var aes = Aes.Create();
            if (!ValidateKeySize(aes, options.KeySize))
                throw new AesHelperException($"An invalid symmetric key size was specified - {options.KeySize} bits");
            aes.KeySize = options.KeySize;
            aes.Mode = options.Mode;
            aes.GenerateKey();
            aes.GenerateIV();
            return aes;
        }

        /// <summary>
        /// Encrypts the source byte array using a new AES key, and then uses the supplied certificate key material to wrap the
        /// key, IV and salt
        /// </summary>
        /// <param name="source">The source byte array to encrypt</param>
        /// <param name="certificate">A valid <see cref="X509Certificate2"/> which containing a public and private key pair</param>
        /// <param name="options">Sets the encryption and wrapping options</param>
        /// <returns>A <see cref="Pair"/>, where the left value is a byte array containing a wrapped key, salt and IV, and the
        /// right value contains the encrypted source</returns>
        /// <exception cref="AesHelperException"></exception>
        public static Pair<byte[], byte[]> EncryptAndWrapKey(byte[] source, X509Certificate2 certificate,
            AesEncryptionOptions options)
        {
            LogMethodCall(_log);
            LogVerbose(_log, $"Attempting encryption and then wrapping key using \"{options.KeyWrappingOptions}\"");
            if (options.KeyWrappingOptions == AesKeyWrappingOptions.WrapWithPrivateKey && !certificate.HasPrivateKey)
            {
                LogWarning(_log, "No private asymmetric key available for key wrapping");
                throw new AesHelperException("Private key specified for key wrapping operations, but no private key present");
            }

            var cipher = InitialiseAesCipher(options);

            return new Pair<byte[], byte[]>(null, null);
        }
    }
}