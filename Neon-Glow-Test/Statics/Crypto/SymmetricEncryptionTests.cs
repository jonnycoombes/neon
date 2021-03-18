#region

using System.Security.Cryptography;
using JCS.Neon.Glow.Statics.Crypto;
using Xunit;
using Encoding = System.Text.Encoding;

#endregion

namespace JCS.Neon.Glow.Test.Statics.Crypto
{
    /// <summary>
    ///     Test suite for <see cref="SymmetricEncryption" />
    /// </summary>
    [Trait("Category", "Cryptography")]
    public class SymmetricEncryptionTests : TestBase
    {
        [Theory(DisplayName =
            "(AES) Must be able to encrypt/decrypt based on wrapped keys and a valid x509 certificate (public -> private)")]
        [Trait("Category", "Cryptography")]
        [InlineData(128, "this is a test string", SymmetricEncryption.SymmetricAlgorithmOption.Aes)]
        [InlineData(256, "this is a different test string", SymmetricEncryption.SymmetricAlgorithmOption.Aes)]
        [InlineData(256, "t", SymmetricEncryption.SymmetricAlgorithmOption.Aes)]
        public void EncryptAndDecryptWithWrappingSingleBlockAes(int keySize, string source,
            SymmetricEncryption.SymmetricAlgorithmOption algorithm)
        {
            var cert = LoadTestCertificate();

            // encrypt and wrap the key, IV
            var encryptionResult = SymmetricEncryption.EncryptAndWrap(Encoding.UTF8.GetBytes(source), cert,
                builder =>
                {
                    builder.SetSymmetricAlgorithm(algorithm);
                    builder.SetCipherMode(CipherMode.CBC);
                    builder.SetKeySize(keySize);
                    builder.SetKeyWrappingOption(SymmetricEncryption.KeyWrappingOption.WrapWithPublicKey);
                });

            // things shouldn't get smaller when encrypting
            Assert.True(encryptionResult.Right.Length >= source.Length);

            var decryptionResult = SymmetricEncryption.UnwrapAndDecrypt(encryptionResult, cert,
                builder =>
                {
                    builder.SetCipherMode(CipherMode.CBC);
                    builder.SetKeySize(keySize);
                    builder.SetKeyUnwrappingOption(SymmetricEncryption.KeyUnwrappingOption.UnwrapWithPrivateKey);
                });

            var decodedResult = Encoding.UTF8.GetString(decryptionResult);
            Assert.Equal(decodedResult, source);
        }

        [Theory(DisplayName =
            "(AES) Must be able to encrypt/decrypt based on wrapped keys and a valid x509 certificate (public -> private)")]
        [Trait("Category", "Cryptography")]
        [InlineData(128, 256, SymmetricEncryption.SymmetricAlgorithmOption.Aes)]
        [InlineData(128, 1024, SymmetricEncryption.SymmetricAlgorithmOption.Aes)]
        [InlineData(256, 256, SymmetricEncryption.SymmetricAlgorithmOption.Aes)]
        [InlineData(256, 19024, SymmetricEncryption.SymmetricAlgorithmOption.Aes)]
        public void EncryptAndDecryptWithWrappingStreamAes(int keySize, int size,
            SymmetricEncryption.SymmetricAlgorithmOption algorithm)
        {
            var source = Passphrase.GenerateRandomPassphrase(
                builder => { builder.SetRequiredLength(size); });
            var cert = LoadTestCertificate();

            // encrypt and wrap the key, IV
            var encryptionResult = SymmetricEncryption.EncryptAndWrap(Encoding.UTF8.GetBytes(source), cert,
                builder =>
                {
                    builder.SetSymmetricAlgorithm(algorithm);
                    builder.SetSymmetricAlgorithm(SymmetricEncryption.SymmetricAlgorithmOption.Aes);
                    builder.SetCipherMode(CipherMode.CBC);
                    builder.SetKeySize(keySize);
                    builder.SetKeyWrappingOption(SymmetricEncryption.KeyWrappingOption.WrapWithPublicKey);
                });

            // things shouldn't get smaller when encrypting
            Assert.True(encryptionResult.Right.Length >= source.Length);

            var decryptionResult = SymmetricEncryption.UnwrapAndDecrypt(encryptionResult, cert,
                builder =>
                {
                    builder.SetCipherMode(CipherMode.CBC);
                    builder.SetKeySize(keySize);
                    builder.SetKeyUnwrappingOption(SymmetricEncryption.KeyUnwrappingOption.UnwrapWithPrivateKey);
                });

            var decodedResult = Encoding.UTF8.GetString(decryptionResult);
            Assert.Equal(decodedResult, source);
        }

