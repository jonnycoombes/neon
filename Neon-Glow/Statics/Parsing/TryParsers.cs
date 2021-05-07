/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System;
using JCS.Neon.Glow.Types;
using NodaTime;
using NodaTime.Text;
using Serilog;

#endregion

namespace JCS.Neon.Glow.Statics.Parsing
{
    /// <summary>
    ///     Contains a bunch of static methods which will attempt ("try") to parse various types and return an
    ///     <see cref="Option{T}" />
    /// </summary>
    public static class TryParsers
    {
        /// <summary>
        ///     Static logger
        /// </summary>
        private static readonly ILogger _log = Log.ForContext(typeof(TryParsers));

        /// <summary>
        ///     Attempts to parse out a guid from a given string
        /// </summary>
        /// <param name="input">The source string</param>
        /// <returns>An option which will be some if the parse is successful</returns>
        public static Option<Guid> TryParseGuid(string input)
        {
            Logging.MethodCall(_log);
            try
            {
                return Option<Guid>.Some(Guid.Parse(input));
            }
            catch
            {
                Logging.Warning(_log, $"Failed to parse GUID with input \"{input}\"");
                return Option<Guid>.None;
            }
        }

        /// <summary>
        ///     Attempts to parse a <see cref="Uri" /> from a given source string
        /// </summary>
        /// <param name="input">The source</param>
        /// <returns>An option which will be Some if the parse is successful</returns>
        public static Option<Uri> TryParseUri(string input)
        {
            Logging.MethodCall(_log);
            try
            {
                return Option<Uri>.Some(new Uri(input));
            }
            catch
            {
                Logging.Warning(_log, $"Failed to parse Uri with input \"{input}\"");
                return Option<Uri>.None;
            }
        }

        /// <summary>
        ///     Attempts the parse of an <see cref="OffsetDateTime" /> using the general ISO
        ///     pattern
        /// </summary>
        /// <param name="src"></param>
        /// <returns>An option value</returns>
        public static Option<OffsetDateTime> TryParseGeneralIsoOffsetDateTime(string? src)
        {
            Logging.MethodCall(_log);
            if (src == null)
            {
                return Option<OffsetDateTime>.None;
            }

            var result = OffsetDateTimePattern.GeneralIso.Parse(src);
            return result.Success ? Option<OffsetDateTime>.Some(result.Value) : Option<OffsetDateTime>.None;
        }

        /// <summary>
        ///     Attempts the parse of an <see cref="LocalDateTime" /> using the general ISO pattern
        /// </summary>
        /// <param name="src"></param>
        /// <returns>An option value</returns>
        public static Option<LocalDateTime> TryParseGeneralIsoLocalDateTime(string? src)
        {
            Logging.MethodCall(_log);
            if (src == null)
            {
                return Option<LocalDateTime>.None;
            }

            var result = LocalDateTimePattern.GeneralIso.Parse(src);
            return result.Success ? Option<LocalDateTime>.Some(result.Value) : Option<LocalDateTime>.None;
        }
    }
}