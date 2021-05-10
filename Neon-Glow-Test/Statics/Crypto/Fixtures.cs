/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography.X509Certificates;
using JCS.Neon.Glow.Statics.IO;
using JCS.Neon.Glow.Types;
using X509Certificate = JCS.Neon.Glow.Statics.Crypto.X509Certificate;

#endregion

namespace JCS.Neon.Glow.Test.Statics.Crypto
{
    /// <summary>
    ///     Fixture class for crypto-related tests
    /// </summary>
    public class Fixtures
    {
        /// <summary>
        ///     The test X509 certificate
        /// </summary>
        public X509Certificate2 Certificate => LoadCertificate();

        public Fixtures()
        {
        }

        /// <summary>
        ///     Just loads a test certificate for use during tests
        /// </summary>
        /// <returns></returns>
        public X509Certificate2 LoadCertificate(string passphrase = "test")
        {
            var sshOption = File.GetHomeSubdirectoryPath(".config", "neon", "glow", "test.pfx");
            var result = sshOption.Fold(path =>
            {
                var cert = X509Certificate.ImportFromFile(path, () => passphrase);
                return cert;
            }, () => new X509Certificate2());
            return result;
        }
    }
}