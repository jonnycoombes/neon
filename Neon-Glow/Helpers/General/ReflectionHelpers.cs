using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JCS.Neon.Glow.Types;
using Serilog;
using static JCS.Neon.Glow.Helpers.General.LogHelpers;

namespace JCS.Neon.Glow.Helpers.General
{
    /// <summary>
    ///     Static class containing useful reflection methods and helpers
    /// </summary>
    public static class ReflectionHelpers
    {
        /// <summary>
        ///     The logger
        /// </summary>
        private static readonly ILogger _log = Log.ForContext(typeof(ReflectionHelpers));
        
        /// <summary>
        /// Searches the currently loaded assemblies for implementations of a given interface type
        /// </summary>
        /// <typeparam name="T">The type to be search for</typeparam>
        /// <returns></returns>
        public static IEnumerable<Type> LocateAllImplementors<T>()
        {
            LogMethodCall(_log);
            var type = typeof(T);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p));
            return types;
        }
        
        /// <summary>
        /// Static class
        /// </summary>
        /// <param name="args"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Option<T> CreateInstance<T>(params object?[] args)
        {
            LogMethodCall(_log);
            try
            {
                return new Option<T>((T) Activator.CreateInstance(typeof(T), args));
            }
            catch
            {
                _log.Warning($"Failed to create a new instance of type {typeof(T)}");
                return Option<T>.None;
            }
        }

        /// <summary>
        ///     Static helper for the dynamic creation of Exception-derived instances
        /// </summary>
        /// <param name="args">The arguments to be passed through to the constructor</param>
        /// <typeparam name="E">A type parameter, derived from <see cref="Exception" /></typeparam>
        /// <returns></returns>
        public static Exception CreateException<E>(params object?[] args)
            where E : Exception
        {
            LogMethodCall(_log);
            var instance = CreateInstance<E>(args);
            E exception;
            if (instance.IsSome(out exception))
                return exception;
            return new InvalidOperationException($"Unable to raise the requested exception of type {typeof(E)}");
        }

        /// <summary>
        /// Returns a string containing the version of the currently executing assembly
        /// </summary>
        /// <returns></returns>
        public static string GetApplicationAssemblyVersion(bool includeAssemblyName = false)
        {
            LogMethodCall(_log);
            var assembly = Assembly.GetEntryAssembly();
            if (includeAssemblyName)
            {
                return $"{assembly?.FullName} - {assembly?.GetName().Version}";
            }
            else
            {
                return $"{assembly?.GetName().Version}";
            }
        }
    }
}