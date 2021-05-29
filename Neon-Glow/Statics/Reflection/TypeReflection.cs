/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System;
using System.Linq;
using System.Reflection;
using JCS.Neon.Glow.Types;

#endregion

namespace JCS.Neon.Glow.Statics.Reflection
{
    /// <summary>
    ///     Static methods for reflecting on given types
    /// </summary>
    public static class TypeReflection
    {
        /// <summary>
        ///     Used when comparing names
        /// </summary>
        private const StringComparison DefaultStringComparison = StringComparison.CurrentCulture;

        /// <summary>
        ///     Attempts to retrieve the information about a public property on a given type
        /// </summary>
        /// <param name="t">The <see cref="Type" /> to inspect</param>
        /// <param name="name">The name of the property to locate</param>
        /// <param name="flags">The <see cref="BindingFlags" /> to use</param>
        /// <returns>An option</returns>
        public static Option<PropertyInfo> GetProperty(Type t, string name,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static |BindingFlags.Public | BindingFlags.NonPublic)
        {
            try
            {
                var property = t.GetProperties(flags).First(p => p.Name.Equals(name));
                return Option<PropertyInfo>.FromNullable(property);
            }
            catch (Exception)
            {
                return Option<PropertyInfo>.None;
            }
        }

        /// <summary>
        ///     Checks whether a given property exists on a given type
        /// </summary>
        /// <param name="t">The <see cref="Type" />to check</param>
        /// <param name="name">The name of the property</param>
        /// <param name="flags">The <see cref="BindingFlags" /> to use</param>
        /// <returns>True if the property is present, false otherwise</returns>
        public static bool HasProperty(Type t, string name,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static |BindingFlags.Public | BindingFlags.NonPublic)
        {
            return GetProperty(t, name, flags).IsSome();
        }

        /// <summary>
        ///     Attempts to retrieve the information relating to a field on a given type <paramref name="t" />
        /// </summary>
        /// <param name="t">The <see cref="Type" /> to inspect</param>
        /// <param name="name">The name of the field to check for</param>
        /// <param name="flags">The <see cref="BindingFlags" /> to use</param>
        /// <returns></returns>
        public static Option<FieldInfo> GetField(Type t, string name,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            try
            {
                var field = t.GetFields(flags).First(f => f.Name.Equals(name));
                return Option<FieldInfo>.FromNullable(field);
            }
            catch (Exception)
            {
                return Option<FieldInfo>.None;
            }
        }

        /// <summary>
        ///     Checks whether a given type, <paramref name="t" /> has a specific field
        /// </summary>
        /// <param name="t">The <see cref="Type" /> to inspect</param>
        /// <param name="name">The name of the field</param>
        /// <param name="flags">The <see cref="BindingFlags" /> to use</param>
        /// <returns></returns>
        public static bool HasField(Type t, string name,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            return GetField(t, name, flags).IsSome();
        }
    }
}