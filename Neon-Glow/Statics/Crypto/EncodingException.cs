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
    public class EncodingException : Exception
    {
        public EncodingException(string? message) : base(message)
        {
        }

        public EncodingException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}