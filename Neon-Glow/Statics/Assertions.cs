/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System;
using Serilog;

#endregion

namespace JCS.Neon.Glow.Statics
{
    /// <summary>
    ///     Class which contains a number of potentially useful assertion utilities
    /// </summary>
    public static class Assertions
    {
        /// <summary>
        ///     Static <see cref="ILogger" /> instance used for dumping all assetion-related error messages
        /// </summary>
        private static readonly ILogger _log = Log.ForContext(typeof(Assertions));

        /// <summary>
        ///     Boolean assertion which checks whether a given assertion is true, and if not logs and then throws an exception of a
        ///     specific type
        /// </summary>
        /// <param name="assertion">The simple boolean expression to assert</param>
        /// <param name="message">The message to be logged/wrapped in the resultant exception</param>
        /// <typeparam name="E">The type of exception to throw</typeparam>
        /// <exception cref="E">Thrown if the assertion is false</exception>
        public static void Checked<E>(bool assertion, string message)
            where E : Exception
        {
            if (!assertion)
            {
                throw Exceptions.LoggedException<E>(_log, message);
            }
        }

        /// <summary>
        ///     Boolean assertion which takes a boolean-producing lambda
        /// </summary>
        /// <param name="f">Lambda which returns a boolean value</param>
        /// <param name="message">The message to log and wrap in the exception</param>
        /// <typeparam name="E">The type of exception to throw</typeparam>
        /// <exception cref="E">Thrown if the assertion is false</exception>
        public static void Checked<E>(Func<bool> f, string message) where E : Exception
        {
            Checked<E>(f(), message);
        }
    }
}