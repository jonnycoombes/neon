using System;
using JCS.Neon.Glow.Types;
using Serilog;

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
        ///     Static class
        /// </summary>
        /// <param name="args"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Option<T> CreateInstance<T>(params object?[] args)
        {
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
            var instance = CreateInstance<E>(args);
            E exception;
            if (instance.IsSome(out exception))
                return exception;
            return new InvalidOperationException($"Unable to raise the requested exception of type {typeof(E)}");
        }
    }
}