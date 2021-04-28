/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
﻿#region

using System;
using System.Collections.Generic;
using System.Threading;
using NodaTime;

#endregion

namespace JCS.Neon.Glow.Statics.DateTime
{
    /// <summary>
    ///     Class for holding static helpers which involve arithmetic performed on <see cref="System.DateTime" /> instances, or
    ///     Noda specific
    ///     time/data structures.  Internally, even calculations involving the system (.NET) <see cref="System.DateTime" />
    ///     structure will be
    ///     converted internally to Noda representations in order to take advantage of Noda's improved functionality.
    /// </summary>
    public static class DateTimeArithmetic
    {
        /// <summary>
        ///     <see cref="Period" /> representing a single second
        /// </summary>
        private static Period SingleSecond = Period.FromSeconds(1);

        /// <summary>
        ///     <see cref="Period" /> representing a single day
        /// </summary>
        private static readonly Period SingleDay = Period.FromDays(1);

        /// <summary>
        ///     <see cref="Period" /> representing a single week
        /// </summary>
        private static Period SingleWeek = Period.FromWeeks(1);

        /// <summary>
        ///     <see cref="Period" /> representing a single month
        /// </summary>
        private static Period SingleMonth = Period.FromMonths(1);

        /// <summary>
        ///     <see cref="Period" /> representing a single year
        /// </summary>
        private static Period SingleYear = Period.FromYears(1);

        
        /// <summary>
        ///     This method will create a list of 7 day date interval buckets starting on a given <see cref="startDay" />, so that
        ///     the intervals combined are *inclusive* of both the <see cref="start" /> and <see cref="end" /> dates.
        /// </summary>
        /// <param name="start">A <see cref="LocalDate" /> the earliest date to include in the buckets combined</param>
        /// <param name="end">A <see cref="LocalDate" /> the latest date to include in the buckets combined</param>
        /// <param name="startDay">The <see cref="IsoDayOfWeek" /> to start each bucket on</param>
        /// <returns>
        ///     A list of <see cref="DateInterval" /> non-intersecting intervals, which when combined will include both the
        ///     specified <see cref="start" /> and <see cref="end" /> dates.
        /// </returns>
        public static DateInterval[] WeeklyIntervals(System.DateTime start, System.DateTime end, IsoDayOfWeek startDay)
        {
            return WeeklyIntervals(LocalDateTime.FromDateTime(start), LocalDateTime.FromDateTime(end), startDay);
        }

        /// <summary>
        ///     This method will create a list of 7 day date interval buckets starting on a given <see cref="startDay" />, so that
        ///     the intervals combined are *inclusive* of both the <see cref="start" /> and <see cref="end" /> dates.
        /// </summary>
        /// <param name="start">A <see cref="LocalDate" /> the earliest date to include in the buckets combined</param>
        /// <param name="end">A <see cref="LocalDate" /> the latest date to include in the buckets combined</param>
        /// <param name="startDay">The <see cref="IsoDayOfWeek" /> to start each bucket on</param>
        /// <returns>
        ///     A list of <see cref="DateInterval" /> non-intersecting intervals, which when combined will include both the
        ///     specified <see cref="start" /> and <see cref="end" /> dates.
        /// </returns>
        public static DateInterval[] WeeklyIntervals(LocalDateTime start, LocalDateTime end, IsoDayOfWeek startDay)
        {
            return WeeklyIntervals(start.Date, end.Date, startDay);
        }

        /// <summary>
        ///     This method will create a list of 7 day date interval buckets starting on a given <see cref="startDay" />, so that
        ///     the intervals combined are *inclusive* of both the <see cref="start" /> and <see cref="end" /> dates.
        /// </summary>
        /// <param name="start">A <see cref="LocalDate" /> the earliest date to include in the buckets combined</param>
        /// <param name="end">A <see cref="LocalDate" /> the latest date to include in the buckets combined</param>
        /// <param name="startDay">The <see cref="IsoDayOfWeek" /> to start each bucket on</param>
        /// <returns>
        ///     An array of non-intersecting <see cref="DateInterval" /> instances, which when combined will include both the
        ///     specified <see cref="start" /> and <see cref="end" /> dates.
        /// </returns>
        public static DateInterval[] WeeklyIntervals(LocalDate start, LocalDate end, IsoDayOfWeek startDay)
        {
            var intervals = new List<DateInterval>();

            if (start > end)
            {
                throw new ArgumentOutOfRangeException(nameof(start), @"start must be before the end date");
            }

            if (startDay == IsoDayOfWeek.None)
            {
                throw new ArgumentOutOfRangeException(nameof(startDay), @"You must specify a valid starting day for the buckets");
            }

            var dayPeriod = Period.FromDays(1);
            var bucketStart = start;
            while (bucketStart.DayOfWeek != startDay)
            {
                bucketStart = bucketStart.Minus(SingleDay);
            }
            
            var done = false;
            var dateCursor = bucketStart;
            while (!done)
            {
                var interval = new DateInterval(bucketStart, bucketStart.PlusDays(6));
                intervals.Add(interval);
                done = interval.Contains(end);
                if (!done)
                {
                    bucketStart = bucketStart.PlusDays(7);
                }
            }
            
            return intervals.ToArray();
        }
    }
}