using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using JCS.Neon.Glow.Types;
using NodaTime;
using NodaTime.Extensions;
using Xunit;
using static JCS.Neon.Glow.Helpers.NodaTimeHelpers;

namespace JCS.Neon.Glow.Test.Helpers
{
    public class NodaTimeHelpersTests : IDisposable
    {
        [Theory(DisplayName = "Can parse a series of standard ISO date representations to OffsetDateTime instances")]
        [InlineData("")]
        public  void CheckOffsetDateTimeParsing(string rep)
        {
            var parsed = ParseGeneralIsoOffsetDateTime(rep);
            Assert.True(!parsed.IsNone);
        }
        
        [Theory(DisplayName = "Can parse a series of standard ISO date representations to LocalDateTime instances")]
        [InlineData("")]
        public  void CheckLocalDatetimeParsing(string rep)
        {
            var parsed = ParseGeneralIsoOffsetDateTime(rep);
            Assert.True(!parsed.IsNone);
        }
        
        /// <summary>
        /// Test OffsetDateTime transfer between object and string
        /// </summary>
        [Fact(DisplayName = "Can move between string and internal Instant representation through OffsetDateTime")]
        public  void OffsetDateTimeStringSerialisation()
        {
            var instant = Instant.FromDateTimeUtc(DateTime.Now.ToUniversalTime());
            var offsetDateTime = instant.InUtc().ToOffsetDateTime();
            var stringOption = ToGeneralIsoString(offsetDateTime);
            Assert.True(!stringOption.IsNone);
            var rep = stringOption.GetOrElse(() => null);
            var parsed= ParseGeneralIsoOffsetDateTime(rep).Fold(
                time => time,
                () => new OffsetDateTime());
            Assert.True(parsed.Date.Equals(offsetDateTime.Date)); 
        }
        
        /// <summary>
        /// Test OffsetDateTime transfer between object and string
        /// </summary>
        [Fact(DisplayName = "Can move between string and internal Instant representation through LocalDateTime")]
        public  void LocalDateTimeStringSerialisation()
        {
            var instant = Instant.FromDateTimeUtc(DateTime.Now.ToUniversalTime());
            var localDateTime = instant.ToDateTimeUtc().ToLocalDateTime();
            var stringOption = ToGeneralIsoString(localDateTime);
            Assert.True(!stringOption.IsNone);
            var rep = stringOption.GetOrElse(() => null);
            var parsed= ParseGeneralIsoLocalDateTime(rep).Fold(
                time => time,
                () => new LocalDateTime());
            Assert.True(parsed.Date.Equals(localDateTime.Date)); 
        }
        
        public void Dispose()
        {
        }
    }
}