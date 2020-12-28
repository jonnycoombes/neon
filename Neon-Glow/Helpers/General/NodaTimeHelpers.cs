using JCS.Neon.Glow.Types;
using NodaTime;
using NodaTime.Text;

namespace JCS.Neon.Glow.Helpers.General
{
    /// <summary>
    ///     Class for holding general helper functions related to NodaTime
    /// </summary>
    public static class NodaTimeHelpers
    {
        /// <summary>
        ///     Converts a <see cref="OffsetDateTime" /> to a string
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static Option<string> ToGeneralIsoString(OffsetDateTime? src)
        {
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
            if (src == null)
                return Option<string>.None;
            return Option<string>.Some(LocalDateTimePattern.GeneralIso.Format(src.Value));
        }
    }
}