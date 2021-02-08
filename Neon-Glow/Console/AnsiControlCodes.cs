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
            ForegroundBlack = 30,
            ForegroundRed = 31,
            ForegroundGreen = 32,
            ForegroundYellow = 33,
            ForegroundBlue = 34,
            ForegroundMagenta = 35,
            ForegroundCyan = 36,
            ForegroundWhite = 37,
            SetForegroundColour = 38,
            DefaultForegroundColour = 39,
            BackgroundBlack = 40,
            BackgroundRed = 41,
            BackgroundGreen = 42,
            BackgroundYellow = 43,
            BackgroundBlue = 44,
            BackgroundMagenta = 45,
            BackgroundCyan = 46,
            BackgroundWhite = 47,
            SetBackgroundColour = 48,
            DefaultBackgroundColour = 49,
            DisableProportionalSpacing = 50,
            Framed = 51,
            Encircled = 52,
            Overlined = 53,
            NotFramedOrCircled = 54,
            NotOverlined = 55,
            SetUnderlineColour = 58,
            DefaultUnderlineColour = 59,
            IdeogramUnderline = 60,
            IdeogramDoubleUnderline = 61,
            IdeogramOverline = 62,
            IdeogramDoubleOverline = 63,
            IdeogramStressMarking = 64,
            NoIdeogramAttrributes = 65,
            Superscript = 73,
            Subscript = 74,
            BrightForegroundBlack = 90,
            BrightForegroundRed = 91,
            BrightForegroundGreen = 92,
            BrightForegroundYellow = 93,
            BrightForegroundBlue = 94,
            BrightForegroundMagenta = 95,
            BrightForegroundCyan = 96,
            BrightForegroundWhite = 97,
            BrightBackgroundBlack = 100,
            BrightBackgroundRed = 101,
            BrightBackgroundGreen = 102,
            BrightBackgroundYellow = 103,
            BrightBackgroundBlue = 104,
            BrightBackgroundMagenta = 105,
            BrightBackgroundCyan = 106,
            BrightBackgroundWhite = 107
        }

        /// <summary>
        ///     Control Sequence Introducer
        /// </summary>
        public static readonly string CSI = $"{AsciiControlCodes.ESC}[";

        /// <summary>
        ///     Cursor up one - corresponding dyanmic function is <see cref="CursorUp" />
        /// </summary>
        public static readonly string CUU = $"{AsciiControlCodes.ESC}A";

        /// <summary>
        ///     Cursor down one - corresponding dynamic function is <see cref="CursorDown" />
        /// </summary>
        public static readonly string CUD = $"{AsciiControlCodes.ESC}B";

        /// <summary>
        ///     Cursor forward (right) one - corresponding dynamic function is <see cref="CursorForward" />
        /// </summary>
        public static readonly string CUF = $"{AsciiControlCodes.ESC}C";

        /// <summary>
        ///     Cursor backwards (left) one - corresponding dynamic function is <see cref="CursorBackward" />
        /// </summary>
        public static readonly string CUB = $"{AsciiControlCodes.ESC}D";

        /// <summary>
        ///     Cursor next line - corresponding dynamic function is <see cref="CursorNextLine" />
        /// </summary>
        public static readonly string CNL = $"{AsciiControlCodes.ESC}E";

        /// <summary>
        ///     Cursor previous line - corresponding dynamic function is <see cref="CursorPreviousLine" />
        /// </summary>
        public static readonly string CPL = $"{AsciiControlCodes.ESC}F";

        /// <summary>
        ///     Cursor horizontal absolute - corresponding dynamic function is <see cref="CursorHorizontalAbsolute" />
        /// </summary>
        public static readonly string CHA = $"{AsciiControlCodes.ESC}G";

        /// <summary>
        ///     Cursor position - corresponding dynamic functions are <see cref="CursorPosition" />, <see cref="CursorPositionRow" /> and
        ///     <see cref="CursorPositionColumn" />
        /// </summary>
        public static readonly string CUP = $"{AsciiControlCodes.ESC}H";

        /// <summary>
        ///     Scroll up - corresponding dynamic function is <see cref="ScrollUp" />
        /// </summary>
        public static readonly string SU = $"{AsciiControlCodes.ESC}S";

        /// <summary>
        ///     Scroll down - corresponding dynamic function is <see cref="ScrollDown" />
        /// </summary>
        public static readonly string SD = $"{AsciiControlCodes.ESC}T";

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
        ///     Soft reset.  This should reset the following:
        ///     <ul>
        ///         <li>Cursor visibility</li>
        ///         <li>Numeric keypad - numeric mode</li>
        ///         <li>Cursor keys - normal mode</li>
        ///         <li>Top and bottom margins</li>
        ///         <li>Character set</li>
        ///         <li>Graphics rendition (SGR off)</li>
        ///         <li>Save cursor state</li>
        ///     </ul>
        /// </summary>
        public static readonly string DECSTR = $"{CSI}!p";

        /// <summary>
        ///     Clears the entire display
        /// </summary>
        public static readonly string ClearDisplay = $"{CSI}2J";

        /// <summary>
        ///     Clears the entire display and scrollback buffer
        /// </summary>
        public static readonly string ClearDisplayClearBuffer = $"{CSI}3J";

        /// <summary>
        ///     Enable the auxillary serial port
        /// </summary>
        public static readonly string AuxPortOn = $"{CSI}5i";

        /// <summary>
        ///     Disable the auxillary serial port
        /// </summary>
        public static readonly string AuxPortOff = $"{CSI}4i";

        /// <summary>
        ///     Device status report
        /// </summary>
        public static readonly string DSR = $"{CSI}6n";

        /// <summary>
        ///     Save cursor position
        /// </summary>
        public static readonly string SCP = $"{CSI}s";

        /// <summary>
        ///     Restore cursor position
        /// </summary>
        public static readonly string RCP = $"{CSI}u";

        /// <summary>
        ///     Show the cursor
        /// </summary>
        public static readonly string ShowCursor = $"{CSI}?25h";

        /// <summary>
        ///     Hide the cursor
        /// </summary>
        public static readonly string HideCursor = $"{CSI}?25l";

        /// <summary>
        ///     Enable cursor blinking (ATT160)
        /// </summary>
        public static readonly string EnableCursorBlinking = $"{CSI}?12h";

        /// <summary>
        ///     Disable cursor blinking (ATT160)
        /// </summary>
        public static readonly string DisableCursorBlinking = $"{CSI}?12l";

        /// <summary>
        ///     Enable the alternative screen buffer
        /// </summary>
        public static readonly string EnableAlternativeScreenBuffer = $"{CSI}?1049h";

        /// <summary>
        ///     Disable the alternative screen buffer
        /// </summary>
        public static readonly string DisableAlternativeScreenBuffer = $"{CSI}?1049l";

        /// <summary>
        ///     Enable bracketed paste mode - text pasted will be surrounded by "ESC[200~" and "ESC[201~"
        /// </summary>
        public static readonly string EnableBracketedPasteMode = $"{CSI}?2004h";

        /// <summary>
        ///     Disable bracketed paste mode
        /// </summary>
        public static readonly string DisableBracketedPasteMode = $"{CSI}?2004l";

        /// <summary>
        ///     Reset the terminal state back to it's original
        /// </summary>
        public static readonly string RIS = $"{AsciiControlCodes.ESC}c";

        /// <summary>
        ///     Generates a control code for setting the foreground colour to a specific (r,g,b) triplet value
        /// </summary>
        /// <param name="r">The red channel</param>
        /// <param name="g">The green channel</param>
        /// <param name="b">The blue channel</param>
        /// <returns>An ANSI control code</returns>
        public static string SetForegroundColour(ushort r, ushort g, ushort b)
        {
            return $"{CSI}{SgrMode.SetForegroundColour};2;{r};{g};{b}m";
        }

        /// <summary>
        ///     Generates a control code for setting the background colour to a specific (r,g,b) triplet value
        /// </summary>
        /// <param name="r">The red channel</param>
        /// <param name="g">The green channel</param>
        /// <param name="b">The blue channel</param>
        /// <returns>An ANSI control code</returns>
        public static string SetBackgroundColour(ushort r, ushort g, ushort b)
        {
            return $"{CSI}{SgrMode.SetBackgroundColour};2;{r};{g};{b}m";
        }

        /// <summary>
        ///     Generates a control code for setting the current graphic rendition mode
        /// </summary>
        /// <param name="mode">The mode to apply</param>
        /// <returns>An ANSI control code</returns>
        public static string SelectGraphicRendition(SgrMode mode)
        {
            return $"{CSI}{mode}m";
        }

        /// <summary>
        ///     Generates a control code for setting the current graphic rendition mode, based on an array of different mode parameters
        /// </summary>
        /// <param name="modes">An array of modes which will be converted into a ';' delimited string</param>
        /// <returns>An ANSI control code</returns>
        public static string SelectGraphicRendition(IEnumerable<SgrMode> modes)
        {
            return $"{CSI}{modes.Select(m => m.ToString()).Aggregate((s, t) => "s;t")}m";
        }

        /// <summary>
        ///     Generates a control code for scrolling the display up <paramref name="count" /> lines
        /// </summary>
        /// <param name="count">The number of lines to scroll</param>
        /// <returns>An ANSI control code</returns>
        public static string ScrollUp(ushort count)
        {
            return $"{AsciiControlCodes.ESC}{count}S";
        }

        /// <summary>
        ///     Generates a control code for scrolling the display down <paramref name="count" /> lines
        /// </summary>
        /// <param name="count">The number of lines to scroll</param>
        /// <returns>An ANSI control code</returns>
        public static string ScrollDown(ushort count)
        {
            return $"{AsciiControlCodes.ESC}{count}S";
        }

        /// <summary>
        ///     Generates a control code for moving the cursor <paramref name="count" /> lines down
        /// </summary>
        /// <param name="count">The number of lines to move the cursor down</param>
        /// <returns>An ANSI control code</returns>
        public static string CursorNextLine(ushort count)
        {
            return $"{AsciiControlCodes.ESC}{count}E";
        }


        /// <summary>
        ///     Generates a control code for moving the cursor <paramref name="count" /> lines up
        /// </summary>
        /// <param name="count">The number of lines to move the cursor down</param>
        /// <returns>An ANSI control code</returns>
        public static string CursorPreviousLine(ushort count)
        {
            return $"{AsciiControlCodes.ESC}{count}F";
        }

        /// <summary>
        ///     Generates a control code for moving the cursor <paramref name="count" /> columns to the right
        /// </summary>
        /// <param name="count">The number of columns to move the cursor</param>
        /// <returns>An ANSI control code</returns>
        public static string CursorHorizontalAbsolute(ushort count)
        {
            return $"{AsciiControlCodes.ESC}{count}G";
        }

        /// <summary>
        ///     Generates a control code for setting the cursor position at coordinates (<paramref name="row" />, <paramref name="column" />)
        /// </summary>
        /// <param name="row">The row index for the cursor position.  The values should be 1-based, although may be zero based on certain terminals</param>
        /// <param name="column">
        ///     The column index for the cursor position.  The values should be 1-based, although may be zero-based on certain
        ///     terminals
        /// </param>
        /// <returns>An ANSI control code</returns>
        public static string CursorPosition(ushort row, ushort column)
        {
            return $"{AsciiControlCodes.ESC}{row};{column}H";
        }

        /// <summary>
        ///     Essentially a synonym for <see cref="CursorPosition" />
        /// </summary>
        /// <param name="row">The row for the cursor position</param>
        /// <param name="column">The column for the cursor position</param>
        /// <returns>An ANSI control code</returns>
        public static string HorizontalVerticalPosition(ushort row, ushort column)
        {
            return $"{AsciiControlCodes.ESC}{row};{column}f";
        }

        /// <summary>
        ///     Generates a control code for setting the cursor row position only.
        /// </summary>
        /// <param name="row">The row index</param>
        /// <returns>An ANSI control code</returns>
        public static string CursorPositionRow(ushort row)
        {
            return $"{AsciiControlCodes.ESC}{row}H";
        }

        /// <summary>
        ///     Generates a control code for setting the cursor column position only
        /// </summary>
        /// <param name="column">The column index</param>
        /// <returns>An ANSI control code</returns>
        public static string CursorPositionColumn(ushort column)
        {
            return $"{AsciiControlCodes.ESC};{column}H";
        }

        /// <summary>
        ///     Generates a control code for moving the cursor up by count
        /// </summary>
        /// <param name="count">The number of lines to move the cursor</param>
        /// <returns>An ANSI control code for moving the cursor up</returns>
        public static string CursorUp(ushort count)
        {
            return $"{AsciiControlCodes.ESC}{count}A";
        }

        /// <summary>
        ///     Generates a control code for moving the cursor down by count
        /// </summary>
        /// <param name="count">The number of lines to move the cursor</param>
        /// <returns>An ANSI control code for moving the cursor up</returns>
        public static string CursorDown(ushort count)
        {
            return $"{AsciiControlCodes.ESC}{count}B";
        }

        /// <summary>
        ///     Generates a control code for moving the cursor forward by count
        /// </summary>
        /// <param name="count">The number of lines to move the cursor</param>
        /// <returns>An ANSI control code for moving the cursor up</returns>
        public static string CursorForward(ushort count)
        {
            return $"{AsciiControlCodes.ESC}{count}C";
        }

        /// <summary>
        ///     Generates a control code for moving the cursor backward by count
        /// </summary>
        /// <param name="count">The number of lines to move the cursor</param>
        /// <returns>An ANSI control code for moving the cursor up</returns>
        public static string CursorBackward(ushort count)
        {
            return $"{AsciiControlCodes.ESC}{count}D";
        }
    }
}