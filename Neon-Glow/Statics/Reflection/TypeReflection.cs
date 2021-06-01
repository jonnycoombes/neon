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
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            try
            {
                return Option<PropertyInfo>.FromNullable(t.GetProperty(name, flags));
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
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
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
                return Option<FieldInfo>.FromNullable(t.GetField(name, flags));
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

        /// <summary>
        ///     Attempts to retrieve the <see cref="MethodInfo" /> associated with a given <paramref name="name" /> and
        ///     <paramref name="t" />
        /// </summary>
        /// <param name="t">The type to inspect</param>
        /// <param name="name">The name of the method to look for</param>
        /// <param name="flags">The <see cref="BindingFlags" /> value to use</param>
        /// <returns></returns>
        public static Option<MethodInfo> GetMethod(Type t, string name,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            try
            {
                return Option<MethodInfo>.FromNullable(t.GetMethod(name, flags));
            }
            catch (Exception)
            {
                return Option<MethodInfo>.None;
            }
        }

        /// <summary>
        ///     Checks whether a given method exists on a type <paramref name="t" />
        /// </summary>
        /// <param name="t">The <see cref="Type" /> to inspect</param>
        /// <param name="name">T/he name of the method to check for</param>
        /// <param name="flags">The <see cref="BindingFlags" /> to use</param>
        /// <returns></returns>
        public static bool HasMethod(Type t, string name,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            return GetMethod(t, name, flags).IsSome();
        }

        /// <summary>
        ///     Attempts to retrieve the <see cref="EventInfo" /> associated with a given <paramref name="name" /> and
        ///     <paramref name="t" />
        /// </summary>
        /// <param name="t">The type to inspect</param>
        /// <param name="name">The name of the method to look for</param>
        /// <param name="flags">The <see cref="BindingFlags" /> value to use</param>
        /// <returns></returns>
        public static Option<EventInfo> GetEvent(Type t, string name,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            try
            {
                return Option<EventInfo>.FromNullable(t.GetEvent(name, flags));
            }
            catch (Exception)
            {
                return Option<EventInfo>.None;
            }
        }

        /// <summary>
        ///     Checks whether a given method exists on a type <paramref name="t" />
        /// </summary>
        /// <param name="t">The <see cref="Type" /> to inspect</param>
        /// <param name="name">T/he name of the method to check for</param>
        /// <param name="flags">The <see cref="BindingFlags" /> to use</param>
        /// <returns></returns>
        public static bool HasEvent(Type t, string name,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            return GetEvent(t, name, flags).IsSome();
        }

        /// <summary>
        ///     Filter function for use with <see cref="TypeFilter" />
        /// </summary>
        /// <param name="t">The type to check</param>
        /// <param name="criteria">The criteria object</param>
        /// <returns>true if the given type name matches the string representation of the criteria object</returns>
        public static bool TypeFilterByName(Type t, object? criteria)
        {
            if (criteria == null)
            {
                return false;
            }

            return t.Name == criteria.ToString();
        }

        /// <summary>
        ///     Checks whether a given type supports a specific interface
        /// </summary>
        /// <param name="t">The type to check</param>
        /// <typeparam name="T">The type of the interface to check for</typeparam>
        /// <returns>A truthy value</returns>
        public static bool SupportsInterface<T>(Type t)
        {
            return t.FindInterfaces(TypeFilterByName, typeof(T).Name).Any();
        }
    }
}