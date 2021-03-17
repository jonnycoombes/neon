using JCS.Neon.Glow.Dates;
using JCS.Neon.Glow.Parsing;
using JCS.Neon.Glow.Types;
using NodaTime;
using NodaTime.Extensions;
using Xunit;

namespace JCS.Neon.Glow.Test.Dates
{
    /// <summary>
    /// Tests string conversion routines
    /// </summary>
    [Trait("Category", "Dates")]
    public class DateConversionHelperTests : TestBase
    {
        [Fact(DisplayName = "Can move between string and internal Instant representation through OffsetDateTime")]
        [Trait("Category", "Dates")]
        public void OffsetDateTimeStringSerialisation()
        {
            var instant = Instant.FromDateTimeUtc(System.DateTime.Now.ToUniversalTime());
            var offsetDateTime = instant.InUtc().ToOffsetDateTime();
            var stringOption = DateConversionHelper.ToGeneralIsoString(offsetDateTime);
            Assert.True(!stringOption.IsNone);
            var rep = stringOption.GetOrElse(() => null);
            var parsed = TryParsers.TryParseGeneralIsoOffsetDateTime(rep).Fold(
                time => time,
                () => new OffsetDateTime());
            Assert.True(parsed.Date.Equals(offsetDateTime.Date));
        }

        [Fact(DisplayName = "Can move between string and internal Instant representation through LocalDateTime")]
        [Trait("Category", "Dates")]
        public void LocalDateTimeStringSerialisation()
        {
            var instant = Instant.FromDateTimeUtc(System.DateTime.Now.ToUniversalTime());
            var localDateTime = instant.ToDateTimeUtc().ToLocalDateTime();
            var stringOption = DateConversionHelper.ToGeneralIsoString(localDateTime);
            Assert.True(!stringOption.IsNone);
            var rep = stringOption.GetOrElse(() => null);
            var parsed = TryParsers.TryParseGeneralIsoLocalDateTime(rep).Fold(
                time => time,
                () => new LocalDateTime());
            Assert.True(parsed.Date.Equals(localDateTime.Date));
        } 
    }
}