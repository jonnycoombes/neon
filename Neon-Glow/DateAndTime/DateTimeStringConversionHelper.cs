#region

using JCS.Neon.Glow.Logging;
using JCS.Neon.Glow.Types;
using NodaTime;
using NodaTime.Text;
using Serilog;

#endregion

namespace JCS.Neon.Glow.DateAndTime
{
    /// <summary>
    ///     Class for holding general helper functions related to NodaTime
    /// </summary>
    public static class DateTimeStringConversionHelper
    {
        /// <summary>
        ///     PRivate static logger
        /// </summary>
        private static readonly ILogger _log = Log.ForContext(typeof(DateTimeStringConversionHelper));

        /// <summary>
        ///     Converts a <see cref="OffsetDateTime" /> to a string
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static Option<string> ToGeneralIsoString(OffsetDateTime? src)
        {
            LogHelper.MethodCall(_log);
            return src == null ? Option<string>.None : Option<string>.Some(OffsetDateTimePattern.GeneralIso.Format(src.Value));
        }

        /// <summary>
        ///     Converts a <see cref="LocalDateTime" /> to a string using the general ISO format
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static Option<string> ToGeneralIsoString(LocalDateTime? src)
        {
            LogHelper.MethodCall(_log);
            return src == null ? Option<string>.None : Option<string>.Some(LocalDateTimePattern.GeneralIso.Format(src.Value));
        }
    }
}