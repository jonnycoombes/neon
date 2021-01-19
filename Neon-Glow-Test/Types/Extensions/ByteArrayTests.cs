using System.ComponentModel.DataAnnotations;
using System.Text;
using JCS.Neon.Glow.Types.Extensions;
using Xunit;

namespace JCS.Neon.Glow.Test.Types.Extensions
{
    [Trait("Category", "Extensions")]
    public class ByteArrayTests : TestBase
    {
        [Theory(DisplayName = "Must be able to concatenate to arrays formed from strings")]
        [Trait("Category", "Extensions")]
        [InlineData("testValue 1", "test value 2")]
        [InlineData("asdfa99gasdfa''werw#errr", "some cheesy bits")]
        [InlineData("Some random test with some ][;-09 weird characters", "some other crap goes in here")]
        public void CheckConcatenation(string s, string t)
        {
            var summed = s + t;
            var sb = Encoding.UTF8.GetBytes(s);
            var tb = Encoding.UTF8.GetBytes(t);
            sb = sb.Concatenate(tb);
            var result = Encoding.UTF8.GetString(sb);
            Assert.Equal(Encoding.UTF8.GetString(sb), summed);
        }


        [Theory(DisplayName = "Must be able to chain byte array concatenation")]
        [Trait("Category", "Extensions")]
        [InlineData("testValue 1", "test value 2", "cheese")]
        [InlineData("asdfa99gasdfa''werw#errr", "some cheesy bits", "cheddar")]
        [InlineData("Some random test with some ][;-09 weird characters", "some other crap goes in here", "wotsits")]
        public void CheckChainedConcatenation(string x, string y, string z)
        {
            var summed = x + y + z;
            var sx = Encoding.UTF8.GetBytes(x);
            var sy = Encoding.UTF8.GetBytes(y);
            var sz = Encoding.UTF8.GetBytes(z);
            sx = sx.Concatenate(sy).Concatenate(sz);
            Assert.Equal(Encoding.UTF8.GetString(sx), summed);
        }

        [Theory(DisplayName = "Can head and tail a byte array")]
        [Trait("Category", "Extensions")]
        [InlineData("test")]
        [InlineData("")]
        public void ValidateHeadAndTailOperations(string x)
        {
            var sx = Encoding.ASCII.GetBytes(x);
            var head = sx.Head();
            if (x.Length == 0)
                Assert.Null(head);
            else
                Assert.NotNull(head);
            var tail = sx.Tail();
            if (x.Length == 0)
                Assert.Null(tail);
            else
                Assert.NotNull(tail);
        }
    }
}