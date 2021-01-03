using Xunit;
using static JCS.Neon.Glow.Helpers.Crypto.AesHelpers;

namespace JCS.Neon.Glow.Test.Helpers.Crypto
{
    /// <summary>
    /// Test suite for <see cref="AesHelpers"/>
    /// </summary>
    [Trait("Test Type", "Unit")]
    [Trait("Target Class", "AesHelpers")]
    public class AesHelpersTests
    {
        [Fact(DisplayName = "Must be able to encrypt/decrypt based on wrapped keys and a valid x509 certificate")]
        [Trait("Test Type", "Unit")]
        [Trait("Target Class", "AesHelpers")]
        public void EncryptAndDecryptWithWrapping()
        {
            Assert.True(false);
        }
    }
}