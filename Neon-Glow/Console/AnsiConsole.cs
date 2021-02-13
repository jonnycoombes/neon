#region

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using JCS.Neon.Glow.Logging;
using JCS.Neon.Glow.OS.Interop.Windows;
using JCS.Neon.Glow.Types;
using Serilog;

#endregion

namespace JCS.Neon.Glow.Console
{
    /// <summary>
    ///     <para>
    ///         Static alternative to the <see cref="Console" /> which provides support for various ANSI terminal operations.
    ///         This wrapper around the basic .NET core <see cref="System.Console" /> has a number of differences and enhancements:
    ///     </para>
    ///     <para>
    ///         AnsiConsole will attempt to switch to a virtual terminal mode on Windows 10 automatically using underlying
    ///         calls to the Win32 API.
    ///     </para>
    ///     <para>
    ///         All cursor positioning is done based on 1-based row and column coordinates, taking care of the translation
    ///         between the undelrying zero-based system used within .NET.
    ///     </para>
    /// </summary>
    public static partial class AnsiConsole
    {
        /// <summary>
        ///     An enumeration used to track which buffer the console is currently performing operations on
        /// </summary>
        public enum AnsiConsoleBuffer
        {
            Primary,
            Alternative
        }

        /// <summary>
        ///     Static logger
        /// </summary>
        private static readonly ILogger _log = Log.ForContext(typeof(AnsiConsole));

        /// <summary>
        /// The state of the console at a given point in time
        /// </summary>
        private static AnsiConsoleState _state = new AnsiConsoleState();
        
        /// <summary>
        ///     Static constructor - this does all the necessary preparatory work setting up the <see cref="System.Console" /> so that
        ///     it behaves a bit more as we'd expect it to in the 21st century.
        /// </summary>
        static AnsiConsole()
        {
            LogHelpers.MethodCall(_log);
            try
            {
                Kernel32.EnableVirtualTerminal();
                System.Console.OutputEncoding = Encoding.UTF8;
            }
            catch (Exception ex)
            {
                LogHelpers.Warning(_log, $"Caught an exception whilst attempting to setup the console \"{ex.Message}\"");
                LogHelpers.ExceptionWarning(_log, ex);
            }
        }

        /// <summary>
        ///     Returns the currently reported width of the console
        /// </summary>
        public static int Columns => System.Console.BufferWidth;

        /// <summary>
        ///     Returns the currently reported height of the console
        /// </summary>
        public static int Rows => System.Console.BufferHeight;

        /// <summary>
        ///     Clears the display by issuing the ClearDisplay ANSI code to the current console
        /// </summary>
        /// <param name="resetCursor">If set to true, the cursor will be reset to the origin after clearing the display</param>
        /// <param name="clearBuffer">
        ///     If set to true, the scrollback buffer is also cleared.  By default, this is disabled because
        ///     it doesn't look to be supported very well across different terminals
        /// </param>
        public static void ClearDisplay(bool resetCursor = false, bool clearBuffer = false)
        {
            CheckedWrite(clearBuffer ? AnsiControlCodes.EraseDisplayClearBuffer : AnsiControlCodes.EraseDisplay);
            if (resetCursor)
            {
                ResetCursor();
            }
        }

        /// <summary>
        ///     Switches to the alternative buffer and vice-versa.  The current buffer in use is tracked within the console state structure
        /// </summary>
        public static void SwitchBuffer()
        {
        }

        /// <summary>
        ///     Clears the current display from the current cursor position to the end of the display
        /// </summary>
        public static void ClearToEnd()
        {
            CheckedWrite(AnsiControlCodes.ClearToEnd);
        }

        /// <summary>
        ///     Clears the current display from the top of the display to the current cursor position
        /// </summary>
        public static void ClearToCursor()
        {
            CheckedWrite(AnsiControlCodes.ClearToCursor);
        }

        /// <summary>
        ///     Sets the current window/terminal title
        /// </summary>
        /// <param name="title">The title to set</param>
        public static void SetTitle(string title)
        {
            CheckedWrite($"{AnsiControlCodes.SetWindowTitle(title)}");
        }


        /// <summary>
        ///     Exception class used throughout the <see cref="AnsiConsole" /> in the event of critical or transient errors
        /// </summary>
        public class AnsiConsoleException : Exception
        {
            public AnsiConsoleException(string? message) : base(message)
            {
            }

            public AnsiConsoleException(string? message, Exception? innerException) : base(message, innerException)
            {
            }
        }
    }
}