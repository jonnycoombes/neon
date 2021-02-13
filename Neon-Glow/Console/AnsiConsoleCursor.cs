#region

using System.Drawing;

#endregion

namespace JCS.Neon.Glow.Console
{
    /// <summary>
    /// </summary>
    public static partial class AnsiConsole
    {
        /// <summary>
        ///     Synonymous for <see cref="AnsiConsole.TopLeft" />
        /// </summary>
        public static Point Origin = new(1, 1);

        /// <summary>
        ///     <see cref="CursorPosition" /> representing the extreme top left of the display
        /// </summary>
        public static Point TopLeft => new(1, 1);

        /// <summary>
        ///     <see cref="CursorPosition" /> representing the extreme top right of the display
        /// </summary>
        public static Point TopRight => new(1, _state.Columns);

        /// <summary>
        ///     <see cref="CursorPosition" /> representing the extreme bottom left of the display
        /// </summary>
        public static Point BottomLeft => new(_state.Rows, 1);

        /// <summary>
        ///     <see cref="CursorPosition" /> representing the extreme bottom right of the display
        /// </summary>
        public static Point BottomRight => new(_state.Rows, _state.Columns);

        /// <summary>
        ///     Creates a <see cref="Point" /> with the current cursor coordinates, translated to a 1-based coordinate system
        /// </summary>
        /// <returns>A <see cref="Point" /> containing the current cursor position (row, column)</returns>
        private static Point CurrentCursorPosition()
        {
            return new(System.Console.CursorLeft + 1, System.Console.CursorTop + 1);
        }

        /// <summary>
        ///     Pushes the current cursor position onto the cursor position stack
        /// </summary>
        private static void PushCursorPosition()
        {
            _state.PushCursorPosition(CurrentCursorPosition());
        }

        /// <summary>
        ///     Pops and discards a cursor position from the cursor position stack
        /// </summary>
        private static void PopCursorPosition()
        {
            _state.PopCursorPosition();
        }

        /// <summary>
        ///     Pops a <see cref="CursorPosition" /> off the stack, and then sets the display cursor position based on it.  If there are no
        ///     positions on the stack, then this a NOOP
        /// </summary>
        private static void RestoreCursorPosition()
        {
            if (_state.PopCursorPosition().IsSome(out var p))
            {
                SetCursorPosition(p.Y, p.X);
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
        ///     Sets the cursor postion to a given row and column.  Both indexes are 1-based.
        ///     TODO Bounds checking on buffer size
        /// </summary>
        /// <param name="row">The row number</param>
        /// <param name="column">The column number</param>
        public static void SetCursorPosition(int row, int column)
        {
            CheckedWrite(AnsiControlCodes.CursorPosition(row, column));
        }
    }
}