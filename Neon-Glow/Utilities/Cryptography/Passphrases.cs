using System;
using System.Security.Cryptography;
using System.Text;
using JCS.Neon.Glow.Types.Extensions;
using JCS.Neon.Glow.Utilities.General;
using Serilog;

namespace JCS.Neon.Glow.Utilities.Cryptography
{
    #region Exceptions

    /// <summary>
    ///     Thrown be various passphrase helper methods
    /// </summary>
    public class PassphraseException : Exception
    {
        public PassphraseException(string? message) : base(message)
        {
        }

        public PassphraseException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }

    #endregion

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
        public PassphraseGenerationOptions Options = new();

        public PassphraseGenerationOptionsBuilder SetRequiredLength(int length)
        {
            Options.RequiredLength = length;
            return this;
        }

        public PassphraseGenerationOptionsBuilder SetBase64Encoding(bool b)
        {
            Options.EncodeBase64 = b;
            return this;
        }
    }

    /// <summary>
    ///     Contains static helper methods for the generation and validation of passwords
    /// </summary>
    public static class Passphrases
    {
        /// <summary>
        ///     String containing valid upper case characters
        /// </summary>
        public const string UpperCaseCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        ///     String containing valid lower case characters
        /// </summary>
        public const string LowerCaseCharacters = "abcdefghijklmnopqrstuvwxyz";

        /// <summary>
        ///     String containing valid numeric characters
        /// </summary>
        public const string NumericCharacters = "0123456789";

        /// <summary>
        ///     String containing valid 'special' characters
        /// </summary>
        public const string SpecialCharacters = "!@?_-+=~";

        /// <summary>
        ///     Static logger
        /// </summary>
        private static readonly ILogger _log = Log.ForContext(typeof(Passphrases));

        /// <summary>
        ///     Generates a random passphrase using the supplied options
        /// </summary>
        /// <param name="configureAction">The <see cref="PassphraseGenerationOptions" /></param>
        /// <returns>A randomly generated password, optionally base 64 encoded</returns>
        /// <exception cref="PassphraseException">Thrown if the supplied options are invalid</exception>
        public static string GenerateRandomPassphrase(Action<PassphraseGenerationOptionsBuilder> configureAction)
        {
            Logs.MethodCall(_log);
            var builder = new PassphraseGenerationOptionsBuilder();
            configureAction(builder);
            var options = builder.Options;

            if (options.RequiredLength < PassphraseGenerationOptions.MinimumPasswordLength)
            {
                throw Exceptions.LoggedException<PassphraseException>(_log,
                    "The specified passphrase length doesn't meet minimum length requirements");
            }

            using (var rng = new RNGCryptoServiceProvider())
            {
                var sb = new StringBuilder();
                var randoms = new byte[2];
                for (var i = 0; i < options.RequiredLength; i++)
                {
                    randoms = randoms.Randomise();
                    var charResidue = 0;
                    switch (randoms[0] % 4)
                    {
                        case 0:
                            charResidue = randoms[1] % UpperCaseCharacters.Length;
                            sb.Append(UpperCaseCharacters[charResidue]);
                            break;
                        case 1:
                            charResidue = randoms[1] % LowerCaseCharacters.Length;
                            sb.Append(LowerCaseCharacters[charResidue]);
                            break;
                        case 2:
                            charResidue = randoms[1] % NumericCharacters.Length;
                            sb.Append(NumericCharacters[charResidue]);
                            break;
                        default:
                            charResidue = randoms[1] % SpecialCharacters.Length;
                            sb.Append(SpecialCharacters[charResidue]);
                            break;
                    }
                }

                if (options.EncodeBase64)
                    return Encoding.EncodeBase64(sb.ToString());
                return sb.ToString();
            }
        }

        /// <summary>
        ///     Given a passphrase, attempts to validate it given a set of options
        /// </summary>
        /// <param name="passphrase">Passphrase to validate</param>
        /// <param name="options">The <see cref="PassphraseValidationOptions" /> to use</param>
        /// <returns><code>true</code> if the passphrase is validated, <code>false</code> otherwise</returns>
        /// <exception cref="NotImplementedException"></exception>
        public static bool ValidatePassphrase(string passphrase, PassphraseValidationOptions options)
        {
            //TODO 
            throw new NotImplementedException();
        }
    }
}