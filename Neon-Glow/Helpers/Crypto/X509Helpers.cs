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
        /// Given a path to a (PKCS12) .pfx file will attempt to import both public and private key
        /// material in the form of an X509 certificate.
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
        /// Given a path to a (PKCS12) .pfx file will attempt to import both public and private key
        /// material in the form of an X509 certificate.
        /// </summary>
        /// <param name="source">The path to the file to use</param>
        /// <param name="passphrase">A passphrase to be used in order to decrypt any private key material</param>
        /// <param name="exportable">Whether or not the private key should be marked as exportable</param>
        /// <returns></returns>
        public static X509Certificate2 LoadCertificateFromPfxFile(string source, string passphrase, bool exportable = true)
        {
            LogMethodCall(_log);
            return LoadCertificateFromPfxFile(source, () => passphrase, exportable);
        }

        /// <summary>
        /// Loads an x509 certificate from a byte array source which should contain the certificate material in PKCS12
        /// format.  The passphrase function is used to decrypt any included private key material
        /// </summary>
        /// <param name="source">The byte array source containing the x509 certificate and associated key material in PKCS12 format</param>
        /// <param name="pf">A lambda which returns a passphrase which will be used to decrypt any private key material</param>
        /// <param name="exportable">Whether or not the private key should be exportable or not</param>
        /// <returns></returns>
        /// <exception cref="X509HelperException">Thrown if the import fails</exception>
        public static X509Certificate2 LoadCertificateFromByteArray(byte[] source, Func<string> pf, bool exportable = true)
        {
            LogMethodCall(_log);
            try
            {
                if (exportable)
                {
                    return new X509Certificate2(source, pf(), X509KeyStorageFlags.Exportable);
                }
                else
                {
                    return new X509Certificate2(source, pf());
                }
            }
            catch (Exception ex)
            {
                throw new X509HelperException("Import failed, see inner exception", ex);
            }
        }

        /// <summary>
        /// Loads an x509 certificate from a byte array source which should contain the certificate material in PKCS12
        /// format.  The passphrase function is used to decrypt any included private key material
        /// </summary>
        /// <param name="source">Byte array containing the source material for the certificate in PKCS12 format</param>
        /// <param name="passphrase">The passphrase to be used to decrypt any private key material</param>
        /// <param name="exportable">Whether the private key should be marked as exportable</param>
        /// <returns></returns>
        /// <exception cref="X509HelperException">Thrown if the import fails</exception>
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