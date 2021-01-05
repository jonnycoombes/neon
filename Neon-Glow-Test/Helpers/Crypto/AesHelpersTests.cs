using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using JCS.Neon.Glow.Helpers.Crypto;
using JCS.Neon.Glow.Types;
using Xunit;
using static JCS.Neon.Glow.Helpers.Crypto.AesHelpers;
using static JCS.Neon.Glow.Helpers.Crypto.X509Helpers;
using static JCS.Neon.Glow.Helpers.General.FileHelpers;
using static JCS.Neon.Glow.Helpers.Crypto.PassphraseHelpers;

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

        [Theory(DisplayName = "Must be able to encrypt/decrypt based on wrapped keys and a valid x509 certificate (public -> private)")]
        [Trait("Test Type", "Unit")]
        [Trait("Target Class", "AesHelpers")]
        [InlineData(128, "this is a test string")]
        [InlineData(256, "this is a different test string")]
        [InlineData(256, "t")]
        public void EncryptAndDecryptWithWrappingSingleBlock(int keySize, string source)
        {
            var cert = LoadCertificate();

            // encrypt and wrap the key, IV
            var encryptionResult = EncryptAndWrap(Encoding.UTF8.GetBytes(source), cert,
                builder =>
                {
                    builder.SetCipherMode(CipherMode.CBC);
                    builder.SetKeySize(keySize);
                    builder.SetKeyWrappingOption(AesKeyWrappingOption.WrapWithPublicKey);
                });
            
            // things shouldn't get smaller when encrypting
            Assert.True(encryptionResult.Right.Length >= source.Length);
            
            var decryptionResult = UnwrapAndDecrypt(encryptionResult, cert,
                builder =>
                {
                    builder.SetCipherMode(CipherMode.CBC);
                    builder.SetKeySize(keySize);
                    builder.SetKeyUnwrappingOption(AesKeyUnwrappingOption.UnwrapWithPrivateKey);
                });
            
            var decodedResult = Encoding.UTF8.GetString(decryptionResult);
            Assert.Equal(decodedResult, source);
        }

        [Theory(DisplayName = "Must be able to encrypt/decrypt based on wrapped keys and a valid x509 certificate (public -> private)")]
        [Trait("Test Type", "Unit")]
        [Trait("Target Class", "AesHelpers")]
        [InlineData(128, 256)]
        [InlineData(128, 1024)]
        [InlineData(256, 256)]
        [InlineData(256, 19024)]
        public void EncryptAndDecryptWithWrappingStream(int keySize, int size)
        {
            var source = GenerateRandomPassphrase(
                builder =>
            {
                builder.SetRequiredLength(size);
            });
            var cert = LoadCertificate();

            // encrypt and wrap the key, IV
            var encryptionResult = EncryptAndWrap(Encoding.UTF8.GetBytes(source), cert, 
                builder =>
                {
                    builder.SetCipherMode(CipherMode.CBC);
                    builder.SetKeySize(keySize);
                    builder.SetKeyWrappingOption(AesKeyWrappingOption.WrapWithPublicKey);
                });

            // things shouldn't get smaller when encrypting
            Assert.True(encryptionResult.Right.Length >= source.Length);
            
            var decryptionResult = UnwrapAndDecrypt(encryptionResult, cert, 
                builder =>
                {
                    builder.SetCipherMode(CipherMode.CBC);
                    builder.SetKeySize(keySize);
                    builder.SetKeyUnwrappingOption(AesKeyUnwrappingOption.UnwrapWithPrivateKey);
                });
            
            var decodedResult = Encoding.UTF8.GetString(decryptionResult);
            Assert.Equal(decodedResult, source);
            
        }
    }
}