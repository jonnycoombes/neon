#region

using System;
using System.Text;
using System.Threading;
using JCS.Neon.Glow.Logging;
using JCS.Neon.Glow.OS;
using JCS.Neon.Glow.OS.Interop.Windows;
using Serilog;

#endregion

namespace JCS.Neon.Glow.Console
{
    /// <summary>
    ///     <para>
    ///         Static alternative to the <see cref="Console" /> which provides support for various ANSI terminal operations.
    ///         This wrapper around the basic .NET core <see cref="System.Console" /> has a number of differences and
    ///         enhancements:
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
            Primary = 0,
            Alternative = 1
        }

        /// <summary>
        ///     Static logger
        /// </summary>
        private static readonly ILogger _log = Log.ForContext(typeof(AnsiConsole));

        /// <summary>
        ///     The currently active buffer
        /// </summary>
        private static AnsiConsoleBuffer _currentBuffer = AnsiConsoleBuffer.Primary;

        /// <summary>
        ///     The current states of the console - one for each buffer.  Updates to state only affect the current buffer
        /// </summary>
        private static readonly AnsiConsoleState[] _states = {new(), new()};

        /// <summary>
        ///     A <see cref="Timer" /> used to periodically update the current state of the console
        /// </summary>
        private static Timer _stateUpdateTimer =
            new(UpdateStateCallback, null, TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(100));

        /// <summary>
        ///     Static constructor - this does all the necessary preparatory work setting up the <see cref="System.Console" /> so
        ///     that it behaves a bit more as we'd expect it to in the 21st century.
        /// </summary>
        static AnsiConsole()
        {
            LogHelpers.MethodCall(_log);
            try
            {
                Kernel32.EnableVirtualTerminal();
                System.Console.OutputEncoding = Encoding.UTF8;
                _currentBuffer = AnsiConsoleBuffer.Primary;
                UpdateConsoleState();
            }
            catch (Exception ex)
            {
                LogHelpers.Warning(_log, $"Caught an exception whilst attempting to setup the console \"{ex.Message}\"");
                LogHelpers.ExceptionWarning(_log, ex);
            }
        }

        /// <summary>
        ///     Returns the currently active buffer
        /// </summary>
        public static AnsiConsoleBuffer CurrentBuffer => _currentBuffer;

        /// <summary>
        ///     Returns the currently active state, depending on which buffer is currently in use
        /// </summary>
        /// <returns>The currently applicable <see cref="AnsiConsoleState" /> instance</returns>
        private static AnsiConsoleState CurrentState => _currentBuffer == AnsiConsoleBuffer.Primary ? _states[0] : _states[1];

        /// <summary>
        ///     Returns the current number of rows in the display
        /// </summary>
        public static int Rows => CurrentState.Rows;

        /// <summary>
        ///     Returns the current number of columns in the display
        /// </summary>
        public static int Columns => CurrentState.Columns;

        /// <summary>
        ///     Timed callback that updates the console state periodically
        /// </summary>
        /// <param name="state"></param>
        private static void UpdateStateCallback(object? state)
        {
            UpdateConsoleState();
        }

        /// <summary>
        ///     Updates the currently active state of the terminal.  Writes the information into the currently active
        ///     <see cref="AnsiConsoleState" /> instance
        /// </summary>
        private static void UpdateConsoleState()
        {
            var state = CurrentState;

            // windows specific call to retrieve the title
            if (PlatformInformation.IsWindows)
            {
                state.Title = System.Console.Title;
            }

            // raise an event if the geometry has changed
            state.Rows = System.Console.BufferHeight;
            state.Columns = System.Console.BufferWidth;
        }

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
        ///     Switches to the alternative buffer and vice-versa.  The current buffer in use is tracked within the console state
        ///     structure.  Note that when switching to the alternate buffer, it is always cleared.  The
        ///     <paramref name="clearPrimary" />
        ///     parameter lets the callee decided whether the primary buffer should be cleared when switched to
        /// </summary>
        /// <param name="clearPrimary">Whether or not the primary buffer should be cleared when switched do.</param>
        public static void SwitchBuffer(bool clearPrimary = false)
        {
            if (_currentBuffer == AnsiConsoleBuffer.Primary)
            {
                CheckedWrite(AnsiControlCodes.EnableAlternativeScreenBuffer);
                _currentBuffer = AnsiConsoleBuffer.Alternative;
                UpdateConsoleState();
            }
            else
            {
                CheckedWrite(AnsiControlCodes.DisableAlternativeScreenBuffer);
                _currentBuffer = AnsiConsoleBuffer.Primary;
                if (clearPrimary)
                {
                    ClearDisplay();
                }

                UpdateConsoleState();
            }
        }

        /// <summary>
        ///     Switches the current buffer and also sets the window title
        /// </summary>
        /// <param name="title">The title to apply</param>
        /// <param name="clearPrimary">Whether or not the primary buffer should be cleared when switched to</param>
        public static void SwitchBuffer(string title, bool clearPrimary = false)
        {
            SwitchBuffer(clearPrimary);
            SetTitle(title);
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
            CurrentState.Title = title;
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