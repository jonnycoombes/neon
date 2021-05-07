/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System;
using System.Collections.Generic;
using System.Linq;
using JCS.Neon.Glow.Types;
using Serilog;

#endregion

namespace JCS.Neon.Glow.Statics.Reflection
{
    /// <summary>
    ///     Static class containing utilites relating to the activation of types
    /// </summary>
    public static class Activation
    {
        /// <summary>
        ///     Static logger
        /// </summary>
        private static readonly ILogger _log = Log.ForContext(typeof(Activation));

        /// <summary>
        ///     Searches the currently loaded assemblies for implementations of a given interface type
        /// </summary>
        /// <typeparam name="T">The type to be search for</typeparam>
        /// <returns></returns>
        public static IEnumerable<Type> LocateAllImplementors<T>()
        {
            Logging.MethodCall(_log);
            var type = typeof(T);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p));
            return types;
        }

        /// <summary>
        ///     Static helper which will create *any* instance of a given type T
        /// </summary>
        /// <param name="args">The parameters to be passed through to the constructor during activation</param>
        /// <typeparam name="T">The type to create</typeparam>
        /// <returns></returns>
        public static Option<T> CreateInstance<T>(params object?[] args)
            where T : notnull
        {
            Logging.MethodCall(_log);
            try
            {
                if (!typeof(T).IsInterface)
                {
                    var instance = Activator.CreateInstance(typeof(T), args);
                    if (instance != null)
                    {
                        return new Option<T>((T) instance);
                    }

                    return Option<T>.None;
                }

                Logging.Warning(_log, "Specified type T is an interface - cannot directly instantiate");
                return Option<T>.None;
            }
            catch
            {
                Logging.Warning(_log, $"Failed to create a new instance of type {typeof(T)}");
                return Option<T>.None;
            }
        }

        /// <summary>
        ///     Method which will attempt to instantiate an instance of a given type, and then cast to type T
        /// </summary>
        /// <param name="baseType">The base type to instantiate</param>
        /// <param name="args">The constructor arguments to pass through during activation</param>
        /// <typeparam name="T">The type to cast into</typeparam>
        /// <returns></returns>
        public static Option<T> CreateAndCastInstance<T>(Type baseType, params object?[] args)
            where T : notnull
        {
            Logging.MethodCall(_log);
            try
            {
                if (baseType.IsAssignableTo(typeof(T)))
                {
                    var instance = Activator.CreateInstance(baseType, args);
                    if (instance != null)
                    {
                        return new Option<T>((T) instance);
                    }

                    return Option<T>.None;
                }

                Logging.Warning(_log, "The specified type is not assignable to required type T");
                return Option<T>.None;
            }
            catch
            {
                Logging.Warning(_log, $"Failed to create a new instance of type {typeof(T)}");
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
            Logging.MethodCall(_log);
            var instance = CreateInstance<E>(args);
            if (instance.IsSome(out var exception))
            {
                return exception;
            }

            return new InvalidOperationException($"Unable to raise the requested exception of type {typeof(E)}");
        }
    }
}