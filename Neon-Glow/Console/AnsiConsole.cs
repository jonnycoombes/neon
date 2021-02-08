#region

using System;
using JCS.Neon.Glow.Interop.Windows;
using JCS.Neon.Glow.Utilities.General;
using Serilog;

#endregion

namespace JCS.Neon.Glow.Console
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
            Kernel32.EnableVirtualTerminal();
        }

        /// <summary>
        ///     Clears the display by issuing the ClearDisplay ANSI code to the current console
        /// </summary>
        /// <param name="clearBuffer">
        ///     If set to true, the scrollback buffer is also cleared.  By default, this is disabled because
        ///     it doesn't look to be supported very well across different terminals
        /// </param>
        public static void ClearDisplay(bool clearBuffer = false)
        {
            System.Console.Write(clearBuffer ? AnsiControlCodes. EraseDisplayClearBuffer: AnsiControlCodes.EraseDisplay);
        }

        public static void ClearToEnd()
        {
            System.Console.Write(AnsiControlCodes.ClearToEnd);
        }

        public static void ClearToCursor()
        {
            System.Console.Write(AnsiControlCodes.ClearToCursor);
        }

        public static void ReportCursorPosition()
        {
            WriteLine($"({System.Console.CursorLeft},{System.Console.CursorTop})");
        }

        /// <summary>
        ///     Writes a simple line to current terminal
        /// </summary>
        /// <param name="value">The line to write to the console</param>
        public static void WriteLine(string value)
        {
            System.Console.WriteLine(value);
        }


        /// <summary>
        ///     Writes a formatted string to the console
        /// </summary>
        /// <param name="format">A format string</param>
        /// <param name="args">The arguments to be applied to the format string</param>
        public static void WriteLine(string format, object[]? args)
        {
            System.Console.WriteLine(format, args);
        }
    }
}