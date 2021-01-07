using System;
using System.Collections.Generic;
using System.Reflection;
using Serilog;
using Serilog.Events;
using static JCS.Neon.Glow.Helpers.General.LogHelpers;
using static JCS.Neon.Glow.Helpers.General.ReflectionHelpers;

namespace JCS.Neon.Glow.Helpers.General
{
    /// <summary>
    /// Class containing a bunch of static methods relating to throwing/processing
    /// exceptions
    /// </summary>
    public static class ExceptionHelpers
    {
        /// <summary>
        /// Cache to store constructor information for raising new exceptionss
        /// </summary>
        private static Dictionary<Type, ConstructorInfo> _ctorCache = new();

        public static E LoggedException<E>(ILogger log, string message, Exception inner = null )
            where E : Exception
        {
            LogError(log, message);
            return (E) CreateException<E>(message, inner);
        }
    }
}