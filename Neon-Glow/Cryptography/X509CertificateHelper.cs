#region

using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using JCS.Neon.Glow.Logging;
using Serilog;

#endregion

namespace JCS.Neon.Glow.Cryptography
{
    /// <summary>
    ///     Static class containing methods for dealing with X509 certificates
    /// </summary>
    public static class X509CertificateHelper
    {
        /// <summary>
        ///     Static logger
        /// </summary>
        private static readonly ILogger _log = Log.ForContext(typeof(X509CertificateHelper));

        /// <summary>
        ///     Given a path to a (PKCS12) .pfx file will attempt to import both public and private key
        ///     material in the form of an X509 certificate.
        /// </summary>
        /// <param name="source">The path to the source pfx file</param>
        /// <param name="pf">A function which will produce a passphrase for the pfx file</param>
        /// <param name="exportable"></param>
        /// <returns>A valid <see cref="X509Certificate2" /></returns>
        /// <exception cref="X509CertificateHelperException">Thrown in the event of something going wrong.  Will contain an inner exception</exception>
        public static X509Certificate2 ImportFromFile(string source, Func<string> pf, bool exportable = true)
        {
            LogHelper.MethodCall(_log);
            LogHelper.Verbose(_log, $"Attempting x509 certificate load from \"{source}\"");
            if (!File.Exists(source))
            {
                LogHelper.Warning(_log, "Specified source for x509 certificate doesn't exist, or can't be accessed");
                throw Exceptions.ExceptionHelper.LoggedException<X509CertificateHelperException>(_log,
                    $"Specified source PKCS12 file doesn't exist, or isn't accessible: {source}");
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
                throw Exceptions.ExceptionHelper.LoggedException<X509CertificateHelperException>(_log,
                    "Import failed, see inner exception", ex);
            }
        }

        /// <summary>
        ///     Given a path to a (PKCS12) .pfx file will attempt to import both public and private key
        ///     material in the form of an X509 certificate.
        /// </summary>
        /// <param name="source">The path to the file to use</param>
        /// <param name="passphrase">A passphrase to be used in order to decrypt any private key material</param>
        /// <param name="exportable">Whether or not the private key should be marked as exportable</param>
        /// <returns></returns>
        public static X509Certificate2 ImportFromFile(string source, string passphrase, bool exportable = true)
        {
            LogHelper.MethodCall(_log);
            return ImportFromFile(source, () => passphrase, exportable);
        }

        /// <summary>
        ///     Loads an x509 certificate from a byte array source which should contain the certificate material in PKCS12
        ///     format.  The passphrase function is used to decrypt any included private key material
        /// </summary>
        /// <param name="source">The byte array source containing the x509 certificate and associated key material in PKCS12 format</param>
        /// <param name="pf">A lambda which returns a passphrase which will be used to decrypt any private key material</param>
        /// <param name="exportable">Whether or not the private key should be exportable or not</param>
        /// <returns></returns>
        /// <exception cref="X509CertificateHelperException">Thrown if the import fails</exception>
        public static X509Certificate2 ImportFromByteArray(byte[] source, Func<string> pf, bool exportable = true)
        {
            LogHelper.MethodCall(_log);
            try
            {
                return exportable ? new X509Certificate2(source, pf(), X509KeyStorageFlags.Exportable) : new X509Certificate2(source, pf());
            }
            catch (Exception ex)
            {
                throw Exceptions.ExceptionHelper.LoggedException<X509CertificateHelperException>(_log,
                    "Import failed, see inner exception", ex);
            }
        }

        /// <summary>
        ///     Loads an x509 certificate from a byte array source which should contain the certificate material in PKCS12
        ///     format.  The passphrase function is used to decrypt any included private key material
        /// </summary>
        /// <param name="source">Byte array containing the source material for the certificate in PKCS12 format</param>
        /// <param name="passphrase">The passphrase to be used to decrypt any private key material</param>
        /// <param name="exportable">Whether the private key should be marked as exportable</param>
        /// <returns></returns>
        /// <exception cref="X509CertificateHelperException">Thrown if the import fails</exception>
        public static X509Certificate2 ImportFromByteArray(byte[] source, string passphrase, bool exportable = true)
        {
            LogHelper.MethodCall(_log);
            return ImportFromByteArray(source, () => passphrase, exportable);
        }

