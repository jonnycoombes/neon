using JCS.Neon.Glow.Helpers.Crypto;
using Xunit;
using static JCS.Neon.Glow.Helpers.Crypto.EncodingHelpers;

namespace JCS.Neon.Glow.Test.Helpers.Crypto
{
    [Trait("Test Type", "Unit")]
    [Trait("Target Class", "EncodingHelpers")]
    public class EncodingHelpersTests
    {
        [Theory(DisplayName = "Must be able to encode and decode in Base64 format")]
        [Trait("Test Type", "Unit")]
        [Trait("Target Class", "EncodingHelpers")]
        [InlineData("testValue 1")]
        [InlineData("asdfa99gasdfa''werw#errr")]
        [InlineData("Some random test with some ][;-09 weird characters")]
        public void PerformBase64Transcoding(string source)
        {
            var encoded = EncodeBase64(source);
            var decoded = DecodeBase64(encoded);
            Assert.Equal(source, decoded);
        }
        
        [Theory(DisplayName = "Must be able to encode and decode in Base64Url format")]
        [Trait("Test Type", "Unit")]
        [Trait("Target Class", "EncodingHelpers")]
        [InlineData("testValue 1")]
        [InlineData("asdfa99gasdfa''werw#errr")]
        [InlineData("Some random test with some ][;-09 weird characters")]
        [InlineData("http://jcs-software.co.uk/neon-tetra?test&value=1")]
        public void PerformBase64UrlTranscoding(string source)
        {
            var encoded = EncodeBase64(source);
            var decoded = DecodeBase64(encoded);
            Assert.Equal(source, decoded);
        }
        
       [Theory(DisplayName = "Must be able to encode and decode in various encodings")]
       [Trait("Test Type", "Unit")]
       [Trait("Target Class", "EncodingHelpers")]
       [InlineData("testValue 1", ByteEncoding.Ascii)]
       [InlineData("asdfa99gasdfa''werw#errr", ByteEncoding.Latin1)]
       [InlineData("Some random test ¾òẏỷỨﺫﳲ", ByteEncoding.Unicode)]
       [InlineData("http://jcs-software.co.uk/neon-tetra?test&value=1", ByteEncoding.Utf8)]
       [InlineData("Some further random tests ", ByteEncoding.Utf32)]
       public void PerformByteTranscoding(string source, ByteEncoding encoding)
       {
           var encoded = StringToBytes(source, encoding);
           var decoded = BytesToString(encoded, encoding);
           Assert.Equal(source, decoded);
       } 
    }
}