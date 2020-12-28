using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace JCS.Neon.Glow.Helpers.Crypto
{
    /// <summary>
    ///     Static class containing methods for dealing with X509 certificates
    /// </summary>
    public static class X509Helpers
    {
        /// <summary>
        ///     Given a path to a (PKCS12) .pfx file will attempt to import both public and private key
        ///     material in the form of an X509 certificate.
        /// </summary>
        /// <param name="source">The path to the source pfx file</param>
        /// <param name="pf">A function which will produce a passphrase for the pfx file</param>
        /// <returns>A valid <see cref="X509Certificate2"/></returns>
        /// <exception cref="X509HelperException">Thrown in the event of something going wrong.  Will contain an inner exception</exception>
        public static X509Certificate2 CertificatefromPfxFile(string source, Func<string> pf)
        {
            if (!File.Exists(source))
            {
                throw new X509HelperException($"Specified source PKCS12 file doesn't exist, or isn't accessible: {source}");
            }

            try
            {
                var cert = new X509Certificate2(source, pf());
                return cert;
            }
            catch (Exception ex)
            {
                throw new X509HelperException("Import failed, see inner exception", ex);
            }

        }

        #region Exceptions

        public class X509HelperException : Exception
        {
            public X509HelperException(string? message) : base(message)
            {
            }

            public X509HelperException(string? message, Exception? innerException) : base(message, innerException)
            {
            }
        }

        #endregion
    }
}