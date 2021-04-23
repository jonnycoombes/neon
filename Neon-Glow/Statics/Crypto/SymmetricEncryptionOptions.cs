using System.Security.Cryptography;

namespace JCS.Neon.Glow.Statics.Crypto
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