#region

using System.Linq;
using JCS.Neon.Glow.Cryptography;
using Xunit;

#endregion

namespace JCS.Neon.Glow.Test.Cryptography
{
    /// <summary>
    ///     Test suite for <see cref="Rng" />
    /// </summary>
    [Trait("Category", "Cryptography")]
    public class RngTests : TestBase
    {
        [Theory(DisplayName = "Must be able to generate sequences of bytes ")]
        [InlineData(256)]
        [InlineData(1024)]
        [InlineData(1)]
        [InlineData(0)]
        [InlineData(25000)]
        public void CheckBoundedByteSequences(uint length)
        {
            var iterator = Rng.BoundedSequence(length);
            var bytes = iterator as byte[] ?? iterator.ToArray();
            Assert.Equal((int) length, bytes.Count());
        }

        [Theory(DisplayName = "Must be able to generate sequences of random integers")]
        [InlineData(256, 0, 124)]
        [InlineData(1024, -100, 1000)]
        [InlineData(1, -6, 6)]
        [InlineData(0, 1, 2)]
        [InlineData(25000, 123, 67894)]
        public void CheckBoundedIntegerSequences(uint length, int min, int max)
        {
            var iterator = Rng.BoundedSequence(length, min, max);
            var ints = iterator as int[] ?? iterator.ToArray();
            Assert.Equal((int) length, ints.Count());
            if (length > 0)
            {
                Assert.InRange(ints.First(), min, max);
            }
        }

        [Theory(DisplayName = "Must be able to generate sequences of random doubles")]
        [InlineData(256, 1.5)]
        [InlineData(1024, 56.45)]
        [InlineData(1, -56.2)]
        [InlineData(0, 123.4)]
        [InlineData(25000, 22)]
        public void CheckBoundedDoubleSequences(uint length, double scale)
        {
            var iterator = Rng.BoundedSequence(length, scale);
            var doubles = iterator as double[] ?? iterator.ToArray();
            Assert.Equal((int) length, doubles.Count());
            if (length > 0)
            {
                if (scale > 0)
                {
                    Assert.InRange(doubles.First(), 0, scale);
                }
                else
                {
                    Assert.InRange(doubles.First(), scale, 0);
                }
            }
        }
    }
}