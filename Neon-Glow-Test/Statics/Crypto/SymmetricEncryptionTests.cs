/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System.Security.Cryptography;
using JCS.Neon.Glow.Statics.Crypto;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;
using Encoding = System.Text.Encoding;

#endregion

namespace JCS.Neon.Glow.Test.Statics.Crypto
{
    /// <summary>
    ///     Test suite for <see cref="SymmetricEncryption" />
    /// </summary>
    [Trait("Category", "Cryptography")]
    public class SymmetricEncryptionTests : TestBase, IClassFixture<Fixtures>
    {
        /// <summary>
        /// The test fixtures
        /// </summary>
        protected Fixtures Fixtures { get; set; }

        [Theory(DisplayName =
            "(AES) Must be able to encrypt/decrypt based on wrapped keys and a valid x509 certificate (public -> private)")]
        [Trait("Category", "Cryptography")]
        [InlineData(128, "this is a test string", SymmetricAlgorithmOption.Aes)]
        [InlineData(256, "this is a different test string", SymmetricAlgorithmOption.Aes)]
        [InlineData(256, "t", SymmetricAlgorithmOption.Aes)]
        public void EncryptAndDecryptWithWrappingSingleBlockAes(int keySize, string source,
            SymmetricAlgorithmOption algorithm)
        {
            var cert = Fixtures.Certificate; 

            // encrypt and wrap the key, IV
            var encryptionResult = SymmetricEncryption.EncryptAndWrap(Encoding.UTF8.GetBytes(source), cert,
                builder =>
                {
                    builder.SymmetricAlgorithm(algorithm);
                    builder.CipherMode(CipherMode.CBC);
                    builder.KeySize(keySize);
                    builder.KeyWrappingOption(KeyWrappingOption.WrapWithPublicKey);
                });

            // things shouldn't get smaller when encrypting
            Assert.True(encryptionResult.Right.Length >= source.Length);

            var decryptionResult = SymmetricEncryption.UnwrapAndDecrypt(encryptionResult, cert,
                builder =>
                {
                    builder.CipherMode(CipherMode.CBC);
                    builder.KeySize(keySize);
                    builder.KeyUnwrappingOption(KeyUnwrappingOption.UnwrapWithPrivateKey);
                });

            var decodedResult = Encoding.UTF8.GetString(decryptionResult);
            Assert.Equal(decodedResult, source);
        }

        [Theory(DisplayName =
            "(AES) Must be able to encrypt/decrypt based on wrapped keys and a valid x509 certificate (public -> private)")]
        [Trait("Category", "Cryptography")]
        [InlineData(128, 256, SymmetricAlgorithmOption.Aes)]
        [InlineData(128, 1024, SymmetricAlgorithmOption.Aes)]
        [InlineData(256, 256, SymmetricAlgorithmOption.Aes)]
        [InlineData(256, 19024, SymmetricAlgorithmOption.Aes)]
        public void EncryptAndDecryptWithWrappingStreamAes(int keySize, int size,
            SymmetricAlgorithmOption algorithm)
        {
            var source = Passphrase.GenerateRandomPassphrase(
                builder => { builder.RequiredLength(size); });
            var cert = Fixtures.Certificate; 

            // encrypt and wrap the key, IV
            var encryptionResult = SymmetricEncryption.EncryptAndWrap(Encoding.UTF8.GetBytes(source), cert,
                builder =>
                {
                    builder.SymmetricAlgorithm(algorithm);
                    builder.SymmetricAlgorithm(SymmetricAlgorithmOption.Aes);
                    builder.CipherMode(CipherMode.CBC);
                    builder.KeySize(keySize);
                    builder.KeyWrappingOption(KeyWrappingOption.WrapWithPublicKey);
                });

            // things shouldn't get smaller when encrypting
            Assert.True(encryptionResult.Right.Length >= source.Length);

            var decryptionResult = SymmetricEncryption.UnwrapAndDecrypt(encryptionResult, cert,
                builder =>
                {
                    builder.CipherMode(CipherMode.CBC);
                    builder.KeySize(keySize);
                    builder.KeyUnwrappingOption(KeyUnwrappingOption.UnwrapWithPrivateKey);
                });

            var decodedResult = Encoding.UTF8.GetString(decryptionResult);
            Assert.Equal(decodedResult, source);
        }

