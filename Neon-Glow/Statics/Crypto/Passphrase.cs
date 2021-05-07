/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System;
using System.Security.Cryptography;
using System.Text;
using JCS.Neon.Glow.Types.Extensions;
using Serilog;

#endregion

namespace JCS.Neon.Glow.Statics.Crypto
{
    /// <summary>
    ///     Contains static helper methods for the generation and validation of passwords
    /// </summary>
    public static class Passphrase
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
        private static readonly ILogger _log = Log.ForContext(typeof(Passphrase));

        /// <summary>
        ///     Generates a random passphrase using the supplied options
        /// </summary>
        /// <param name="configureAction">The <see cref="PassphraseGenerationOptions" /></param>
        /// <returns>A randomly generated password, optionally base 64 encoded</returns>
        /// <exception cref="PassphraseException">Thrown if the supplied options are invalid</exception>
        public static string GenerateRandomPassphrase(Action<PassphraseGenerationOptionsBuilder> configureAction)
        {
            Logging.MethodCall(_log);
            var builder = new PassphraseGenerationOptionsBuilder();
            configureAction(builder);
            var options = builder.Build();

            if (options.RequiredLength < PassphraseGenerationOptions.MinimumPasswordLength)
            {
                throw Exceptions.LoggedException<PassphraseException>(_log,
                    "The specified passphrase length doesn't meet minimum length requirements");
            }

            using var rng = new RNGCryptoServiceProvider();
            var sb = new StringBuilder();
            var randoms = new byte[2];
            for (var i = 0; i < options.RequiredLength; i++)
            {
                randoms = randoms.Randomise();
                int charResidue;
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

            return options.EncodeBase64 ? Encodings.Base64Encode(sb.ToString()) : sb.ToString();
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