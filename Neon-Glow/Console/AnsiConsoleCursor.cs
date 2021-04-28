/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
﻿#region

using System;
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
        ///     <see cref="Point" /> representing the extreme top left of the display
        /// </summary>
        public static Point TopLeft => new(1, 1);

        /// <summary>
        ///     <see cref="Point" /> representing the extreme top right of the display
        /// </summary>
        public static Point TopRight => new(1, CurrentState.Columns);

        /// <summary>
        ///     <see cref="Point" /> representing the extreme bottom left of the display
        /// </summary>
        public static Point BottomLeft => new(CurrentState.Rows, 1);

        /// <summary>
        ///     <see cref="Point" /> representing the extreme bottom right of the display
        /// </summary>
        public static Point BottomRight => new(CurrentState.Rows, CurrentState.Columns);

        /// <summary>
        ///     Creates a <see cref="Point" /> with the current cursor coordinates, translated to a 1-based coordinate system
        /// </summary>
        /// <returns>A <see cref="Point" /> containing the current cursor position (row, column)</returns>
        private static Point CurrentCursorPosition()
        {
            CheckEnabled();
            return new Point(System.Console.CursorLeft + 1, System.Console.CursorTop + 1);
        }

        /// <summary>
        ///     Pushes the current cursor position onto the cursor position stack
        /// </summary>
        private static void PushCursorPosition()
        {
            CheckEnabled();
            CurrentState.PushCursorPosition(CurrentCursorPosition());
        }

        /// <summary>
        ///     Pops and discards a cursor position from the cursor position stack
        /// </summary>
        private static void PopCursorPosition()
        {
            CheckEnabled();
            CurrentState.PopCursorPosition();
        }

        /// <summary>
        ///     Pops a <see cref="Point" /> off the stack, and then sets the display cursor position based on it.  If
        ///     there are no
        ///     positions on the stack, then this a NOOP
        /// </summary>
        private static void RestoreCursorPosition()
        {
            CheckEnabled();
            if (CurrentState.PopCursorPosition().IsSome(out var p))
            {
                SetCursorPosition(p);
            }
        }

        /// <summary>
        ///     Hides the cursor
        /// </summary>
        public static void HideCursor()
        {
            CheckEnabled();
            CheckedWrite(AnsiControlCodes.HideCursor);
        }

        /// <summary>
        ///     Displays the cursor
        /// </summary>
        public static void ShowCursor()
        {
            CheckEnabled();
            CheckedWrite(AnsiControlCodes.ShowCursor);
        }

        /// <summary>
        ///     Resets the cursor to the first row, first column
        /// </summary>
        public static void ResetCursor()
        {
            CheckEnabled();
            CheckedWrite(AnsiControlCodes.CursorPosition(1, 1));
        }

        /// <summary>
        /// </summary>
        /// <param name="position"></param>
        public static void SetCursorPosition(Point position)
        {
            CheckEnabled();
            CheckBounds(position);
            CheckedWrite(AnsiControlCodes.CursorPosition(position));
        }

        /// <summary>
        ///     Checks whether a requested cursor position falls outside of the current console bounds
        /// </summary>
        /// <param name="p">A cursor position, specified as a <see cref="Point" /></param>
        private static void CheckBounds(Point p)
        {
            CheckEnabled();
            if (p.X > CurrentState.Columns || p.Y > CurrentState.Rows)
            {
                throw new AnsiConsoleCursorBoundsError("Cursor position out of bounds");
            }
        }

        /// <summary>
        ///     Exception which is thrown when positioning or write operations fall outside the current bounds of the console
        /// </summary>
        public class AnsiConsoleCursorBoundsError : AnsiConsoleException
        {
            public AnsiConsoleCursorBoundsError(string? message) : base(message)
            {
            }

            public AnsiConsoleCursorBoundsError(string? message, Exception? innerException) : base(message, innerException)
            {
            }
        }
    }
}