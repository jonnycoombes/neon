/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System;

#endregion

namespace JCS.Neon.Glow.Statics.Crypto
{
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
}