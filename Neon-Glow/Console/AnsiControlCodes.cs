using System.Linq;

namespace JCS.Neon.Glow.Console
{
    public static class AnsiControlCodes
    {
        public enum SgrMode
        {
            
        }
        
        /// <summary>
        ///     Control Sequence Introducer
        /// </summary>
        public static readonly string CSI = $"{AsciiControlCodes.ESC}[";

        /// <summary>
        ///     Cursor up one
        /// </summary>
        public static readonly string CUU = $"{AsciiControlCodes.ESC}A";

        /// <summary>
        ///     Cursor down one
        /// </summary>
        public static readonly string CUD = $"{AsciiControlCodes.ESC}B";

        /// <summary>
        ///     Cursor forward (right) one
        /// </summary>
        public static readonly string CUF = $"{AsciiControlCodes.ESC}C";

        /// <summary>
        ///     Cursor backwards (left) one
        /// </summary>
        public static readonly string CUB = $"{AsciiControlCodes.ESC}D";

        /// <summary>
        ///     Reverse index
        /// </summary>
        public static readonly string RI = $"{AsciiControlCodes.ESC}M";

        /// <summary>
        ///     Save cursor position to memory
        /// </summary>
        public static readonly string DECSC = $"{AsciiControlCodes.ESC}{AsciiControlCodes.BEL}";

        /// <summary>
        ///     Restore cursor position from memory
        /// </summary>
        public static readonly string DECSR = $"{AsciiControlCodes.ESC}{AsciiControlCodes.BS}";

        /// <summary>
        ///     Clears to the end of the display from the current cursor position
        /// </summary>
        public static readonly string ClearToEnd = $"{CSI}0J";

        /// <summary>
        ///     Clears from the start of the display to the current cursor position
        /// </summary>
        public static readonly string ClearToCursor = $"{CSI}1J";

        /// <summary>
        ///     Clears the entire display
        /// </summary>
        public static readonly string ClearDisplay = $"{CSI}2J";

        /// <summary>
        ///     Clears the entire display and scrollback buffer
        /// </summary>
        public static readonly string ClearDisplayClearBuffer = $"{CSI}3J";

        /// <summary>
        /// Sets the current graphic rendition mode
        /// </summary>
        /// <param name="mode">The mode to apply</param>
        /// <returns></returns>
        public static string SelectGraphicRendition(int mode)
        {
            return $"{AnsiControlCodes.CSI}{mode}m";
        }

        /// <summary>
        /// Sets the current graphic rendition mode, based on an array of different mode parameters
        /// </summary>
        /// <param name="modes">An array of modes which will be converted into a ';' delimited string</param>
        /// <returns></returns>
        public static string SelectGraphicRendition(int[] modes)
        {
            return $"{AnsiControlCodes.CSI}{modes.Select(m => m.ToString()).Aggregate((s, t) => "s;t")}m";
        }

        /// <summary>
        ///     Generates a control code for moving the cursor up by <count>
        /// </summary>
        /// <param name="count">The number of lines to move the cursor</param>
        /// <returns>An ANSI control code for moving the cursor up</returns>
        public static string CursorUp(int count)
        {
            return $"{AsciiControlCodes.ESC}{count}A";
        }

        /// <summary>
        ///     Generates a control code for moving the cursor down by <count>
        /// </summary>
        /// <param name="count">The number of lines to move the cursor</param>
        /// <returns>An ANSI control code for moving the cursor up</returns>
        public static string CursorDown(int count)
        {
            return $"{AsciiControlCodes.ESC}{count}B";
        }

        /// <summary>
        ///     Generates a control code for moving the cursor forward by <count>
        /// </summary>
        /// <param name="count">The number of lines to move the cursor</param>
        /// <returns>An ANSI control code for moving the cursor up</returns>
        public static string CursorForward(int count)
        {
            return $"{AsciiControlCodes.ESC}{count}C";
        }

        /// <summary>
        ///     Generates a control code for moving the cursor backward by <count>
        /// </summary>
        /// <param name="count">The number of lines to move the cursor</param>
        /// <returns>An ANSI control code for moving the cursor up</returns>
        public static string CursorBackward(int count)
        {
            return $"{AsciiControlCodes.ESC}{count}D";
        }
    }
}