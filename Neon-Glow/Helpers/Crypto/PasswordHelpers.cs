using System;

namespace JCS.Neon.Glow.Helpers.Crypto
{

    /// <summary>
    /// Class containing various password generation parameters 
    /// </summary>
    public class PasswordGenerationOptions
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
        
    }
    
    /// <summary>
    /// Contains static helper methods for the generation and validation of passwords
    /// </summary>
    public class PasswordHelpers
    {
        /// <summary>
        /// String containing valid upper case characters
        /// </summary>
        public const string UpperCaseCharacters = "";

        /// <summary>
        /// String containing valid lower case characters
        /// </summary>
        public const string LowerCaseCharacters = "";

        /// <summary>
        /// String containing valid numeric characters
        /// </summary>
        public const string NumericCharacters = "";

        /// <summary>
        /// String containing valid 'special' characters
        /// </summary>
        public const string SpecialCharacters = "";

    }
}