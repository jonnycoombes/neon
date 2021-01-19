using System;
using Xunit;
using static JCS.Neon.Glow.Utilities.General.Parsing;

namespace JCS.Neon.Glow.Test.Utilities.General
{
    /// <summary>
    /// Test suite for <see cref="JCS.Neon.Glow.Helpers.ParseHelpers"/>
    /// </summary>
    [Trait("Category", "General")]
    public class ParsingTests : TestBase, IDisposable
    {
        [Theory(DisplayName = "Can parse a series of well-formed guids correctly")]
        [InlineData("30dd879c-ee2f-11db-8314-0800200c9a66")]
        [InlineData("(30dd879c-ee2f-11db-8314-0800200c9a66)")]
        [InlineData("{40dd879c-ee2f-11db-8314-0800200c9a66}")]
        [InlineData("697628a5ffda496b810ece922218b291")]
        [InlineData("18a58d2d58d6449d96b1b272fae89e09")]
        [InlineData("7bcdcb92f55c4d2387f831e9571a122f")]
        [Trait("Category", "General")]
        public void ParseValidGuids(string src)
        {
            Assert.False(ParseGuid(src).IsNone);
        }

        [Theory(DisplayName = "Can parse a series of invalid guids correctly")]
        [InlineData("30dd676879c-ee2f-11db-8314-0800200c9a66")]
        [InlineData("((30dd879c-ee2f-11db-8314-0800200c9a66)")]
        [InlineData("{40dd879c-ee2f-11db}-8314-0800200c9a66}")]
        [InlineData("697628a5ffda496b810ece922218b29134342343")]
        [InlineData("18a58-d6449d96b1b272fae89e09")]
        [InlineData("7bcdcb!&&&&92f55c4d2387f831e9571a122f")]
        [Trait("Category", "General")]
        public void ParseInvalidGuids(string src)
        {
            Assert.True(ParseGuid(src).IsNone);
        }

        public void Dispose()
        {
        }
    }
}