using System;
using System.IO;
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

        [Fact(DisplayName = "Must be able to export, and then reimport a certificate with a given passphrase (byte array)")]
        [Trait("Category", "Crypto")]
        public async void ExportAndImportPfx()
        {
            var original = LoadTestCertificate();
            var exported = X509Certificates.ExportToByteArray(original, "test");
            var recovered = X509Certificates.ImportFromByteArray(exported, "test");
            Assert.Equal("neon-glow-test.jcs-software.co.uk", recovered.GetNameInfo(X509NameType.SimpleName, false));
            var exportPath = Path.Combine(Directory.GetCurrentDirectory(), "testExport.pkcs12");
            await X509Certificates.ExportToFile(exportPath, original, "test");
            var recoveredFromFile = X509Certificates.ImportFromFile(exportPath, "test");
            Assert.Equal("neon-glow-test.jcs-software.co.uk", recoveredFromFile.GetNameInfo(X509NameType.SimpleName, false));
            original.Dispose();
            recovered.Dispose();
            recoveredFromFile.Dispose();
            File.Delete(exportPath);
        }
    }
}