        [Theory(DisplayName =
            "(TripleDes) Must be able to encrypt/decrypt based on wrapped keys and a valid x509 certificate (public -> private)")]
        [Trait("Category", "Cryptography")]
        [InlineData(128, "this is a test string", SymmetricEncryption.SymmetricAlgorithmOption.TripleDes)]
        [InlineData(128, "this is a different test string", SymmetricEncryption.SymmetricAlgorithmOption.TripleDes)]
        [InlineData(128, "t", SymmetricEncryption.SymmetricAlgorithmOption.TripleDes)]
        public void EncryptAndDecryptWithWrappingSingleBlockTripleDes(int keySize, string source,
            SymmetricEncryption.SymmetricAlgorithmOption algorithm)
        {
            var cert = LoadTestCertificate();

            // encrypt and wrap the key, IV
            var encryptionResult = SymmetricEncryption.EncryptAndWrap(Encoding.UTF8.GetBytes(source), cert,
                builder =>
                {
                    builder.SetSymmetricAlgorithm(algorithm);
                    builder.SetCipherMode(CipherMode.CBC);
                    builder.SetKeySize(keySize);
                    builder.SetKeyWrappingOption(SymmetricEncryption.KeyWrappingOption.WrapWithPublicKey);
                });

            // things shouldn't get smaller when encrypting
            Assert.True(encryptionResult.Right.Length >= source.Length);

            var decryptionResult = SymmetricEncryption.UnwrapAndDecrypt(encryptionResult, cert,
                builder =>
                {
                    builder.SetSymmetricAlgorithm(algorithm);
                    builder.SetCipherMode(CipherMode.CBC);
                    builder.SetKeySize(keySize);
                    builder.SetKeyUnwrappingOption(SymmetricEncryption.KeyUnwrappingOption.UnwrapWithPrivateKey);
                });

            var decodedResult = Encoding.UTF8.GetString(decryptionResult);
            Assert.Equal(decodedResult, source);
        }

        [Theory(DisplayName =
            "(TripleDes) Must be able to encrypt/decrypt based on wrapped keys and a valid x509 certificate (public -> private)")]
        [Trait("Category", "Cryptography")]
        [InlineData(128, 256, SymmetricEncryption.SymmetricAlgorithmOption.TripleDes)]
        [InlineData(128, 1024, SymmetricEncryption.SymmetricAlgorithmOption.TripleDes)]
        [InlineData(128, 19024, SymmetricEncryption.SymmetricAlgorithmOption.TripleDes)]
        public void EncryptAndDecryptWithWrappingStreamTripleDes(int keySize, int size,
            SymmetricEncryption.SymmetricAlgorithmOption algorithm)
        {
            var source = Passphrase.GenerateRandomPassphrase(
                builder => { builder.SetRequiredLength(size); });
            var cert = LoadTestCertificate();

            // encrypt and wrap the key, IV
            var encryptionResult = SymmetricEncryption.EncryptAndWrap(Encoding.UTF8.GetBytes(source), cert,
                builder =>
                {
                    builder.SetSymmetricAlgorithm(algorithm);
                    builder.SetCipherMode(CipherMode.CBC);
                    builder.SetKeySize(keySize);
                    builder.SetKeyWrappingOption(SymmetricEncryption.KeyWrappingOption.WrapWithPublicKey);
                });

            // things shouldn't get smaller when encrypting
            Assert.True(encryptionResult.Right.Length >= source.Length);

            var decryptionResult = SymmetricEncryption.UnwrapAndDecrypt(encryptionResult, cert,
                builder =>
                {
                    builder.SetSymmetricAlgorithm(algorithm);
                    builder.SetCipherMode(CipherMode.CBC);
                    builder.SetKeySize(keySize);
                    builder.SetKeyUnwrappingOption(SymmetricEncryption.KeyUnwrappingOption.UnwrapWithPrivateKey);
                });

            var decodedResult = Encoding.UTF8.GetString(decryptionResult);
            Assert.Equal(decodedResult, source);
        }
    }
}