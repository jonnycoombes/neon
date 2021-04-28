/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using JCS.Neon.Glow.Types;
using NodaTime;
using NodaTime.Text;
using Serilog;
using ArgumentOutOfRangeException = System.ArgumentOutOfRangeException;

#endregion

namespace JCS.Neon.Glow.Statics.DateTime
{
    /// <summary>
    ///     Class for holding general helper functions related to NodaTime
    /// </summary>
    public static class DateTimeConversion
    {
        /// <summary>
        ///     PRivate static logger
        /// </summary>
        private static readonly ILogger _log = Log.ForContext(typeof(DateTimeConversion));

        /// <summary>
        ///     Converts a <see cref="OffsetDateTime" /> to a string
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static Option<string> ToGeneralIsoString(OffsetDateTime? src)
        {
            Logging.MethodCall(_log);
            return src == null ? Option<string>.None : Option<string>.Some(OffsetDateTimePattern.GeneralIso.Format(src.Value));
        }

        /// <summary>
        ///     Converts a <see cref="LocalDateTime" /> to a string using the general ISO format
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static Option<string> ToGeneralIsoString(LocalDateTime? src)
        {
            Logging.MethodCall(_log);
            return src == null ? Option<string>.None : Option<string>.Some(LocalDateTimePattern.GeneralIso.Format(src.Value));
        }

        /// <summary>
        /// Creates a local date interval based on two (possibly non-local) <see cref="DateTime"/> instances
        /// </summary>
        /// <param name="start">The starting point of the interval</param>
        /// <param name="end">The end point of the interval</param>
        /// <returns></returns>
        public static DateInterval ToLocalDateInterval(System.DateTime start, System.DateTime end)
        {
            if (start > end)
            {
                throw new ArgumentOutOfRangeException(nameof(start), @"The start date must be less than or equal to the end date");
            }
            return new DateInterval(LocalDate.FromDateTime(start), LocalDate.FromDateTime(end));
        }
    }
}