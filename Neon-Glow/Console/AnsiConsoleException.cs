using System;

namespace JCS.Neon.Glow.Console
{
    public static partial class AnsiConsole
    {
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