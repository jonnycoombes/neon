#region

using JCS.Neon.Glow.Statics.Reflection;
using Serilog;

#endregion

namespace JCS.Neon.Glow.Statics
{
    /// <summary>
    ///     Class containing a bunch of static methods relating to throwing/processing
    ///     exceptions
    /// </summary>
    public static class Exceptions
    {
        /// <summary>
        ///     Generates a new exception of a given type, and logs the supplied message as an
        ///     error first.  An optional inner exception can be supplied
        /// </summary>
        /// <param name="log">The <see cref="ILogger" /> instance to use for logging</param>
        /// <param name="message">The message to be logged, and then set as the message for the generated exception</param>
        /// <param name="inner">An optional inner exception</param>
        /// <typeparam name="E">The type of exception to be generated</typeparam>
        /// <returns>A new instance of type <see cref="E" />, derived from <see cref="System.Exception" /></returns>
        public static E LoggedException<E>(ILogger log, string message, System.Exception inner)
            where E : System.Exception
        {
            Logging.Error(log, message);
            return (E) Activation.CreateException<E>(message, inner);
        }

        /// <summary>
        ///     Generates a new exception of the given type, and logs the supplied message as an error first. No inner exception
        ///     can be supplied with this variant of the method
        /// </summary>
        /// <param name="log">The <see cref="ILogger" /> instance to log to</param>
        /// <param name="message">The message to be logged</param>
        /// <typeparam name="E">The type of the exception to be generated and returned</typeparam>
        /// <returns>A new instance of type <see cref="E" />, derived from <see cref="System.Exception" /></returns>
        public static E LoggedException<E>(ILogger log, string message)
            where E : System.Exception
        {
            Logging.Error(log, message);
            return (E) Activation.CreateException<E>(message);
        }
    }
}