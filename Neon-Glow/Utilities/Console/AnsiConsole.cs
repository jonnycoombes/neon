#region

using System;
using JCS.Neon.Glow.Utilities.General;
using Serilog;

#endregion

namespace JCS.Neon.Glow.Utilities.Console
{
    /// <summary>
    ///     Static alternative to the <see cref="Console" /> which provides support for various ANSI terminal operations
    /// </summary>
    public static class AnsiConsole
    {
        /// <summary>
        ///     Static logger
        /// </summary>
        private static readonly ILogger _log = Log.ForContext(typeof(AnsiConsole));

        /// <summary>
        ///     Static constructor
        /// </summary>
        static AnsiConsole()
        {
            Logs.MethodCall(_log);
        }
    }
}