/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
﻿#region

using System.Collections.Generic;
using System.Drawing;
using JCS.Neon.Glow.Resources;
using JCS.Neon.Glow.Types;

#endregion

namespace JCS.Neon.Glow.Console
{
    /// <summary>
    ///     <para>
    ///         Class used to track the current state of the <see cref="AnsiConsole" />.  Whenever possible, the console tries
    ///         to
    ///         keep its internal state up to date and current with the state of the underlying <see cref="System.Console" />.
    ///         The
    ///         state of
    ///         the <see cref="AnsiConsole" /> is re-entrant and state updates are synchronised on a global lock object.  Note
    ///         that
    ///         changes to the currently active state object do not directly affect the actual state of the console, unless
    ///         <see cref="AnsiConsole.ApplyState" /> is called explicitly.  Background state update tasks are used to maintain
    ///         an
    ///         accurate view of the console state, along with explicit side-effects during console manipulation methods.
    ///     </para>
    ///     <para>
    ///         At any one time, the static instance of <see cref="AnsiConsole" /> maintains two
    ///         <see cref="AnsiConsoleState" /> instances,
    ///         one for the primary buffer, and one for the secondary buffer.
    ///     </para>
    /// </summary>
    public class AnsiConsoleState
    {
        /// <summary>
        ///     Stack used to keep track of cursor instances positions
        /// </summary>
        private readonly Stack<Point> _cursorPositionStack = new();

        /// <summary>
        ///     Used to control concurrent modifications of state
        /// </summary>
        private readonly object _stateLock = new();

        /// <summary>
        ///     Backing variable for tracking the current buffer height in rows
        /// </summary>
        private int _bufferHeight;

        /// <summary>
        ///     Backing variable for tracking the current buffer width in columns
        /// </summary>
        private int _bufferWidth;

        /// <summary>
        ///     Backing variable for tracking the current console window title
        /// </summary>
        private string _title = Messages.ConsoleDefaultTitle;

        /// <summary>
        ///     Getter/setter for the number of rows in the buffer
        /// </summary>
        /// <exception cref="AnsiConsoleException">Thrown if an attempt is made to update the number of rows to less than 1</exception>
        public int Rows
        {
            get => _bufferHeight;
            set
            {
                if (value > 0)
                {
                    lock (_stateLock)
                    {
                        _bufferHeight = value;
                    }
                }
                else
                {
                    throw new AnsiConsole.AnsiConsoleException("Number of rows cannot be less than 1");
                }
            }
        }

        /// <summary>
        ///     Getter/setter for the number of columns in the buffer
        /// </summary>
        /// <exception cref="AnsiConsoleException">Thrown if an attempt is made to update the number of columns to less than 1</exception>
        public int Columns
        {
            get => _bufferWidth;
            set
            {
                if (value > 0)
                {
                    lock (_stateLock)
                    {
                        _bufferWidth = value;
                    }
                }
                else
                {
                    throw new AnsiConsole.AnsiConsoleException("Number of columns cannot be less than 1");
                }
            }
        }

        /// <summary>
        ///     Getter/setter for the current console title
        /// </summary>
        public string Title
        {
            get => _title;
            set
            {
                lock (_stateLock)
                {
                    _title = value;
                }
            }
        }

        /// <summary>
        ///     Pushes a cursor position onto the stack
        /// </summary>
        /// <param name="position">A cursor position, encoded as a <see cref="Point" /> struct</param>
        public void PushCursorPosition(Point position)
        {
            lock (_cursorPositionStack)
            {
                _cursorPositionStack.Push(position);
            }
        }

        /// <summary>
        ///     Tries to pop a cursor position off the stack
        /// </summary>
        /// <returns>
        ///     An <see cref="Option{T}" />.  The option will contain a value if there is a position available on the stack,
        ///     None otherwise
        /// </returns>
        public Option<Point> PopCursorPosition()
        {
            lock (_cursorPositionStack)
            {
                if (_cursorPositionStack.TryPop(out var position))
                {
                    return Option<Point>.Some(position);
                }

                return Option<Point>.None;
            }
        }
    }
}