using System;
using JCS.Neon.Glow.Types;
using NodaTime;
using NodaTime.Text;

namespace JCS.Neon.Glow.Helpers.General
{
    /// <summary>
    /// Class containing various methods for parsing out values from strings
    /// </summary>
    public static class ParseHelpers
    {
        /// <summary>
        /// Attempts to parse out a guid from a given string
        /// </summary>
        /// <param name="src">The source string</param>
        /// <returns>An option which will be some if the parse is successful</returns>
        public static Option<Guid> ParseGuid(string src)
        {
            try
            {
                return Option<Guid>.Some(Guid.Parse(src));
            }
            catch
            {
                return Option<Guid>.None;
            }
        }

        /// <summary>
        /// Attempts to parse a <see cref="Uri"/> from a given source string
        /// </summary>
        /// <param name="src">The source</param>
        /// <returns>An option which will be Some if the parse is successful</returns>
        public static Option<Uri> ParseUri(string src)
        {
            try
            {
                return Option<Uri>.Some(new Uri(src));
            }
            catch
            {
                return Option<Uri>.None;
            }
        }

        /// <summary>
        /// Attempts the parse of an <see cref="OffsetDateTime"/> using the general ISO
        /// pattern
        /// </summary>
        /// <param name="src"></param>
        /// <returns>An option value</returns>
        public static Option<OffsetDateTime> ParseGeneralIsoOffsetDateTime(string? src)
        {
            if (src == null) return Option<OffsetDateTime>.None;
            var result = OffsetDateTimePattern.GeneralIso.Parse(src);
            if (result.Success)
                return Option<OffsetDateTime>.Some(result.Value);
            else
                return Option<OffsetDateTime>.None;
        }

        /// <summary>
        /// Attempts the parse of an <see cref="LocalDateTime"/> using the general ISO pattern
        /// </summary>
        /// <param name="src"></param>
        /// <returns>An option value</returns>
        public static Option<LocalDateTime> ParseGeneralIsoLocalDateTime(string? src)
        {
            if (src == null) return Option<LocalDateTime>.None;
            var result = LocalDateTimePattern.GeneralIso.Parse(src);
            if (result.Success)
                return Option<LocalDateTime>.Some(result.Value);
            else
                return Option<LocalDateTime>.None;
        }
    }
}