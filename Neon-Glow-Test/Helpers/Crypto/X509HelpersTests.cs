using System;
using System.Security.Cryptography.X509Certificates;
using JCS.Neon.Glow.Types;
using Xunit;
using Xunit.Sdk;
using static JCS.Neon.Glow.Helpers.Crypto.X509Helpers;
using static JCS.Neon.Glow.Helpers.General.FileHelpers;

namespace JCS.Neon.Glow.Test.Helpers.Crypto
{
    /// <summary>
    /// Test suite for <see cref="JCS.Neon.Glow.Helpers.Crypto.X509Helpers"/>
    /// </summary>
    [Trait("Test Type", "Unit")]
    [Trait("Target Class", "X509CertificateHelper")]
    public class X509HelpersTests : IDisposable
    {

        [Fact(DisplayName = "Can load a valid pfx file with a valid passphrase")]
        [Trait("Test Type", "Unit")]
        [Trait("Target Class", "X509CertificateHelper")]
        public void LoadPfxFromFile()
        {
            var sshOption = GetHomeSubdirectoryPath(new string[]{".config", "neon", "glow", "test.pfx"});
            var result = sshOption.Fold(path =>
            {
                var cert = CertificatefromPfxFile(path, () => "test");
                return cert;

            }, () => new X509Certificate2());
            Assert.Equal("neon-glow-test.jcs-software.co.uk",result.GetNameInfo(X509NameType.SimpleName, false));
            Assert.NotNull(result.PublicKey);
            Assert.NotNull(result.PrivateKey);
        }

        [Fact(DisplayName = "Will throw an exception if an invalid passphrase is specified")]
        [Trait("Test Type", "Unit")]
        [Trait("Target Class", "X509CertificateHelper")]
        public void LoadPfxWithInvalidPassphrase()
        {
            var sshOption = GetHomeSubdirectoryPath(new string[]{".ssh", "neon", "test.pfx"});
            var result = sshOption.Fold(path =>
            {
                X509Certificate2 cert = null;
                Assert.Throws<X509HelperException>(() =>
                {
                    cert= CertificatefromPfxFile(path, () => "testssdfsdf");
                });
                return cert;
            }, () => new X509Certificate2());
        }
        
        public void Dispose()
        {
        }
    }
}