using System;
using System.Collections.Generic;
using System.Reflection;
using Serilog;

namespace JCS.Neon.Glow.Utilities.General
{
    /// <summary>
    ///     Class containing a bunch of static methods relating to throwing/processing
    ///     exceptions
    /// </summary>
    public static class Exceptions
    {
        /// <summary>
        ///     Cache to store constructor information for raising new exceptionss
        /// </summary>
        private static Dictionary<Type, ConstructorInfo> _ctorCache = new();

        public static E LoggedException<E>(ILogger log, string message, Exception inner = null)
            where E : Exception
        {
            Logs.Error(log, message);
            return (E) Reflection.CreateException<E>(message, inner);
        }
    }
}