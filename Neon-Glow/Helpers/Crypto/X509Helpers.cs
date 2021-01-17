using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Serilog;
using static JCS.Neon.Glow.Helpers.General.LogHelpers;

namespace JCS.Neon.Glow.Helpers.Crypto
{
    /// <summary>
    ///     Static class containing methods for dealing with X509 certificates
    /// </summary>
    public static class X509Helpers
    {
        /// <summary>
        /// Static logger
        /// </summary>
        private static ILogger _log = Log.ForContext(typeof(X509Helpers));
        
        /// <summary>
        ///     Given a path to a (PKCS12) .pfx file will attempt to import both public and private key
        ///     material in the form of an X509 certificate.
        /// </summary>
        /// <param name="source">The path to the source pfx file</param>
        /// <param name="pf">A function which will produce a passphrase for the pfx file</param>
        /// <returns>A valid <see cref="X509Certificate2"/></returns>
        /// <exception cref="X509HelperException">Thrown in the event of something going wrong.  Will contain an inner exception</exception>
        public static X509Certificate2 LoadCertificateFromPfxFile(string source, Func<string> pf, bool exportable = true)
        {
            LogMethodCall(_log);
            LogVerbose(_log, $"Attempting x509 certificate load from \"{source}\"");
            if (!File.Exists(source))
            {
                LogWarning(_log, $"Specified source for x509 certificate doesn't exist, or can't be accessed");
                throw new X509HelperException($"Specified source PKCS12 file doesn't exist, or isn't accessible: {source}");
            }

            try
            {
                if (exportable)
                {
                    var cert = new X509Certificate2(source, pf(), X509KeyStorageFlags.Exportable);
                    return cert;
                }
                else
                {
                    var cert = new X509Certificate2(source, pf());
                    return cert;
                }
            }
            catch (Exception ex)
            {
                throw new X509HelperException("Import failed, see inner exception", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="passphrase"></param>
        /// <param name="exportable"></param>
        /// <returns></returns>
        public static X509Certificate2 LoadCertificateFromPfxFile(string source, string passphrase, bool exportable = true)
        {
            LogMethodCall(_log);
            return LoadCertificateFromPfxFile(source, () => passphrase, exportable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pf"></param>
        /// <param name="exportable"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static X509Certificate2 LoadCertificateFromByteArray(byte[] source, Func<string> pf, bool exportable = true)
        {
            LogMethodCall(_log);
            throw new NotImplementedException();
        }

        public static X509Certificate2 LoadCertificateFromByteArray(byte[] source, string passphrase, bool exportable = true)
        {
            return LoadCertificateFromByteArray(source, () => passphrase, exportable);
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