        /// <summary>
        ///     Exports a given certificate to a byte array, using a PKCS12 encoding and a supplied passphrase
        /// </summary>
        /// <param name="certificate">The certificate to export</param>
        /// <param name="pf">Function that will return a passphrase to use during the export</param>
        /// <returns></returns>
        /// <exception cref="X509CertificateHelperException">If an error occurs during the export</exception>
        public static byte[] ExportToByteArray(X509Certificate2 certificate, Func<string> pf)
        {
            LogHelper.MethodCall(_log);
            try
            {
                return certificate.Export(X509ContentType.Pkcs12, pf());
            }
            catch (Exception ex)
            {
                throw Exceptions.ExceptionHelper.LoggedException<X509CertificateHelperException>(_log,
                    "Export failed, see inner exception", ex);
            }
        }

        /// <summary>
        ///     Convenience wrapper which just calls
        ///     <see cref="ExportToByteArray(System.Security.Cryptography.X509Certificates.X509Certificate2,System.Func{string})" />
        /// </summary>
        /// <param name="certificate">The certificate to export</param>
        /// <param name="passphrase">The passphrase to use</param>
        /// <returns>A byte array which contains the PKCS12 encoding of the certificate and key material</returns>
        /// <exception cref="X509CertificateHelperException">If an error occurs during the export</exception>
        public static byte[] ExportToByteArray(X509Certificate2 certificate, string passphrase)
        {
            LogHelper.MethodCall(_log);
            return ExportToByteArray(certificate, () => passphrase);
        }

        /// <summary>
        ///     Exports a given certificate to a PKCS12 file, using a given passphrase to secure private key material
        /// </summary>
        /// <param name="path">The path to export to (e.g. certificate.pkcs12, certificate.pks)</param>
        /// <param name="certificate">The certificate to export</param>
        /// <param name="pf">Function that returns a passphrase</param>
        /// <exception cref="X509CertificateHelperException">If an error occurs during the export</exception>
        public static async Task ExportToFile(string path, X509Certificate2 certificate, Func<string> pf)
        {
            LogHelper.MethodCall(_log);
            try
            {
                await using var outFile = new FileStream(path, FileMode.OpenOrCreate);
                var export = ExportToByteArray(certificate, pf());
                await outFile.WriteAsync(export);
                await outFile.FlushAsync();
            }
            catch (Exception ex)
            {
                throw Exceptions.ExceptionHelper.LoggedException<X509CertificateHelperException>(_log,
                    "Export failed, see inner exception", ex);
            }
        }

        /// <summary>
        ///     Convenience wrapper which just calls
        ///     <see cref="ExportToFile(string,System.Security.Cryptography.X509Certificates.X509Certificate2,System.Func{string})" />
        /// </summary>
        /// <param name="path">The path to export to</param>
        /// <param name="certificate">The certificate to export</param>
        /// <param name="passphrase">The passphrase to use in order to secure key material</param>
        /// <exception cref="X509CertificateHelperException">If an error occurs during the export</exception>
        public static Task ExportToFile(string path, X509Certificate2 certificate, string passphrase)
        {
            LogHelper.MethodCall(_log);
            return ExportToFile(path, certificate, () => passphrase);
        }

        /// <summary>
        ///     Class used to specify options for certificate generation
        /// </summary>
        public class X509CertificateGenerationOptions
        {
        }

        #region Exceptions

        public class X509CertificateHelperException : Exception
        {
            public X509CertificateHelperException(string? message) : base(message)
            {
            }

            public X509CertificateHelperException(string? message, Exception? innerException) : base(message, innerException)
            {
            }
        }

        #endregion
    }
}