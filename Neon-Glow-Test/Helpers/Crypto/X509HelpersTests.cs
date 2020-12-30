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
    [Trait("Category", "Unit")]
    public class X509HelpersTests : IDisposable
    {

        [Fact(DisplayName = "Can load a valid pfx file with a valid passphrase")]
        [Trait("Category", "Crypto")]
        public void LoadPfxFromFile()
        {
            var sshOption = GetCurrentHomePath(new string[]{".ssh", "neon", "test.pfx"});
            var result = sshOption.Fold(path =>
            {
                var cert = CertificatefromPfxFile(path, () => "test");
                return cert;

            }, () => new X509Certificate2());
            Assert.NotNull(result.PublicKey);
            Assert.NotNull(result.PrivateKey);
        }

        [Fact(DisplayName = "Will throw an exception if an invalid passphrase is specified")]
        [Trait("Category", "Crypto")]
        public void LoadPfxWithInvalidPassphrase()
        {
            var sshOption = GetCurrentHomePath(new string[]{".ssh", "neon", "test.pfx"});
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