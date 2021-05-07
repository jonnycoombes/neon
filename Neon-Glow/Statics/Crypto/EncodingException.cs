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