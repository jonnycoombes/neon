#region

using System;
using JCS.Neon.Glow.Statics;

#endregion

namespace JCS.Neon.Glow.Console
{
    /// <summary>
    /// </summary>
    public static partial class AnsiConsole
    {
        /// <summary>
        ///     Checked version of System.Console.Write which logs information about any I/O errors.
        /// </summary>
        /// <param name="value">The value to write</param>
        private static void CheckedWrite(string value)
        {
            CheckEnabled();
            try
            {
                System.Console.Write(value);
            }
            catch (Exception ex)
            {
                Logging.ExceptionWarning(_log, ex);
            }
        }

        /// <summary>
        ///     Checked version of System.Console.WriteLine which logs information about any I/O errors
        /// </summary>
        /// <param name="value">The value to write</param>
        private static void CheckedWriteLine(string value)
        {
            CheckEnabled();
            try
            {
                System.Console.WriteLine(value);
            }
            catch (Exception ex)
            {
                Logging.ExceptionWarning(_log, ex);
            }
        }

        /// <summary>
        ///     Checked version of System.Console.Write which logs I/O errors
        /// </summary>
        /// <param name="format">A format string</param>
        /// <param name="args">Optional argument array</param>
        private static void CheckedWrite(string format, object[]? args)
        {
            CheckEnabled();
            try
            {
                System.Console.Write(format, args);
            }
            catch (Exception ex)
            {
                Logging.ExceptionWarning(_log, ex);
            }
        }

        /// <summary>
        ///     Checked version of System.Console.WriteLine which logs I/O errors
        /// </summary>
        /// <param name="format">A format string</param>
        /// <param name="args">Optional argument array</param>
        private static void CheckedWriteLine(string format, object[]? args)
        {
            CheckEnabled();
            try
            {
                System.Console.WriteLine(format, args);
            }
            catch (Exception ex)
            {
                Logging.ExceptionWarning(_log, ex);
            }
        }

        /// <summary>
        ///     Writes to the display at the current cursor position with a terminating newline
        /// </summary>
        /// <param name="value">The value to write to the console</param>
        public static void WriteLine(string value)
        {
            CheckEnabled();
            CheckedWriteLine(value);
        }

        /// <summary>
        ///     Writes to the display at the current cursor position without a terminating newline, and then restores the cursor
        ///     position
        ///     to the location it was at prior to the write
        /// </summary>
        /// <param name="value"></param>
        public static void WriteLineRestoreCursor(string value)
        {
            CheckEnabled();
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
            CheckEnabled();
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
            CheckEnabled();
            CheckedWrite(value);
        }

        /// <summary>
        ///     Writes to the display at the current cursor position without a terminating newline and then restores the cursor to
        ///     the position it was in prior to the write
        /// </summary>
        /// <param name="value">The value to write to the console</param>
        public static void WriteRestoreCursor(string value)
        {
            CheckEnabled();
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
            CheckEnabled();
            CheckedWriteLine(format, args);
        }

        /// <summary>
        ///     Writes a formatted string to the display at the current cursor position, without a terminating newline
        /// </summary>
        /// <param name="format">A format string</param>
        /// <param name="args">The arguments to be applied to the format string</param>
        public static void WriteRestoreCursor(string format, object[]? args)
        {
            CheckEnabled();
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
            CheckEnabled();
            CheckedWrite(format, args);
        }

        /// <summary>
        ///     Deletes the line at the current cursor position
        /// </summary>
        public static void DeleteCurrentLine()
        {
            CheckEnabled();
            CheckedWrite($"{AnsiControlCodes.DeleteLine()}");
        }

        public static void EraseCurrentLine()
        {
            CheckEnabled();
            CheckedWrite($"{AnsiControlCodes.EraseCharacter(15)}");
        }
    }
}