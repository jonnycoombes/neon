using JCS.Neon.Glow.Types;
using NodaTime;
using NodaTime.Text;

namespace JCS.Neon.Glow.Helpers
{
    /// <summary>
    /// Class for holding general helper functions related to NodaTime
    /// </summary>
    public static class NodaTimeHelpers
    {
        /// <summary>
        /// Attempts the parse of an <see cref="OffsetDateTime"/> using the general ISO
        /// pattern
        /// </summary>
        /// <param name="value"></param>
        /// <returns>An option value</returns>
        public static Option<OffsetDateTime> ParseGeneralIsoOffsetDateTime(string? value)
        {
            if (value == null) return Option<OffsetDateTime>.None;
            var result= OffsetDateTimePattern.GeneralIso.Parse(value);
            if (result.Success) 
                return Option<OffsetDateTime>.Some(result.Value);
            else 
                return Option<OffsetDateTime>.None;
        }

        /// <summary>
        /// Converts a <see cref="OffsetDateTime"/> to a string
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Option<string> ToGeneralIsoString(OffsetDateTime? t)
        {
            if (t == null)
                return Option<string>.None;
            else
                return Option<string>.Some(OffsetDateTimePattern.GeneralIso.Format(t.Value));
        }

        /// <summary>
        /// Attempts the parse of an <see cref="LocalDateTime"/> using the general ISO pattern
        /// </summary>
        /// <param name="value"></param>
        /// <returns>An option value</returns>
        public static Option<LocalDateTime> ParseGeneralIsoLocalDateTime(string? value)
        {
            if (value == null) return Option<LocalDateTime>.None;
            var result = LocalDateTimePattern.GeneralIso.Parse(value);
            if (result.Success)
                return Option<LocalDateTime>.Some(result.Value);
            else
                return Option<LocalDateTime>.None;                
        }

        /// <summary>
        /// Converts a <see cref="LocalDateTime"/> to a string using the general ISO format
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Option<string> ToGeneralIsoString(LocalDateTime? t)
        {
            if (t == null)
                return Option<string>.None;
            else
                return Option<string>.Some(LocalDateTimePattern.GeneralIso.Format(t.Value));
        }
    }
}