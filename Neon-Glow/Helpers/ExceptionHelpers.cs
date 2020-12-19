using System;
using System.Collections.Generic;
using System.Reflection;
using Serilog;
using Serilog.Events;
using static JCS.Neon.Glow.Helpers.LogHelpers;
using static JCS.Neon.Glow.Helpers.ReflectionHelpers;

namespace JCS.Neon.Glow.Helpers
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
        private static Dictionary<Type, ConstructorInfo> _ctorCache= new ();
        
        public static E LoggedException<E>(ILogger log, string message, Exception inner= null, LogEventLevel level=LogEventLevel.Error)
            where E : Exception
        {
            LogAtLevel(log, message, level);
            return (E)CreateException<E>(message, inner);
        }
    }
}