        [Theory(DisplayName =
            "(TripleDes) Must be able to encrypt/decrypt based on wrapped keys and a valid x509 certificate (public -> private)")]
        [Trait("Category", "Cryptography")]
        [InlineData(128, "this is a test string", SymmetricAlgorithmOption.TripleDes)]
        [InlineData(128, "this is a different test string", SymmetricAlgorithmOption.TripleDes)]
        [InlineData(128, "t", SymmetricAlgorithmOption.TripleDes)]
        public void EncryptAndDecryptWithWrappingSingleBlockTripleDes(int keySize, string source,
            SymmetricAlgorithmOption algorithm)
        {
            var cert = Fixtures.Certificate; 

            // encrypt and wrap the key, IV
            var encryptionResult = SymmetricEncryption.EncryptAndWrap(Encoding.UTF8.GetBytes(source), cert,
                builder =>
                {
                    builder.SymmetricAlgorithm(algorithm);
                    builder.CipherMode(CipherMode.CBC);
                    builder.KeySize(keySize);
                    builder.KeyWrappingOption(KeyWrappingOption.WrapWithPublicKey);
                });

            // things shouldn't get smaller when encrypting
            Assert.True(encryptionResult.Right.Length >= source.Length);

            var decryptionResult = SymmetricEncryption.UnwrapAndDecrypt(encryptionResult, cert,
                builder =>
                {
                    builder.SymmetricAlgorithm(algorithm);
                    builder.CipherMode(CipherMode.CBC);
                    builder.KeySize(keySize);
                    builder.KeyUnwrappingOption(KeyUnwrappingOption.UnwrapWithPrivateKey);
                });

            var decodedResult = Encoding.UTF8.GetString(decryptionResult);
            Assert.Equal(decodedResult, source);
        }

        [Theory(DisplayName =
            "(TripleDes) Must be able to encrypt/decrypt based on wrapped keys and a valid x509 certificate (public -> private)")]
        [Trait("Category", "Cryptography")]
        [InlineData(128, 256, SymmetricAlgorithmOption.TripleDes)]
        [InlineData(128, 1024, SymmetricAlgorithmOption.TripleDes)]
        [InlineData(128, 19024, SymmetricAlgorithmOption.TripleDes)]
        public void EncryptAndDecryptWithWrappingStreamTripleDes(int keySize, int size,
            SymmetricAlgorithmOption algorithm)
        {
            var source = Passphrase.GenerateRandomPassphrase(
                builder => { builder.RequiredLength(size); });
            var cert = Fixtures.Certificate; 

            // encrypt and wrap the key, IV
            var encryptionResult = SymmetricEncryption.EncryptAndWrap(Encoding.UTF8.GetBytes(source), cert,
                builder =>
                {
                    builder.SymmetricAlgorithm(algorithm);
                    builder.CipherMode(CipherMode.CBC);
                    builder.KeySize(keySize);
                    builder.KeyWrappingOption(KeyWrappingOption.WrapWithPublicKey);
                });

            // things shouldn't get smaller when encrypting
            Assert.True(encryptionResult.Right.Length >= source.Length);

            var decryptionResult = SymmetricEncryption.UnwrapAndDecrypt(encryptionResult, cert,
                builder =>
                {
                    builder.SymmetricAlgorithm(algorithm);
                    builder.CipherMode(CipherMode.CBC);
                    builder.KeySize(keySize);
                    builder.KeyUnwrappingOption(KeyUnwrappingOption.UnwrapWithPrivateKey);
                });

            var decodedResult = Encoding.UTF8.GetString(decryptionResult);
            Assert.Equal(decodedResult, source);
        }

        public SymmetricEncryptionTests(ITestOutputHelper output, Fixtures fixtures) : base(output)
        {
            Fixtures = fixtures;
        }
    }
}