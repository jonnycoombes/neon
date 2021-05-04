#region

using System;

#endregion

namespace JCS.Neon.Glow.Statics.Crypto
{
    public class X509CertificateException : Exception
    {
        public X509CertificateException(string? message) : base(message)
        {
        }

        public X509CertificateException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}