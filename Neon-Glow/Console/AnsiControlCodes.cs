#region

using System.Collections.Generic;
using System.Linq;
// ReSharper disable All

#endregion

namespace JCS.Neon.Glow.Console
{
    /// <summary>
    ///     Class that wraps various functions, constants and enums that are used to generate VT-100/ANSI control codes
    /// </summary>
    public static class AnsiControlCodes
    {
        /// <summary>
        ///     SGR (Select Graphic Rendition) enumeration
        /// </summary>
        public enum SgrMode
        {
            Reset = 0,
            Bold = 1,
            Faint = 2,
            Italic = 3,
            Underline = 4,
            SlowBlink = 5,
            RapidBlink = 6,
            Invert = 7,
            Conceal = 8,
            Strike = 9,
            PrimaryFont = 10,
            AlternativeFont1 = 11,
            AlternativeFont2 = 12,
            AlternativeFont3 = 13,
            AlternativeFont4 = 14,
            AlternativeFont5 = 15,
            AlternativeFont6 = 16,
            AlternativeFont7 = 17,
            AlternativeFont8 = 18,
            AlternativeFont9 = 19,
            BlackLetterFont = 20,
            DoubleUnderlined = 21,
            Normal = 22,
            NotItalicOrBlackLetter = 23,
            NotUnderlined = 24,
            NotBlinking = 25,
            ProportionalSpacing = 26,
            NotReversed = 27,
            Reveal = 28,
            NotCrossedOut = 29,
            SetDarkForegroundBlack = 30,
            SetDarkForegroundRed = 32,
            SetDarkForegroundGreen = 33,
            SetDarkForegroundYellow = 34,
            SetDarkForegroundBlue = 35,
            SetDarkForegroundMagenta = 36,
            SetDarkForegroundCyan = 37,
            SetDarkForegroundWhite = 38,
            DefaultForegroundColour = 39
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
        ///     Sets the current graphic rendition mode
        /// </summary>
        /// <param name="mode">The mode to apply</param>
        /// <returns></returns>
        public static string SelectGraphicRendition(SgrMode mode)
        {
            return $"{CSI}{mode}m";
        }

        /// <summary>
        ///     Sets the current graphic rendition mode, based on an array of different mode parameters
        /// </summary>
        /// <param name="modes">An array of modes which will be converted into a ';' delimited string</param>
        /// <returns></returns>
        public static string SelectGraphicRendition(IEnumerable<SgrMode> modes)
        {
            return $"{CSI}{modes.Select(m => m.ToString()).Aggregate((s, t) => "s;t")}m";
        }

        /// <summary>
        ///     Generates a control code for moving the cursor up by count
        /// </summary>
        /// <param name="count">The number of lines to move the cursor</param>
        /// <returns>An ANSI control code for moving the cursor up</returns>
        public static string CursorUp(int count)
        {
            return $"{AsciiControlCodes.ESC}{count}A";
        }

        /// <summary>
        ///     Generates a control code for moving the cursor down by count
        /// </summary>
        /// <param name="count">The number of lines to move the cursor</param>
        /// <returns>An ANSI control code for moving the cursor up</returns>
        public static string CursorDown(int count)
        {
            return $"{AsciiControlCodes.ESC}{count}B";
        }

        /// <summary>
        ///     Generates a control code for moving the cursor forward by count
        /// </summary>
        /// <param name="count">The number of lines to move the cursor</param>
        /// <returns>An ANSI control code for moving the cursor up</returns>
        public static string CursorForward(int count)
        {
            return $"{AsciiControlCodes.ESC}{count}C";
        }

        /// <summary>
        ///     Generates a control code for moving the cursor backward by count
        /// </summary>
        /// <param name="count">The number of lines to move the cursor</param>
        /// <returns>An ANSI control code for moving the cursor up</returns>
        public static string CursorBackward(int count)
        {
            return $"{AsciiControlCodes.ESC}{count}D";
        }
    }
}