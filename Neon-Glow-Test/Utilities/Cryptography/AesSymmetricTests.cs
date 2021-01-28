﻿#region

using System.Security.Cryptography;
using JCS.Neon.Glow.Utilities.Cryptography;
using Xunit;
using Encoding = System.Text.Encoding;

#endregion

namespace JCS.Neon.Glow.Test.Utilities.Cryptography
{
    /// <summary>
    ///     Test suite for <see cref="AesSymmetric" />
    /// </summary>
    [Trait("Category", "Crypto")]
    public class AesSymmetricTests : TestBase
    {
        [Theory(DisplayName = "Must be able to encrypt/decrypt based on wrapped keys and a valid x509 certificate (public -> private)")]
        [Trait("Category", "Crypto")]
        [InlineData(128, "this is a test string")]
        [InlineData(256, "this is a different test string")]
        [InlineData(256, "t")]
        public void EncryptAndDecryptWithWrappingSingleBlock(int keySize, string source)
        {
            var cert = LoadTestCertificate();

            // encrypt and wrap the key, IV
            var encryptionResult = AesSymmetric.EncryptAndWrap(Encoding.UTF8.GetBytes(source), cert,
                builder =>
                {
                    builder.SetCipherMode(CipherMode.CBC);
                    builder.SetKeySize(keySize);
                    builder.SetKeyWrappingOption(AesSymmetric.AesSymmetricKeyWrappingOption.WrapWithPublicKey);
                });

            // things shouldn't get smaller when encrypting
            Assert.True(encryptionResult.Right.Length >= source.Length);

            var decryptionResult = AesSymmetric.UnwrapAndDecrypt(encryptionResult, cert,
                builder =>
                {
                    builder.SetCipherMode(CipherMode.CBC);
                    builder.SetKeySize(keySize);
                    builder.SetKeyUnwrappingOption(AesSymmetric.AesSymmetricKeyUnwrappingOption.UnwrapWithPrivateKey);
                });

            var decodedResult = Encoding.UTF8.GetString(decryptionResult);
            Assert.Equal(decodedResult, source);
        }

        [Theory(DisplayName = "Must be able to encrypt/decrypt based on wrapped keys and a valid x509 certificate (public -> private)")]
        [Trait("Category", "Crypto")]
        [InlineData(128, 256)]
        [InlineData(128, 1024)]
        [InlineData(256, 256)]
        [InlineData(256, 19024)]
        public void EncryptAndDecryptWithWrappingStream(int keySize, int size)
        {
            var source = Passphrases.GenerateRandomPassphrase(
                builder => { builder.SetRequiredLength(size); });
            var cert = LoadTestCertificate();

            // encrypt and wrap the key, IV
            var encryptionResult = AesSymmetric.EncryptAndWrap(Encoding.UTF8.GetBytes(source), cert,
                builder =>
                {
                    builder.SetCipherMode(CipherMode.CBC);
                    builder.SetKeySize(keySize);
                    builder.SetKeyWrappingOption(AesSymmetric.AesSymmetricKeyWrappingOption.WrapWithPublicKey);
                });

            // things shouldn't get smaller when encrypting
            Assert.True(encryptionResult.Right.Length >= source.Length);

            var decryptionResult = AesSymmetric.UnwrapAndDecrypt(encryptionResult, cert,
                builder =>
                {
                    builder.SetCipherMode(CipherMode.CBC);
                    builder.SetKeySize(keySize);
                    builder.SetKeyUnwrappingOption(AesSymmetric.AesSymmetricKeyUnwrappingOption.UnwrapWithPrivateKey);
                });

            var decodedResult = Encoding.UTF8.GetString(decryptionResult);
            Assert.Equal(decodedResult, source);
        }
    }
}