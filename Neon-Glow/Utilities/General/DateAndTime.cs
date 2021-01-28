#region

using JCS.Neon.Glow.Types;
using NodaTime;
using NodaTime.Text;
using Serilog;

#endregion

namespace JCS.Neon.Glow.Utilities.General
{
    /// <summary>
    ///     Class for holding general helper functions related to NodaTime
    /// </summary>
    public static class DateAndTime
    {
        /// <summary>
        ///     PRivate static logger
        /// </summary>
        private static readonly ILogger _log = Log.ForContext(typeof(DateAndTime));

        /// <summary>
        ///     Converts a <see cref="OffsetDateTime" /> to a string
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static Option<string> ToGeneralIsoString(OffsetDateTime? src)
        {
            Logs.MethodCall(_log);
            if (src == null)
                return Option<string>.None;
            return Option<string>.Some(OffsetDateTimePattern.GeneralIso.Format(src.Value));
        }

        /// <summary>
        ///     Converts a <see cref="LocalDateTime" /> to a string using the general ISO format
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static Option<string> ToGeneralIsoString(LocalDateTime? src)
        {
            Logs.MethodCall(_log);
            if (src == null)
                return Option<string>.None;
            return Option<string>.Some(LocalDateTimePattern.GeneralIso.Format(src.Value));
        }
    }
}