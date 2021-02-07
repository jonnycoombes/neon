namespace JCS.Neon.Glow.Utilities.Console
{
    public static class AnsiControlCodes
    {
        /// <summary>
        ///     Cursor up one
        /// </summary>
        public static readonly string CUU = $"{AsciiCodes.ESC}A";

        /// <summary>
        ///     Cursor down one
        /// </summary>
        public static readonly string CUD = $"{AsciiCodes.ESC}B";

        /// <summary>
        ///     Cursor forward (right) one
        /// </summary>
        public static readonly string CUF = $"{AsciiCodes.ESC}C";

        /// <summary>
        ///     Cursor backwards (left) one
        /// </summary>
        public static readonly string CUB = $"{AsciiCodes.ESC}D";

        /// <summary>
        ///     Reverse index
        /// </summary>
        public static readonly string RI = $"{AsciiCodes.ESC}M";

        /// <summary>
        ///     Save cursor position to memory
        /// </summary>
        public static readonly string DECSC = $"{AsciiCodes.ESC}{AsciiCodes.BEL}";

        /// <summary>
        ///     Restore cursor position from memory
        /// </summary>
        public static readonly string DECSR = $"{AsciiCodes.ESC}{AsciiCodes.BS}";

        /// <summary>
        ///     Control code for moving the cursor up by <count>
        /// </summary>
        /// <param name="count">The number of lines to move the cursor</param>
        /// <returns>An ANSI control code for moving the cursor up</returns>
        public static string CursorUp(int count)
        {
            return $"{AsciiCodes.ESC}{count}A";
        }
    }
}