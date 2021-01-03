using System.Security.Cryptography.X509Certificates;
using JCS.Neon.Glow.Helpers.Crypto;
using JCS.Neon.Glow.Types;
using Xunit;
using static JCS.Neon.Glow.Helpers.Crypto.AesHelpers;
using static JCS.Neon.Glow.Helpers.Crypto.X509Helpers;
using static JCS.Neon.Glow.Helpers.General.FileHelpers;

namespace JCS.Neon.Glow.Test.Helpers.Crypto
{
    /// <summary>
    /// Test suite for <see cref="AesHelpers"/>
    /// </summary>
    [Trait("Test Type", "Unit")]
    [Trait("Target Class", "AesHelpers")]
    public class AesHelpersTests
    {
        /// <summary>
        /// Just loads a test certificate for use during tests
        /// </summary>
        /// <returns></returns>
        private X509Certificate2 LoadCertificate()
        {
            var sshOption = GetHomeSubdirectoryPath(new string[] {".config", "neon", "glow", "test.pfx"});
            var result = sshOption.Fold(path =>
            {
                var cert = CertificatefromPfxFile(path, () => "test");
                return cert;
            }, () => new X509Certificate2());
            return result;
        }

        [Fact(DisplayName = "Must be able to encrypt/decrypt based on wrapped keys and a valid x509 certificate")]
        [Trait("Test Type", "Unit")]
        [Trait("Target Class", "AesHelpers")]
        public void EncryptAndDecryptWithWrapping()
        {
            var cert = LoadCertificate();
            Assert.True(false);
        }
    }
}