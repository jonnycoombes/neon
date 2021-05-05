/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
namespace JCS.Neon.Glow.Statics.Crypto
{
    /// <summary>
    ///     Thrown be various passphrase helper methods
    /// </summary>
    public class PassphraseException : System.Exception
    {
        public PassphraseException(string? message) : base(message)
        {
        }

        public PassphraseException(string? message, System.Exception? innerException) : base(message, innerException)
        {
        }
    }
}