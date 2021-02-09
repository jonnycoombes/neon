#region

using System;
using System.Collections.Generic;
using System.IO;
using JCS.Neon.Glow.Logging;
using JCS.Neon.Glow.OS.Interop.Windows;
using Serilog;

#endregion

namespace JCS.Neon.Glow.Console
{
    /// <summary>
    ///     <para>
    ///         Static alternative to the <see cref="Console" /> which provides support for various ANSI terminal operations.
    ///         This
    ///         wrapper
    ///         around the basic .NET core <see cref="System.Console" /> has a number of differences and enhancements:
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
    public static class AnsiConsole
    {
        /// <summary>
        ///     Static logger
        /// </summary>
        private static readonly ILogger _log = Log.ForContext(typeof(AnsiConsole));

        /// <summary>
        ///     Stack used for storing cursor positions
        /// </summary>
        private static readonly Stack<CursorPosition> _cursorPositions = new();

        /// <summary>
        ///     Static constructor
        /// </summary>
        static AnsiConsole()
        {
            LogHelpers.MethodCall(_log);
            Kernel32.EnableVirtualTerminal();
        }

        /// <summary>
        ///     <see cref="CursorPosition" /> representing the extreme top left of the display
        /// </summary>
        public static CursorPosition TopLeft => new(1, 1);

        /// <summary>
        ///     <see cref="CursorPosition" /> representing the extreme top right of the display
        /// </summary>
        public static CursorPosition TopRight => new(1, (uint) System.Console.BufferWidth);

        /// <summary>
        ///     <see cref="CursorPosition" /> representing the extreme bottom left of the display
        /// </summary>
        public static CursorPosition BottomLeft => new((uint) System.Console.BufferHeight, 1);

        /// <summary>
        ///     <see cref="CursorPosition" /> representing the extreme bottom right of the display
        /// </summary>
        public static CursorPosition BottomRight => new((uint) System.Console.BufferHeight, (uint) System.Console.BufferWidth);

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
        ///     Hides the cursor
        /// </summary>
        public static void HideCursor()
        {
            CheckedWrite(AnsiControlCodes.HideCursor);
        }

        /// <summary>
        ///     Displays the cursor
        /// </summary>
        public static void ShowCursor()
        {
            CheckedWrite(AnsiControlCodes.ShowCursor);
        }

        /// <summary>
        ///     Resets the cursor to the first row, first column
        /// </summary>
        public static void ResetCursor()
        {
            CheckedWrite(AnsiControlCodes.CursorPosition(1, 1));
        }

