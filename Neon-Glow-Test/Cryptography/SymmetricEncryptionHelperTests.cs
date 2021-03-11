#region

using System.Security.Cryptography;
using System.Text;
using JCS.Neon.Glow.Cryptography;
using Xunit;

#endregion

namespace JCS.Neon.Glow.Test.Cryptography
{
    /// <summary>
    ///     Test suite for <see cref="SymmetricEncryptionHelper" />
    /// </summary>
    [Trait("Category", "Cryptography")]
    public class SymmetricEncryptionHelperTests : TestBase
    {
        [Theory(DisplayName =
            "(AES) Must be able to encrypt/decrypt based on wrapped keys and a valid x509 certificate (public -> private)")]
        [Trait("Category", "Cryptography")]
        [InlineData(128, "this is a test string", SymmetricEncryptionHelper.SymmetricAlgorithmOption.Aes)]
        [InlineData(256, "this is a different test string", SymmetricEncryptionHelper.SymmetricAlgorithmOption.Aes)]
        [InlineData(256, "t", SymmetricEncryptionHelper.SymmetricAlgorithmOption.Aes)]
        public void EncryptAndDecryptWithWrappingSingleBlockAes(int keySize, string source,
            SymmetricEncryptionHelper.SymmetricAlgorithmOption algorithm)
        {
            var cert = LoadTestCertificate();

            // encrypt and wrap the key, IV
            var encryptionResult = SymmetricEncryptionHelper.EncryptAndWrap(Encoding.UTF8.GetBytes(source), cert,
                builder =>
                {
                    builder.SetSymmetricAlgorithm(algorithm);
                    builder.SetCipherMode(CipherMode.CBC);
                    builder.SetKeySize(keySize);
                    builder.SetKeyWrappingOption(SymmetricEncryptionHelper.KeyWrappingOption.WrapWithPublicKey);
                });

            // things shouldn't get smaller when encrypting
            Assert.True(encryptionResult.Right.Length >= source.Length);

            var decryptionResult = SymmetricEncryptionHelper.UnwrapAndDecrypt(encryptionResult, cert,
                builder =>
                {
                    builder.SetCipherMode(CipherMode.CBC);
                    builder.SetKeySize(keySize);
                    builder.SetKeyUnwrappingOption(SymmetricEncryptionHelper.KeyUnwrappingOption.UnwrapWithPrivateKey);
                });

            var decodedResult = Encoding.UTF8.GetString(decryptionResult);
            Assert.Equal(decodedResult, source);
        }

        [Theory(DisplayName =
            "(AES) Must be able to encrypt/decrypt based on wrapped keys and a valid x509 certificate (public -> private)")]
        [Trait("Category", "Cryptography")]
        [InlineData(128, 256, SymmetricEncryptionHelper.SymmetricAlgorithmOption.Aes)]
        [InlineData(128, 1024, SymmetricEncryptionHelper.SymmetricAlgorithmOption.Aes)]
        [InlineData(256, 256, SymmetricEncryptionHelper.SymmetricAlgorithmOption.Aes)]
        [InlineData(256, 19024, SymmetricEncryptionHelper.SymmetricAlgorithmOption.Aes)]
        public void EncryptAndDecryptWithWrappingStreamAes(int keySize, int size,
            SymmetricEncryptionHelper.SymmetricAlgorithmOption algorithm)
        {
            var source = PassphraseHelper.GenerateRandomPassphrase(
                builder => { builder.SetRequiredLength(size); });
            var cert = LoadTestCertificate();

            // encrypt and wrap the key, IV
            var encryptionResult = SymmetricEncryptionHelper.EncryptAndWrap(Encoding.UTF8.GetBytes(source), cert,
                builder =>
                {
                    builder.SetSymmetricAlgorithm(algorithm);
                    builder.SetSymmetricAlgorithm(SymmetricEncryptionHelper.SymmetricAlgorithmOption.Aes);
                    builder.SetCipherMode(CipherMode.CBC);
                    builder.SetKeySize(keySize);
                    builder.SetKeyWrappingOption(SymmetricEncryptionHelper.KeyWrappingOption.WrapWithPublicKey);
                });

            // things shouldn't get smaller when encrypting
            Assert.True(encryptionResult.Right.Length >= source.Length);

            var decryptionResult = SymmetricEncryptionHelper.UnwrapAndDecrypt(encryptionResult, cert,
                builder =>
                {
                    builder.SetCipherMode(CipherMode.CBC);
                    builder.SetKeySize(keySize);
                    builder.SetKeyUnwrappingOption(SymmetricEncryptionHelper.KeyUnwrappingOption.UnwrapWithPrivateKey);
                });

            var decodedResult = Encoding.UTF8.GetString(decryptionResult);
            Assert.Equal(decodedResult, source);
        }

