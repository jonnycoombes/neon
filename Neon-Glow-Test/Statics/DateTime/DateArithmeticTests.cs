#region

using System;
using JCS.Neon.Glow.Statics.DateTime;
using NodaTime;
using Xunit;

#endregion

namespace JCS.Neon.Glow.Test.Statics.DateTime
{
    /// <summary>
    ///     Tests for date/time arithmetic helper functions
    /// </summary>
    [Trait("Category", "Dates")]
    public class DateArithmeticTests : TestBase
    {
        
        /// <summary>
        /// Static set of start date fixtures
        /// </summary>
        private static LocalDate[] startDates = new LocalDate[]
        {
            LocalDate.FromDateTime(System.DateTime.Today)
        };

        /// <summary>
        /// Static set of end date fixtures
        /// </summary>
        private static LocalDate[] endDates = new LocalDate[]
        {
            startDates[0].Plus(Period.FromDays(4)),
            startDates[0].Plus(Period.FromMonths(2)),
            startDates[0].Plus(Period.FromDays(1))
        };
        
        
        [Fact(DisplayName = "Cannot create weekly buckets with incorrect start and end dates")]
        [Trait("Category", "Dates")]
        public void IncorrectStartAndEndDates()
        {
            var start = LocalDate.FromDateTime(System.DateTime.Now);
            var end = start.PlusDays(-1);
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var intervals = DateTimeArithmetic.WeeklyIntervals(start, end, IsoDayOfWeek.Sunday);
            });
        }
        
        [Fact(DisplayName = "Cannot create weekly buckets with incorrect starting day")]
        [Trait("Category", "Dates")]
        public void IncorrectStartingDay()
        {
            var start = LocalDate.FromDateTime(System.DateTime.Now);
            var end = start.PlusDays(5);
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var intervals = DateTimeArithmetic.WeeklyIntervals(start, end, IsoDayOfWeek.None);
            });
        }

        [Fact(DisplayName = "Should create 2 intervals for a +7 date range")]
        public void SevenDayBucketCountCheck()
        {
            var start = LocalDate.FromDateTime(System.DateTime.Today);
            var end = start.PlusDays(7);
            var intervals = DateTimeArithmetic.WeeklyIntervals(start, end, start.DayOfWeek);
            var count = intervals.Length;
            Assert.Equal(2, count);
        }
            
        [Fact(DisplayName = "Should create 2 intervals for a +14 date range")]
        public void FourteenDayBucketCountCheck()
        {
            var start = LocalDate.FromDateTime(System.DateTime.Today);
            var end = start.PlusDays(14);
            var intervals = DateTimeArithmetic.WeeklyIntervals(start, end, start.DayOfWeek);
            var count = intervals.Length;
            Assert.Equal(3, count);
        }
        
    }
}