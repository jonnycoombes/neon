namespace JCS.Neon.Glow.Statics.Crypto
{
    /// <summary>
    ///     Class containing various password generation parameters
    /// </summary>
    public class PassphraseGenerationOptions
    {
        /// <summary>
        ///     The default password length
        /// </summary>
        public const int DefaultPasswordLength = 10;

        /// <summary>
        ///     The minimum allowable password length
        /// </summary>
        public const int MinimumPasswordLength = 8;

        /// <summary>
        ///     The required length for the password
        /// </summary>
        public int RequiredLength { get; set; } = DefaultPasswordLength;

        /// <summary>
        ///     Whether or not the password should be encoded Base64 after generation
        /// </summary>
        public bool EncodeBase64 { get; set; }
    }

    /// <summary>
    ///     A set of options defining a series of checks for passwords
    /// </summary>
    public class PassphraseValidationOptions
    {
        /// <summary>
        ///     The default minimum length for passwords
        /// </summary>
        public const int DefaultMinimumPasswordLength = 8;

        /// <summary>
        ///     The minimum required length for the password
        /// </summary>
        public int MinimumLength { get; set; } = DefaultMinimumPasswordLength;

        /// <summary>
        ///     Whether or not the password must contain special characters
        /// </summary>
        public bool MustContainSpecialCharacters { get; set; } = true;

        /// <summary>
        ///     Whether the password must contain both upper and lowercase letters
        /// </summary>
        public bool MustContainMixedCaseCharacters { get; set; } = true;

        /// <summary>
        ///     Whether the password must contain numerics
        /// </summary>
        public bool MustContainNumericCharacters { get; set; } = true;
    }

    /// <summary>
    ///     Builder for configuration options
    /// </summary>
    public class PassphraseGenerationOptionsBuilder
    {
        /// <summary>
        ///     The actual <see cref="PassphraseGenerationOptions" /> instance
        /// </summary>
        public readonly PassphraseGenerationOptions Options = new();

        /// <summary>
        ///     Sets the required length for new passphrases
        /// </summary>
        /// <param name="length">A non-zero length</param>
        /// <returns>The current builder instance</returns>
        public PassphraseGenerationOptionsBuilder SetRequiredLength(int length)
        {
            Options.RequiredLength = length;
            return this;
        }

        /// <summary>
        ///     Determines whether a passphrase should be base64 encoded or not
        /// </summary>
        /// <param name="b"><code>true</code> if passphrases should be base64 encoded, <code>false</code> otherwise</param>
        /// <returns>The current builder instance</returns>
        public PassphraseGenerationOptionsBuilder SetBase64Encoding(bool b)
        {
            Options.EncodeBase64 = b;
            return this;
        }
    }
}