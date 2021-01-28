#region

using System;
using JCS.Neon.Glow.Types;
using JCS.Neon.Glow.Utilities.General;
using NodaTime;
using NodaTime.Extensions;
using Xunit;

#endregion

namespace JCS.Neon.Glow.Test.Utilities.General
{
    /// <summary>
    ///     Test suite for <see cref="JCS.Neon.Glow.Helpers.NodaTimeHelpers" />
    /// </summary>
    [Trait("Category", "General")]
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
        [Trait("Category", "General")]
        public void CheckOffsetDateTimeParsing(string src)
        {
            Assert.True(!Parsing.ParseGeneralIsoOffsetDateTime(src).IsNone);
        }

        [Theory(DisplayName = "Can parse a series of standard ISO date representations to LocalDateTime instances")]
        [InlineData("2020-12-31T12:12:00")]
        [InlineData("2020-09-13T09:45:12")]
        [InlineData("2020-01-03T07:23:07")]
        [InlineData("2020-12-31T04:56:22")]
        [Trait("Category", "General")]
        public void CheckLocalDatetimeParsing(string src)
        {
            Assert.True(!Parsing.ParseGeneralIsoLocalDateTime(src).IsNone);
        }

        [Theory(DisplayName = "Can correctly identify ISO representations of invalid date values")]
        [Trait("Category", "General")]
        [InlineData("2020-13-31T12:12:00Z")]
        [InlineData("2020-09-43T09:45:12-04")]
        [InlineData("2020-00-03T07:23:07+02")]
        [InlineData("2020-12-31T54:56:22Z")]
        public void CheckInvalidDates(string src)
        {
            Assert.True(Parsing.ParseGeneralIsoOffsetDateTime(src).IsNone);
            Assert.True(Parsing.ParseGeneralIsoLocalDateTime(src).IsNone);
        }

        [Fact(DisplayName = "Can move between string and internal Instant representation through OffsetDateTime")]
        [Trait("Category", "General")]
        public void OffsetDateTimeStringSerialisation()
        {
            var instant = Instant.FromDateTimeUtc(DateTime.Now.ToUniversalTime());
            var offsetDateTime = instant.InUtc().ToOffsetDateTime();
            var stringOption = DateAndTime.ToGeneralIsoString(offsetDateTime);
            Assert.True(!stringOption.IsNone);
            var rep = stringOption.GetOrElse(() => null);
            var parsed = Parsing.ParseGeneralIsoOffsetDateTime(rep).Fold(
                time => time,
                () => new OffsetDateTime());
            Assert.True(parsed.Date.Equals(offsetDateTime.Date));
        }

        [Fact(DisplayName = "Can move between string and internal Instant representation through LocalDateTime")]
        [Trait("Category", "General")]
        public void LocalDateTimeStringSerialisation()
        {
            var instant = Instant.FromDateTimeUtc(DateTime.Now.ToUniversalTime());
            var localDateTime = instant.ToDateTimeUtc().ToLocalDateTime();
            var stringOption = DateAndTime.ToGeneralIsoString(localDateTime);
            Assert.True(!stringOption.IsNone);
            var rep = stringOption.GetOrElse(() => null);
            var parsed = Parsing.ParseGeneralIsoLocalDateTime(rep).Fold(
                time => time,
                () => new LocalDateTime());
            Assert.True(parsed.Date.Equals(localDateTime.Date));
        }
    }
}