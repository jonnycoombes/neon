using System;
using System.Security.Cryptography.X509Certificates;
using JCS.Neon.Glow.Types;
using JCS.Neon.Glow.Utilities.Cryptography;
using Xunit;
using Xunit.Sdk;
using static JCS.Neon.Glow.Utilities.Cryptography.X509Certificates;
using static JCS.Neon.Glow.Utilities.General.Files;

namespace JCS.Neon.Glow.Test.Helpers.Crypto
{
    /// <summary>
    /// Test suite for <see cref="X509Certificates"/>
    /// </summary>
    [Trait("Test Type", "Unit")]
    [Trait("Target Class", "X509CertificateHelper")]
    public class X509HelpersTests : TestBase, IDisposable
    {
        /// <summary>
        /// Just loads a test certificate for use during tests
        /// </summary>
        /// <returns></returns>
        private X509Certificate2 LoadKnownTestCertificate(string passphrase = "test")
        {
            var sshOption = GetHomeSubdirectoryPath(new string[] {".config", "neon", "glow", "test.pfx"});
            var result = sshOption.Fold(path =>
            {
                var cert = LoadCertificateFromPfxFile(path, () => passphrase);
                return cert;
            }, () => new X509Certificate2());
            return result;
        }

        [Fact(DisplayName = "Import from an invalid path should fail with an exception")]
        [Trait("Test Type", "Unit")]
        [Trait("Target Class", "X509CertificateHelper")]
        public void LoadFromInvalidPath()
        {
            Assert.Throws<X509HelperException>(() =>
            {
                var cert = LoadCertificateFromPfxFile("non-sense path", () => "whatever");
            });
        }

        [Fact(DisplayName = "Can load a valid pfx file with a valid passphrase")]
        [Trait("Test Type", "Unit")]
        [Trait("Target Class", "X509CertificateHelper")]
        public void LoadPfxFromFile()
        {
            var result = LoadKnownTestCertificate();
            Assert.Equal("neon-glow-test.jcs-software.co.uk", result.GetNameInfo(X509NameType.SimpleName, false));
            Assert.NotNull(result.PublicKey);
            Assert.NotNull(result.PrivateKey);
        }

        [Fact(DisplayName = "Will throw an exception if an invalid passphrase is specified")]
        [Trait("Test Type", "Unit")]
        [Trait("Target Class", "X509CertificateHelper")]
        public void LoadPfxWithInvalidPassphrase()
        {
            Assert.Throws<X509HelperException>(() => { LoadKnownTestCertificate("bollocks"); });
        }

        [Fact(DisplayName = "Import from an empty byte array must fail with an exception")]
        [Trait("Test Type", "Unit")]
        [Trait("Target Class", "X509CertificateHelper")]
        public void LoadFromInvalidByteArray()
        {
            Assert.Throws<X509HelperException>(() => { LoadCertificateFromByteArray(new byte[] { }, () => "whatever"); });
        }
        
        

        public void Dispose()
        {
        }
    }
}