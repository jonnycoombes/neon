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

        [Fact(DisplayName = "Import from an invalid path should fail with an exception")]
        [Trait("Category", "Crypto")]
        public void LoadFromInvalidPath()
        {
            Assert.Throws<X509Certificates.X509CertificateException>(() =>
            {
                var cert = X509Certificates.ImportFromFile("non-sense path", () => "whatever");
            });
        }

        [Fact(DisplayName = "Can load a valid pfx file with a valid passphrase")]
        [Trait("Category", "Crypto")]
        public void LoadPfxFromFile()
        {
            var result = LoadTestCertificate();
            Assert.Equal("neon-glow-test.jcs-software.co.uk", result.GetNameInfo(X509NameType.SimpleName, false));
            Assert.NotNull(result.PublicKey);
            Assert.NotNull(result.PrivateKey);
        }

        [Fact(DisplayName = "Will throw an exception if an invalid passphrase is specified")]
        [Trait("Category", "Crypto")]
        public void LoadPfxWithInvalidPassphrase()
        {
            Assert.Throws<X509Certificates.X509CertificateException>(() => { LoadTestCertificate("bollocks"); });
        }

        [Fact(DisplayName = "Import from an empty byte array must fail with an exception")]
        [Trait("Category", "Crypto")]
        public void LoadFromInvalidByteArray()
        {
            Assert.Throws<X509Certificates.X509CertificateException>(() =>
            {
                X509Certificates.ImportFromByteArray(new byte[] { }, () => "whatever");
            });
        }
    }
}