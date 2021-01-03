using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using JCS.Neon.Glow.Helpers.General;
using JCS.Neon.Glow.Types;
using NodaTime;
using NodaTime.Extensions;
using Xunit;
using static JCS.Neon.Glow.Helpers.General.NodaTimeHelpers;

namespace JCS.Neon.Glow.Test.Helpers.General
{
    /// <summary>
    /// Test suite for <see cref="JCS.Neon.Glow.Helpers.NodaTimeHelpers"/>
    /// </summary>
    [Trait("Test Type", "Unit")]
    [Trait("Target Class", "NodeTimeHelpers")]
    public class NodaTimeHelpersTests : IDisposable
    {
        [Theory(DisplayName = "Can parse a series of standard ISO date representations to OffsetDateTime instances")]
        [InlineData("2020-12-31T12:12:00Z")]
        [InlineData("2020-09-13T09:45:12-03")]
        [InlineData("2020-01-03T07:23:07Z")]
        [InlineData("2020-12-31T04:56:22+02")]
        [Trait("Test Type", "Unit")]
        [Trait("Target Class", "NodeTimeHelpers")]
        public void CheckOffsetDateTimeParsing(string src)
        {
            Assert.True(!ParseHelpers.ParseGeneralIsoOffsetDateTime(src).IsNone);
        }

        [Theory(DisplayName = "Can parse a series of standard ISO date representations to LocalDateTime instances")]
        [InlineData("2020-12-31T12:12:00")]
        [InlineData("2020-09-13T09:45:12")]
        [InlineData("2020-01-03T07:23:07")]
        [InlineData("2020-12-31T04:56:22")]
        [Trait("Test Type", "Unit")]
        [Trait("Target Class", "NodeTimeHelpers")]
        public void CheckLocalDatetimeParsing(string src)
        {
            Assert.True(!ParseHelpers.ParseGeneralIsoLocalDateTime(src).IsNone);
        }

        [Theory(DisplayName = "Can correctly identify ISO representations of invalid date values")]
        [InlineData("2020-13-31T12:12:00Z")]
        [InlineData("2020-09-43T09:45:12-04")]
        [InlineData("2020-00-03T07:23:07+02")]
        [InlineData("2020-12-31T54:56:22Z")]
        [Trait("Test Type", "Unit")]
        [Trait("Target Class", "NodeTimeHelpers")]
        public void CheckInvalidDates(string src)
        {
            Assert.True(ParseHelpers.ParseGeneralIsoOffsetDateTime(src).IsNone);
            Assert.True(ParseHelpers.ParseGeneralIsoLocalDateTime(src).IsNone);
        }

        [Fact(DisplayName = "Can move between string and internal Instant representation through OffsetDateTime")]
        [Trait("Test Type", "Unit")]
        [Trait("Target Class", "NodeTimeHelpers")]
        public void OffsetDateTimeStringSerialisation()
        {
            var instant = Instant.FromDateTimeUtc(DateTime.Now.ToUniversalTime());
            var offsetDateTime = instant.InUtc().ToOffsetDateTime();
            var stringOption = ToGeneralIsoString(offsetDateTime);
            Assert.True(!stringOption.IsNone);
            var rep = stringOption.GetOrElse(() => null);
            var parsed = ParseHelpers.ParseGeneralIsoOffsetDateTime(rep).Fold(
                time => time,
                () => new OffsetDateTime());
            Assert.True(parsed.Date.Equals(offsetDateTime.Date));
        }

        [Fact(DisplayName = "Can move between string and internal Instant representation through LocalDateTime")]
        [Trait("Test Type", "Unit")]
        [Trait("Target Class", "NodeTimeHelpers")]
        public void LocalDateTimeStringSerialisation()
        {
            var instant = Instant.FromDateTimeUtc(DateTime.Now.ToUniversalTime());
            var localDateTime = instant.ToDateTimeUtc().ToLocalDateTime();
            var stringOption = ToGeneralIsoString(localDateTime);
            Assert.True(!stringOption.IsNone);
            var rep = stringOption.GetOrElse(() => null);
            var parsed = ParseHelpers.ParseGeneralIsoLocalDateTime(rep).Fold(
                time => time,
                () => new LocalDateTime());
            Assert.True(parsed.Date.Equals(localDateTime.Date));
        }

        public void Dispose()
        {
        }
    }
}