        [Theory(DisplayName =
            "(TripleDes) Must be able to encrypt/decrypt based on wrapped keys and a valid x509 certificate (public -> private)")]
        [Trait("Category", "Cryptography")]
        [InlineData(128, "this is a test string", SymmetricEncryptionHelper.SymmetricAlgorithmOption.TripleDes)]
        [InlineData(128, "this is a different test string", SymmetricEncryptionHelper.SymmetricAlgorithmOption.TripleDes)]
        [InlineData(128, "t", SymmetricEncryptionHelper.SymmetricAlgorithmOption.TripleDes)]
        public void EncryptAndDecryptWithWrappingSingleBlockTripleDes(int keySize, string source,
            SymmetricEncryptionHelper.SymmetricAlgorithmOption algorithm)
        {
            var cert = LoadTestCertificate();

            // encrypt and wrap the key, IV
            var encryptionResult = SymmetricEncryptionHelper.EncryptAndWrap(Encoding.UTF8.GetBytes(source), cert,
                builder =>
                {
                    builder.SetSymmetricAlgorithm(algorithm);
                    builder.SetCipherMode(CipherMode.CBC);
                    builder.SetKeySize(keySize);
                    builder.SetKeyWrappingOption(SymmetricEncryptionHelper.KeyWrappingOption.WrapWithPublicKey);
                });

            // things shouldn't get smaller when encrypting
            Assert.True(encryptionResult.Right.Length >= source.Length);

            var decryptionResult = SymmetricEncryptionHelper.UnwrapAndDecrypt(encryptionResult, cert,
                builder =>
                {
                    builder.SetSymmetricAlgorithm(algorithm);
                    builder.SetCipherMode(CipherMode.CBC);
                    builder.SetKeySize(keySize);
                    builder.SetKeyUnwrappingOption(SymmetricEncryptionHelper.KeyUnwrappingOption.UnwrapWithPrivateKey);
                });

            var decodedResult = Encoding.UTF8.GetString(decryptionResult);
            Assert.Equal(decodedResult, source);
        }

        [Theory(DisplayName =
            "(TripleDes) Must be able to encrypt/decrypt based on wrapped keys and a valid x509 certificate (public -> private)")]
        [Trait("Category", "Cryptography")]
        [InlineData(128, 256, SymmetricEncryptionHelper.SymmetricAlgorithmOption.TripleDes)]
        [InlineData(128, 1024, SymmetricEncryptionHelper.SymmetricAlgorithmOption.TripleDes)]
        [InlineData(128, 19024, SymmetricEncryptionHelper.SymmetricAlgorithmOption.TripleDes)]
        public void EncryptAndDecryptWithWrappingStreamTripleDes(int keySize, int size,
            SymmetricEncryptionHelper.SymmetricAlgorithmOption algorithm)
        {
            var source = PassphraseHelper.GenerateRandomPassphrase(
                builder => { builder.SetRequiredLength(size); });
            var cert = LoadTestCertificate();

            // encrypt and wrap the key, IV
            var encryptionResult = SymmetricEncryptionHelper.EncryptAndWrap(Encoding.UTF8.GetBytes(source), cert,
                builder =>
                {
                    builder.SetSymmetricAlgorithm(algorithm);
                    builder.SetCipherMode(CipherMode.CBC);
                    builder.SetKeySize(keySize);
                    builder.SetKeyWrappingOption(SymmetricEncryptionHelper.KeyWrappingOption.WrapWithPublicKey);
                });

            // things shouldn't get smaller when encrypting
            Assert.True(encryptionResult.Right.Length >= source.Length);

            var decryptionResult = SymmetricEncryptionHelper.UnwrapAndDecrypt(encryptionResult, cert,
                builder =>
                {
                    builder.SetSymmetricAlgorithm(algorithm);
                    builder.SetCipherMode(CipherMode.CBC);
                    builder.SetKeySize(keySize);
                    builder.SetKeyUnwrappingOption(SymmetricEncryptionHelper.KeyUnwrappingOption.UnwrapWithPrivateKey);
                });

            var decodedResult = Encoding.UTF8.GetString(decryptionResult);
            Assert.Equal(decodedResult, source);
        }
    }
}