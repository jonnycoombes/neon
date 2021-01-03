using System.Collections.Generic;
using JCS.Neon.Glow.Helpers.Crypto;
using static JCS.Neon.Glow.Helpers.Crypto.PassphraseHelpers;
using Xunit;

namespace JCS.Neon.Glow.Test.Helpers.Crypto
{
    /// <summary>
    /// Test suite for <see cref="PassphraseHelpers"/>
    /// </summary>
    [Trait("Test Type", "Unit")]
    [Trait("Target Class", "PasswordHelpers")]
    public class PasswordHelpersTests
    {

        [Theory(DisplayName = "Must be able to create a random password of a random length")]
        [Trait("Test Type", "Unit")]
        [Trait("Target Class", "PassphraseHelpers")]
        [InlineData(8)]
        [InlineData(12)]
        [InlineData(24)]
        [InlineData(17)]
        [InlineData(256)]
        [InlineData(32)]
        [InlineData(6)]
        [InlineData(-2)]
        public void CreateRandomPassphrase(int length)
        {
            var options = new PassphraseGenerationOptions() {RequiredLength = length};
            if (length >= PassphraseGenerationOptions.MinimumPasswordLength)
            {
                var passphrase = GenerateRandomPassphrase(options);
                Assert.Equal(length, passphrase.Length);
            }
            else
            {
                Assert.Throws<PassphraseHelperException>(() => GenerateRandomPassphrase(options));
            }
        }

        [Theory(DisplayName = "Passwords must be unique across multiple calls")]
        [Trait("Test Type", "Unit")]
        [Trait("Target Class", "PassphraseHelpers")]
        [InlineData(100000,10)]
        [InlineData(25000,256)]
        public void CheckUniqueness(int sampleCount, int passphraseLength)
        {
            var cache = new Dictionary<string, string>();
            for (var i = 0; i < sampleCount; i++)
            {
                var passphrase = GenerateRandomPassphrase(new PassphraseGenerationOptions() {RequiredLength = passphraseLength});
                Assert.DoesNotContain(cache.Keys, k => k.Equals(passphrase));
                cache.Add(passphrase, passphrase);
            }
        }
        
    }
}