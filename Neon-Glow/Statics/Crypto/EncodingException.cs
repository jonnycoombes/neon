/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
namespace JCS.Neon.Glow.Statics.Crypto
{
    public class EncodingException : System.Exception
    {
        public EncodingException(string? message) : base(message)
        {
        }

        public EncodingException(string? message, System.Exception? innerException) : base(message, innerException)
        {
        }
    }
}