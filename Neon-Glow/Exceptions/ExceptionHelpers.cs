#region

using System;
using JCS.Neon.Glow.Logging;
using Serilog;

#endregion

namespace JCS.Neon.Glow.Exceptions
{
    /// <summary>
    ///     Class containing a bunch of static methods relating to throwing/processing
    ///     exceptions
    /// </summary>
    public static class ExceptionHelpers
    {
        /// <summary>
        ///     Generates a new exception of a given type, and logs the supplied message as an
        ///     error first.  An optional inner exception can be supplied
        /// </summary>
        /// <param name="log">The <see cref="ILogger" /> instance to use for logging</param>
        /// <param name="message">The message to be logged, and then set as the message for the generated exception</param>
        /// <param name="inner">An optional inner exception</param>
        /// <typeparam name="E">The type of exception to be generated</typeparam>
        /// <returns>A new instance of type E, derived from <see cref="Exception" /></returns>
        public static E LoggedException<E>(ILogger log, string message, Exception inner = null!)
            where E : Exception
        {
            LogHelpers.Error(log, message);
            return (E) Reflection.ReflectionHelpers.CreateException<E>(message, inner);
        }
    }
}