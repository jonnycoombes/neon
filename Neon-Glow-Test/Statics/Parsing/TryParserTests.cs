/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System;
using JCS.Neon.Glow.Statics.Parsing;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

#endregion

namespace JCS.Neon.Glow.Test.Statics.Parsing
{
    /// <summary>
    /// </summary>
    [Trait("Category", "TryParsers")]
    public class DateAndTimeTests : TestBase, IDisposable
    {
        public void Dispose()
        {
        }

        [Theory(DisplayName = "Can parse a series of standard ISO date representations to OffsetDateTime instances")]
        [InlineData("2020-12-31T12:12:00Z")]
        [InlineData("2020-09-13T09:45:12-03")]
        [InlineData("2020-01-03T07:23:07Z")]
        [InlineData("2020-12-31T04:56:22+02")]
        [Trait("Category", "TryParsers")]
        public void CheckOffsetDateTimeParsing(string src)
        {
            Assert.True(!TryParsers.TryParseGeneralIsoOffsetDateTime(src).IsNone);
        }

        [Theory(DisplayName = "Can parse a series of standard ISO date representations to LocalDateTime instances")]
        [InlineData("2020-12-31T12:12:00")]
        [InlineData("2020-09-13T09:45:12")]
        [InlineData("2020-01-03T07:23:07")]
        [InlineData("2020-12-31T04:56:22")]
        [Trait("Category", "TryParsers")]
        public void CheckLocalDatetimeParsing(string src)
        {
            Assert.True(!TryParsers.TryParseGeneralIsoLocalDateTime(src).IsNone);
        }

        [Theory(DisplayName = "Can correctly identify ISO representations of invalid date values")]
        [Trait("Category", "TryParsers")]
        [InlineData("2020-13-31T12:12:00Z")]
        [InlineData("2020-09-43T09:45:12-04")]
        [InlineData("2020-00-03T07:23:07+02")]
        [InlineData("2020-12-31T54:56:22Z")]
        public void CheckInvalidDates(string src)
        {
            Assert.True(TryParsers.TryParseGeneralIsoOffsetDateTime(src).IsNone);
            Assert.True(TryParsers.TryParseGeneralIsoLocalDateTime(src).IsNone);
        }

        [Theory(DisplayName = "Can parse a series of well-formed guids correctly")]
        [InlineData("30dd879c-ee2f-11db-8314-0800200c9a66")]
        [InlineData("(30dd879c-ee2f-11db-8314-0800200c9a66)")]
        [InlineData("{40dd879c-ee2f-11db-8314-0800200c9a66}")]
        [InlineData("697628a5ffda496b810ece922218b291")]
        [InlineData("18a58d2d58d6449d96b1b272fae89e09")]
        [InlineData("7bcdcb92f55c4d2387f831e9571a122f")]
        [Trait("Category", "TryParsers")]
        public void ParseValidGuids(string src)
        {
            Assert.False(TryParsers.TryParseGuid(src).IsNone);
        }

        [Theory(DisplayName = "Can parse a series of invalid guids correctly")]
        [InlineData("30dd676879c-ee2f-11db-8314-0800200c9a66")]
        [InlineData("((30dd879c-ee2f-11db-8314-0800200c9a66)")]
        [InlineData("{40dd879c-ee2f-11db}-8314-0800200c9a66}")]
        [InlineData("697628a5ffda496b810ece922218b29134342343")]
        [InlineData("18a58-d6449d96b1b272fae89e09")]
        [InlineData("7bcdcb!&&&&92f55c4d2387f831e9571a122f")]
        [Trait("Category", "TryParsers")]
        public void ParseInvalidGuids(string src)
        {
            Assert.True(TryParsers.TryParseGuid(src).IsNone);
        }

        public DateAndTimeTests(ITestOutputHelper output) : base(output)
        {
        }
    }
}