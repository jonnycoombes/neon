/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using JCS.Neon.Glow.Statics.Crypto;
using JCS.Neon.Glow.Types.Extensions;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

#endregion

namespace JCS.Neon.Glow.Test.Statics.Crypto
{
    [Trait("Category", "Cryptography")]
    public class EncodingTests : TestBase
    {
        [Theory(DisplayName = "Must be able to encode and decode in Base64 format")]
        [Trait("Category", "Cryptography")]
        [InlineData("testValue 1")]
        [InlineData("asdfa99gasdfa''werw#errr")]
        [InlineData("Some random test with some ][;-09 weird characters")]
        public void PerformBase64Transcoding(string source)
        {
            var encoded = Encodings.Base64Encode(source);
            var decoded = Encodings.Base64Decode(encoded);
            Assert.Equal(source, decoded);
        }

        [Theory(DisplayName = "Must be able to encode and decode in Base64Url format")]
        [Trait("Category", "Cryptography")]
        [InlineData("testValue 1")]
        [InlineData("asdfa99gasdfa''werw#errr")]
        [InlineData("Some random test with some ][;-09 weird characters")]
        [InlineData("http://jcs-software.co.uk/neon-tetra?test&value=1")]
        public void PerformBase64UrlTranscoding(string source)
        {
            var encoded = Encodings.Base64Encode(source);
            var decoded = Encodings.Base64Decode(encoded);
            Assert.Equal(source, decoded);
        }

        [Theory(DisplayName = "Must be able to encode and decode in various encodings")]
        [Trait("Category", "Cryptography")]
        [InlineData("testValue 1", ByteEncoding.Ascii)]
        [InlineData("asdfa99gasdfa''werw#errr", ByteEncoding.Latin1)]
        [InlineData("Some random test ¾òẏỷỨﺫﳲ", ByteEncoding.Unicode)]
        [InlineData("http://jcs-software.co.uk/neon-tetra?test&value=1", ByteEncoding.Utf8)]
        [InlineData("Some further random tests ", ByteEncoding.Utf32)]
        public void PerformByteTranscoding(string source, ByteEncoding encoding)
        {
            var encoded = Encodings.StringToBytes(source, encoding);
            var decoded = Encodings.BytesToString(encoded, encoding);
            Assert.Equal(source, decoded);
        }

        [Theory(DisplayName = "Must be able to encode and decode to Base 64 in various encodings using string extension methods")]
        [Trait("Category", "Cryptography")]
        [InlineData("testValue 1", ByteEncoding.Ascii)]
        [InlineData("asdfa99gasdfa''werw#errr", ByteEncoding.Latin1)]
        [InlineData("Some random test ¾òẏỷỨﺫﳲ", ByteEncoding.Unicode)]
        [InlineData("http://jcs-software.co.uk/neon-tetra?test&value=1", ByteEncoding.Utf8)]
        [InlineData("Some further random tests ", ByteEncoding.Utf32)]
        public void EncodeAndDecodeBase64ViaExtensionMethods(string s, ByteEncoding encoding)
        {
            var original = s;
            var encoded = s.Base64Encode(encoding);
            var decoded = encoded.Base64Decode(encoding);
            Assert.Equal(original, decoded);
        }

        [Theory(DisplayName = "Must be able to encode and decode to base64URl in various encodings using string extension methods")]
        [Trait("Category", "Cryptography")]
        [InlineData("testValue 1", ByteEncoding.Ascii)]
        [InlineData("asdfa99gasdfa''werw#errr", ByteEncoding.Latin1)]
        [InlineData("Some random test ¾òẏỷỨﺫﳲ", ByteEncoding.Unicode)]
        [InlineData("http://jcs-software.co.uk/neon-tetra?test&value=1", ByteEncoding.Utf8)]
        [InlineData("Some further random tests ", ByteEncoding.Utf32)]
        public void EncodeAndDecodeBase64UrlViaExtensionMethods(string s, ByteEncoding encoding)
        {
            var original = s;
            var encoded = s.Base64UrlEncode(encoding);
            var decoded = encoded.Base64UrlDecode(encoding);
            Assert.Equal(original, decoded);
        }

        public EncodingTests(ITestOutputHelper output) : base(output)
        {
        }
    }
}