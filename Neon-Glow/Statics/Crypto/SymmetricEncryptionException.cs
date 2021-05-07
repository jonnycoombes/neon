#region

using System;

#endregion

namespace JCS.Neon.Glow.Statics.Crypto
{
    public class SymmetricEncryptionException : Exception
    {
        public SymmetricEncryptionException(string? message) : base(message)
        {
        }

        public SymmetricEncryptionException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}