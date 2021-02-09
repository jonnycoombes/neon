#region

using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using JCS.Neon.Glow.Cryptography;
using Xunit;

#endregion

namespace JCS.Neon.Glow.Test.Cryptography
{
    /// <summary>
    ///     Test suite for <see cref="X509CertificateHelper" />
    /// </summary>
    [Trait("Category", "Cryptography")]
    public class X509CertificatesTests : TestBase, IDisposable
    {
        public void Dispose()
        {
        }

        [Fact(DisplayName = "Import from an invalid path should fail with an exception")]
        [Trait("Category", "Cryptography")]
        public void LoadFromInvalidPath()
        {
            Assert.Throws<X509CertificateHelper.X509CertificateHelperException>(() =>
            {
                var cert = X509CertificateHelper.ImportFromFile("non-sense path", () => "whatever");
            });
        }

        [Fact(DisplayName = "Can load a valid pfx file with a valid passphrase")]
        [Trait("Category", "Cryptography")]
        public void LoadPfxFromFile()
        {
            var result = LoadTestCertificate();
            Assert.Equal("neon-glow-test.jcs-software.co.uk", result.GetNameInfo(X509NameType.SimpleName, false));
            Assert.NotNull(result.PublicKey);
            Assert.NotNull(result.PrivateKey);
        }

        [Fact(DisplayName = "Will throw an exception if an invalid passphrase is specified")]
        [Trait("Category", "Cryptography")]
        public void LoadPfxWithInvalidPassphrase()
        {
            Assert.Throws<X509CertificateHelper.X509CertificateHelperException>(() => { LoadTestCertificate("bollocks"); });
        }

        [Fact(DisplayName = "Import from an empty byte array must fail with an exception")]
        [Trait("Category", "Cryptography")]
        public void LoadFromInvalidByteArray()
        {
            Assert.Throws<X509CertificateHelper.X509CertificateHelperException>(() =>
            {
                X509CertificateHelper.ImportFromByteArray(new byte[] { }, () => "whatever");
            });
        }

        [Fact(DisplayName = "Must be able to export, and then reimport a certificate with a given passphrase (byte array)")]
        [Trait("Category", "Cryptography")]
        public async void ExportAndImportPfx()
        {
            var original = LoadTestCertificate();
            var exported = X509CertificateHelper.ExportToByteArray(original, "test");
            var recovered = X509CertificateHelper.ImportFromByteArray(exported, "test");
            Assert.Equal("neon-glow-test.jcs-software.co.uk", recovered.GetNameInfo(X509NameType.SimpleName, false));
            var exportPath = Path.Combine(Directory.GetCurrentDirectory(), "testExport.pkcs12");
            await X509CertificateHelper.ExportToFile(exportPath, original, "test");
            var recoveredFromFile = X509CertificateHelper.ImportFromFile(exportPath, "test");
            Assert.Equal("neon-glow-test.jcs-software.co.uk", recoveredFromFile.GetNameInfo(X509NameType.SimpleName, false));
            original.Dispose();
            recovered.Dispose();
            recoveredFromFile.Dispose();
            File.Delete(exportPath);
        }
    }
}