        /// <summary>
        ///     Sets the cursor postion to a given row and column.  Both indexes are 0-based
        /// </summary>
        /// <param name="row">The row number</param>
        /// <param name="column">The column number</param>
        public static void SetCursorPosition(uint row, uint column)
        {
            CheckedWrite(AnsiControlCodes.CursorPosition(row, column));
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
        ///     Checked version of System.Console.Write which logs information about any I/O errors.
        /// </summary>
        /// <param name="value">The value to write</param>
        private static void CheckedWrite(string value)
        {
            try
            {
                System.Console.Write(value);
            }
            catch (IOException ex)
            {
                LogHelpers.ExceptionWarning(_log, ex);
            }
        }

        /// <summary>
        ///     Checked version of System.Console.WriteLine which logs information about any I/O errors
        /// </summary>
        /// <param name="value">The value to write</param>
        private static void CheckedWriteLine(string value)
        {
            try
            {
                System.Console.WriteLine(value);
            }
            catch (IOException ex)
            {
                LogHelpers.ExceptionWarning(_log, ex);
            }
        }

        /// <summary>
        ///     Checked version of System.Console.Write which logs I/O errors
        /// </summary>
        /// <param name="format">A format string</param>
        /// <param name="args">Optional argument array</param>
        private static void CheckedWrite(string format, object[]? args)
        {
            try
            {
                System.Console.Write(format, args);
            }
            catch (IOException ex)
            {
                LogHelpers.ExceptionWarning(_log, ex);
            }
        }

        /// <summary>
        ///     Checked version of System.Console.WriteLine which logs I/O errors
        /// </summary>
        /// <param name="format">A format string</param>
        /// <param name="args">Optional argument array</param>
        private static void CheckedWriteLine(string format, object[]? args)
        {
            try
            {
                System.Console.WriteLine(format, args);
            }
            catch (IOException ex)
            {
                LogHelpers.ExceptionWarning(_log, ex);
            }
        }

        /// <summary>
        ///     Gets the current cursor position as a string representation
        /// </summary>
        /// <returns>A string of the format "[row:{r},col:{c}]" where {r} is the current row, {c} is the current column</returns>
        public static string ReportCursorPositionString()
        {
            return $"[row:{System.Console.CursorTop + 1}, col:{System.Console.CursorLeft + 1}]";
        }

        /// <summary>
        ///     Utility function that just returns the current cursor position in (row, column) format
        /// </summary>
        /// <param name="restoreCursor">Whether or not the cursor position should be restored after writing</param>
        public static void ReportCursorPosition(bool restoreCursor = true)
        {
            if (restoreCursor)
            {
                WriteRestoreCursor($"[row:{System.Console.CursorTop + 1}, col:{System.Console.CursorLeft + 1}]");
            }
            else
            {
                Write($"[row:{System.Console.CursorTop + 1}, col:{System.Console.CursorLeft + 1}]");
            }
        }

        /// <summary>
        ///     Writes out the current display metrics - i.e. the width and height of the display
        /// </summary>
        /// <param name="restoreCursor">Whether or not the cursor position should be restored after the write</param>
        public static void ReportDisplayGeometry(bool restoreCursor = true)
        {
            if (restoreCursor)
            {
                WriteRestoreCursor($"[rows:{System.Console.BufferHeight}, cols:{System.Console.BufferWidth}]");
            }
            else
            {
                Write($"[rows:{System.Console.BufferHeight}, cols:{System.Console.BufferWidth}]");
            }
        }

        /// <summary>
        ///     Returns a string representation of the current screen geometry
        /// </summary>
        /// <returns>
        ///     A string in the form "[rows:{r},cols:{c}]" where {r} is the current number of rows, {c} is the current number
        ///     of columns
        /// </returns>
        public static string ReportDisplayGeometryString()
        {
            return $"[rows:{System.Console.BufferHeight}, cols:{System.Console.BufferWidth}]";
        }

        /// <summary>
        ///     Writes to the display at the current cursor position with a terminating newline
        /// </summary>
        /// <param name="value">The value to write to the console</param>
        public static void WriteLine(string value)
        {
            CheckedWriteLine(value);
        }

        /// <summary>
        ///     Pushes the current cursor position onto the cursor position stack
        /// </summary>
        private static void PushCursorPosition()
        {
            _cursorPositions.Push(new CursorPosition());
        }

        /// <summary>
        ///     Pops and discards a cursor position from the cursor position stack
        /// </summary>
        private static void PopCursorPosition()
        {
            _cursorPositions.Pop();
        }

        /// <summary>
        ///     Pops a <see cref="CursorPosition" /> off the stack, and then sets the display cursor position based on it
        /// </summary>
        private static void RestoreCursorPosition()
        {
            if (_cursorPositions.TryPop(out var position))
            {
                SetCursorPosition(position.Row, position.Column);
            }
        }

        /// <summary>
        ///     Writes to the display at the current cursor position without a terminating newline, and then restores the cursor
        ///     position
        ///     to the location it was at prior to the write
        /// </summary>
        /// <param name="value"></param>
        public static void WriteLineRestoreCursor(string value)
        {
            PushCursorPosition();
            WriteLine(value);
            RestoreCursorPosition();
        }

        /// <summary>
        ///     Writes to the display at the current cursor position without a terminating newline, and then restores the cursor
        ///     position
        ///     to the location it was at prior to the write
        /// </summary>
        /// <param name="format">A format string</param>
        /// <param name="args">Optional argument array to be applied to the format string</param>
        public static void WriteLineRestoreCursor(string format, object[]? args)
        {
            PushCursorPosition();
            WriteLine(format, args);
            RestoreCursorPosition();
        }

        /// <summary>
        ///     Writes to the display at the current cursor position without a terminating newline
        /// </summary>
        /// <param name="value">The value to write to the console</param>
        public static void Write(string value)
        {
            CheckedWrite(value);
        }

        /// <summary>
        ///     Writes to the display at the current cursor position without a terminating newline and then restores the cursor to
        ///     the position it was in prior to the write
        /// </summary>
        /// <param name="value">The value to write to the console</param>
        public static void WriteRestoreCursor(string value)
        {
            PushCursorPosition();
            Write(value);
            RestoreCursorPosition();
        }

        /// <summary>
        ///     Writes a formatted string to the console at the current cursor position, with a terminating newline
        /// </summary>
        /// <param name="format">A format string</param>
        /// <param name="args">The arguments to be applied to the format string</param>
        public static void WriteLine(string format, object[]? args)
        {
            CheckedWriteLine(format, args);
        }

        /// <summary>
        ///     Writes a formatted string to the display at the current cursor position, without a terminating newline
        /// </summary>
        /// <param name="format">A format string</param>
        /// <param name="args">The arguments to be applied to the format string</param>
        public static void WriteRestoreCursor(string format, object[]? args)
        {
            PushCursorPosition();
            CheckedWrite(format, args);
            RestoreCursorPosition();
        }

        /// <summary>
        ///     Writes a formatted string to the display at the current cursor position, without a terminating newline
        /// </summary>
        /// <param name="format">A format string</param>
        /// <param name="args">The arguments to be applied to the format string</param>
        public static void Write(string format, object[]? args)
        {
            CheckedWrite(format, args);
        }

        /// <summary>
        ///     Record for storing a pair of cursor coordinates
        /// </summary>
        public record CursorPosition
        {
            /// <summary>
            ///     Construtor captures the current cursor position
            /// </summary>
            public CursorPosition()
            {
                Row = (uint) System.Console.CursorTop + 1;
                Column = (uint) System.Console.CursorLeft + 1;
            }

            /// <summary>
            ///     Constructor which allows for the setting of row and column values
            /// </summary>
            /// <param name="row">The row value (1-based)</param>
            /// <param name="column">The column value (1-based)</param>
            public CursorPosition(uint row, uint column)
            {
                Row = row;
                Column = column;
            }

            /// <summary>
            ///     The row coordinate (1-based)
            /// </summary>
            public uint Row { get; }

            /// <summary>
            ///     The column coordinate (1-based)
            /// </summary>
            public uint Column { get; }
        }
    }
}