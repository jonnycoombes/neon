using System;
using System.Security.Cryptography.X509Certificates;
using JCS.Neon.Glow.Types;
using JCS.Neon.Glow.Utilities.Cryptography;
using JCS.Neon.Glow.Utilities.General;
using Xunit;

namespace JCS.Neon.Glow.Test.Utilities.Cryptography
{
    /// <summary>
    ///     Test suite for <see cref="Glow.Utilities.Cryptography.X509Certificates" />
    /// </summary>
    [Trait("Category", "Crypto")]
    public class X509CertificatesTests : TestBase, IDisposable
    {
        public void Dispose()
        {
        }

        /// <summary>
        ///     Just loads a test certificate for use during tests
        /// </summary>
        /// <returns></returns>
        private X509Certificate2 LoadKnownTestCertificate(string passphrase = "test")
        {
            var sshOption = Files.GetHomeSubdirectoryPath(".config", "neon", "glow", "test.pfx");
            var result = sshOption.Fold(path =>
            {
                var cert = X509Certificates.LoadFromFile(path, () => passphrase);
                return cert;
            }, () => new X509Certificate2());
            return result;
        }

        [Fact(DisplayName = "Import from an invalid path should fail with an exception")]
        [Trait("Category", "Crypto")]
        public void LoadFromInvalidPath()
        {
            Assert.Throws<X509Certificates.X509CertificateException>(() =>
            {
                var cert = X509Certificates.LoadFromFile("non-sense path", () => "whatever");
            });
        }

        [Fact(DisplayName = "Can load a valid pfx file with a valid passphrase")]
        [Trait("Category", "Crypto")]
        public void LoadPfxFromFile()
        {
            var result = LoadKnownTestCertificate();
            Assert.Equal("neon-glow-test.jcs-software.co.uk", result.GetNameInfo(X509NameType.SimpleName, false));
            Assert.NotNull(result.PublicKey);
            Assert.NotNull(result.PrivateKey);
        }

        [Fact(DisplayName = "Will throw an exception if an invalid passphrase is specified")]
        [Trait("Category", "Crypto")]
        public void LoadPfxWithInvalidPassphrase()
        {
            Assert.Throws<X509Certificates.X509CertificateException>(() => { LoadKnownTestCertificate("bollocks"); });
        }

        [Fact(DisplayName = "Import from an empty byte array must fail with an exception")]
        [Trait("Category", "Crypto")]
        public void LoadFromInvalidByteArray()
        {
            Assert.Throws<X509Certificates.X509CertificateException>(() =>
            {
                X509Certificates.LoadFromByteArray(new byte[] { }, () => "whatever");
            });
        }
    }
}