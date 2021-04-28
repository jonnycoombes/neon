/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System.Collections.Generic;
using JCS.Neon.Glow.Statics.Crypto;
using Xunit;

#endregion

namespace JCS.Neon.Glow.Test.Statics.Crypto
{
    /// <summary>
    ///     Test suite for <see cref="Passphrase" />
    /// </summary>
    [Trait("Category", "Cryptography")]
    public class PassphraseTests : TestBase
    {
        [Theory(DisplayName = "Must be able to create a random password of a random length")]
        [Trait("Category", "Crypto")]
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
            var options = new PassphraseGenerationOptions {RequiredLength = length};
            if (length >= PassphraseGenerationOptions.MinimumPasswordLength)
            {
                var passphrase = Passphrase.GenerateRandomPassphrase(builder => { builder.SetRequiredLength(length); });
                Assert.Equal(length, passphrase.Length);
            }
            else
            {
                Assert.Throws<PassphraseException>(() => Passphrase.GenerateRandomPassphrase(
                    builder => { builder.SetRequiredLength(length); }));
            }
        }

        [Theory(DisplayName = "Passwords must be unique across multiple calls")]
        [Trait("Category", "Cryptography")]
        [InlineData(10000, 10)]
        [InlineData(10000, 11)]
        [InlineData(10000, 256)]
        [InlineData(10000, 267)]
        [InlineData(10000, 1032)]
        [InlineData(10000, 16)]
        [InlineData(10000, 64)]
        [InlineData(10000, 128)]
        public void CheckUniqueness(int sampleCount, int passphraseLength)
        {
            var cache = new Dictionary<string, string>();
            for (var i = 0; i < sampleCount; i++)
            {
                var passphrase = Passphrase.GenerateRandomPassphrase(
                    builder => { builder.SetRequiredLength(passphraseLength); });
                Assert.DoesNotContain(cache.Keys, k => k.Equals(passphrase));
                cache.Add(passphrase, null);
            }
        }
    }
}