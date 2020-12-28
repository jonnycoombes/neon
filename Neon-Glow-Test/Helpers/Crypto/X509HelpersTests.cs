using System;
using Xunit;
using static JCS.Neon.Glow.Helpers.Crypto.X509Helpers;

namespace JCS.Neon.Glow.Test.Helpers.Crypto
{
    /// <summary>
    /// Test suite for <see cref="JCS.Neon.Glow.Helpers.Crypto.X509Helpers"/>
    /// </summary>
    public class X509HelpersTests : IDisposable
    {

        [Fact(DisplayName = "Can load a valid pfx file with a valid passphrase")]
        public void LoadPfxFromFile()
        {
            var cert = CertificatefromPfxFile("/Users/jcoombes/Documents/Source/neon/Neon-Glow/Neon-Glow-Test/Crypto/test.pfx", () => { return "test"; });
            Assert.NotNull(cert.PublicKey);
            Assert.True(cert.HasPrivateKey);
        }

        [Fact(DisplayName = "Will throw an exception if an invalid passphrase is specified")]
        public void LoadPfxWithInvalidPassphrase()
        {
            Assert.Throws<X509HelperException>(() =>
            {
                return CertificatefromPfxFile("/Users/jcoombes/Documents/Source/neon/Neon-Glow/Neon-Glow-Test/Crypto/test.pfx",
                    () => { return "testicle"; });
            });
        }
        
        public void Dispose()
        {
        }
    }
}