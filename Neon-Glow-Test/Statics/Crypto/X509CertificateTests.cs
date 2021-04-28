/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Xunit;
using X509Certificate = JCS.Neon.Glow.Statics.Crypto.X509Certificate;

#endregion

namespace JCS.Neon.Glow.Test.Statics.Crypto
{
    /// <summary>
    ///     Test suite for <see cref="X509Certificate" />
    /// </summary>
    [Trait("Category", "Cryptography")]
    public class X509CertificateTests : TestBase, IDisposable
    {
        public void Dispose()
        {
        }

        [Fact(DisplayName = "Import from an invalid path should fail with an exception")]
        [Trait("Category", "Cryptography")]
        public void LoadFromInvalidPath()
        {
            Assert.Throws<X509Certificate.X509CertificateHelperException>(() =>
            {
                var cert = X509Certificate.ImportFromFile("non-sense path", () => "whatever");
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
            Assert.Throws<X509Certificate.X509CertificateHelperException>(() => { LoadTestCertificate("bollocks"); });
        }

        [Fact(DisplayName = "Import from an empty byte array must fail with an exception")]
        [Trait("Category", "Cryptography")]
        public void LoadFromInvalidByteArray()
        {
            Assert.Throws<X509Certificate.X509CertificateHelperException>(() =>
            {
                X509Certificate.ImportFromByteArray(new byte[] { }, () => "whatever");
            });
        }

        [Fact(DisplayName = "Must be able to export, and then reimport a certificate with a given passphrase (byte array)")]
        [Trait("Category", "Cryptography")]
        public async void ExportAndImportPfx()
        {
            var original = LoadTestCertificate();
            var exported = X509Certificate.ExportToByteArray(original, "test");
            var recovered = X509Certificate.ImportFromByteArray(exported, "test");
            Assert.Equal("neon-glow-test.jcs-software.co.uk", recovered.GetNameInfo(X509NameType.SimpleName, false));
            var exportPath = Path.Combine(Directory.GetCurrentDirectory(), "testExport.pkcs12");
            await X509Certificate.ExportToFile(exportPath, original, "test");
            var recoveredFromFile = X509Certificate.ImportFromFile(exportPath, "test");
            Assert.Equal("neon-glow-test.jcs-software.co.uk", recoveredFromFile.GetNameInfo(X509NameType.SimpleName, false));
            original.Dispose();
            recovered.Dispose();
            recoveredFromFile.Dispose();
            File.Delete(exportPath);
        }
    }
}