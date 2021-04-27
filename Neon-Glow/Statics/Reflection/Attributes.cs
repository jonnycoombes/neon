#region

using System;
using JCS.Neon.Glow.Types;
using Serilog;

#endregion

namespace JCS.Neon.Glow.Statics.Reflection
{
    /// <summary>
    ///     Class which contains static methods for manipulation and interaction with attributes
    /// </summary>
    public static class Attributes
    {
        /// <summary>
        ///     Static logger
        /// </summary>
        private static readonly ILogger _log = Log.ForContext(typeof(Attributes));

        /// <summary>
        ///     Attempts to lookup and return a custom class-level attribute associated with a given class type
        /// </summary>
        /// <param name="t">The source type to reflect upon</param>
        /// <typeparam name="T">The target type of the custom attribute to retrieve</typeparam>
        /// <returns>An option which may or may contain the custom attribute.</returns>
        public static Option<Attribute> GetCustomClassAttribute<T>(Type t)
            where T : Attribute
        {
            Logging.MethodCall(_log);
            var attribute = Attribute.GetCustomAttribute(t, typeof(T));
            return new Option<Attribute>(attribute);
        }

        /// <summary>
        ///     Checks whether a given class type has a specific custom attribute applied to it
        /// </summary>
        /// <param name="t">The class type to check</param>
        /// <typeparam name="T">The type of the attribute to look for</typeparam>
        /// <returns><code>true</code> if the attribute is applied to the source class type, <code>false</code> otherwise</returns>
        public static bool HasCustomClassAttribute<T>(Type t)
            where T : Attribute
        {
            return GetCustomClassAttribute<T>(t).IsSome();
        }

        /// <summary>
        ///     Attempts to lookup and return multiple instances of a specified custom attribute for a given class type
        /// </summary>
        /// <param name="t">The source type to reflect upon</param>
        /// <typeparam name="T">The type of custom attribute to return</typeparam>
        /// <returns>An array (possibly empty) containing the specified custom attributes</returns>
        public static Attribute[] GetCustomClassAttributes<T>(Type t)
            where T : Attribute
        {
            Logging.MethodCall(_log);
            return Attribute.GetCustomAttributes(t, typeof(T));
        }
    }
}