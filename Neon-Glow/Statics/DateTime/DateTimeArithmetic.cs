#region

using System.Collections.Generic;
using NodaTime;

#endregion

namespace JCS.Neon.Glow.Statics.DateTime
{
    /// <summary>
    ///     Class for holding static helpers which involve arithmetic performed on <see cref="System.DateTime" /> instances, or Noda specific
    ///     time/data structures.  Internally, even calculations involving the system (.NET) <see cref="System.DateTime" /> structure will be
    ///     converted internally to Noda representations in order to take advantage of Noda's improved functionality.
    /// </summary>
    public static class DateTimeArithmetic
    {
        /// <summary>
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="startDay"></param>
        /// <param name="endDay"></param>
        /// <returns></returns>
        public static DateInterval[] WeekdayBuckets(System.DateTime start, System.DateTime end, IsoDayOfWeek startDay, IsoDayOfWeek endDay)
        {
            return WeekdayBuckets(LocalDateTime.FromDateTime(start), LocalDateTime.FromDateTime(end), startDay, endDay);
        }

        /// <summary>
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="startDay"></param>
        /// <param name="endDay"></param>
        /// <returns></returns>
        public static DateInterval[] WeekdayBuckets(LocalDateTime start, LocalDateTime end, IsoDayOfWeek startDay, IsoDayOfWeek endDay)
        {
            var intervals = new List<DateInterval>();
            return intervals.ToArray();
        }
    }
}