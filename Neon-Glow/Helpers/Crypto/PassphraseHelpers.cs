﻿using System;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;
using JCS.Neon.Glow.Types.Extensions;
using Serilog;
using static JCS.Neon.Glow.Helpers.General.LogHelpers;
using static JCS.Neon.Glow.Helpers.Crypto.EncodingHelpers;

namespace JCS.Neon.Glow.Helpers.Crypto
{

    #region Exceptions
    /// <summary>
    /// Thrown be various passphrase helper methods 
    /// </summary>
    public class PassphraseHelperException : Exception
    {
        public PassphraseHelperException(string? message) : base(message)
        {
        }

        public PassphraseHelperException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
    #endregion
    
    /// <summary>
    /// Class containing various password generation parameters 
    /// </summary>
    public class PassphraseGenerationOptions
    {
        /// <summary>
        /// The default password length
        /// </summary>
        public const int DefaultPasswordLength = 10;

        /// <summary>
        /// The minimum allowable password length
        /// </summary>
        public const int MinimumPasswordLength = 8;
        
        /// <summary>
        /// The required length for the password
        /// </summary>
        public int RequiredLength { get; set; } = DefaultPasswordLength;

        /// <summary>
        /// Whether or not the password should be encoded Base64 after generation
        /// </summary>
        public bool EncodeBase64 { get; set; } = false;

    }

    /// <summary>
    /// A set of options defining a series of checks for passwords
    /// </summary>
    public class PassphraseValidationOptions
    {

        /// <summary>
        /// Default constructor
        /// </summary>
        public PassphraseValidationOptions()
        {
            
        }
        
        /// <summary>
        /// The default minimum length for passwords
        /// </summary>
        public const int DefaultMinimumPasswordLength = 8;

        /// <summary>
        /// The minimum required length for the password
        /// </summary>
        public int MinimumLength { get; set; } = DefaultMinimumPasswordLength;

        /// <summary>
        /// Whether or not the password must contain special characters
        /// </summary>
        public bool MustContainSpecialCharacters { get; set; } = true;

        /// <summary>
        /// Whether the password must contain both upper and lowercase letters
        /// </summary>
        public bool MustContainMixedCaseCharacters { get; set; } = true;

        /// <summary>
        /// Whether the password must contain numerics
        /// </summary>
        public bool MustContainNumericCharacters { get; set; } = true;
    }
    
    /// <summary>
    /// Contains static helper methods for the generation and validation of passwords
    /// </summary>
    public static class PassphraseHelpers
    {
        /// <summary>
        /// Static logger
        /// </summary>
        private static ILogger _log = Log.ForContext(typeof(PassphraseHelpers));
        
        /// <summary>
        /// String containing valid upper case characters
        /// </summary>
        public const string UpperCaseCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// String containing valid lower case characters
        /// </summary>
        public const string LowerCaseCharacters = "abcdefghijklmnopqrstuvwxyz";

        /// <summary>
        /// String containing valid numeric characters
        /// </summary>
        public const string NumericCharacters = "0123456789";

        /// <summary>
        /// String containing valid 'special' characters
        /// </summary>
        public const string SpecialCharacters = "!@?_-+=~";

        /// <summary>
        /// Generates a random passphrase using the supplied options
        /// </summary>
        /// <param name="options">The <see cref="PassphraseGenerationOptions"/> to use</param>
        /// <returns>A randomly generated password, optionally base 64 encoded</returns>
        /// <exception cref="PassphraseHelperException">Thrown if the supplied options are invalid</exception>
        public static string GenerateRandomPassphrase(PassphraseGenerationOptions options)
        {
            LogMethodCall(_log);
            if (options.RequiredLength < PassphraseGenerationOptions.MinimumPasswordLength)
            {
                throw new PassphraseHelperException("The specified passphrase length doesn't meet minimum length requirements");
            }
            
            using (var rng = new RNGCryptoServiceProvider())
            {
                var sb = new StringBuilder();
                var randoms = new byte[2];
                for (var i = 0; i < options.RequiredLength; i++)
                {
                    randoms= randoms.Randomise(); 
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
                    return EncodeBase64(sb.ToString());
                else
                    return sb.ToString();
            }
        }

        /// <summary>
        /// Given a passphrase, attempts to validate it given a set of options
        /// </summary>
        /// <param name="passphrase">Passphrase to validate</param>
        /// <param name="options">The <see cref="PassphraseValidationOptions"/> to use</param>
        /// <returns><code>true</code> if the passphrase is validated, <code>false</code> otherwise</returns>
        /// <exception cref="NotImplementedException"></exception>
        public static bool ValidatePassphrase(string passphrase, PassphraseValidationOptions options)
        {
            //TODO 
            throw new NotImplementedException();
        }
        
    }
}