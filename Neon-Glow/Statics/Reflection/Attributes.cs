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
        /// The default <see cref="BindingFlags"/> to use whilst searching for fields and properties.  This combo ensures that we scan
        /// through public/private/protected static and instance 
        /// </summary>
        private const BindingFlags DefaultFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        /// <summary>
        ///     General method that attempts to extract a single instance of an <see cref="Attribute" /> from a given type, based on the supplied
        ///     <see cref="AttributeTargets" /> value provided. By default, this method will return an <see cref="Option{T}" /> in response and will
        ///     swallow any exceptions which occur further down the stack. Note that this method will only return the <i>first</i>
        ///     instance of the attribute if found.  In order to improve performance, a static cache is maintained containing instances of
        ///     custom attributes associated with previously method calls.
        /// </summary>
        /// <param name="site">The <see cref="AttributeSite" /> relating to the attribute to retrieve</param>
        /// <param name="t">The type to inspect</param>
        /// <param name="name">The name of the site which should have the custom attribute associated with it</param>
        /// <typeparam name="T">The type of the custom <see cref="Attribute" /> to retrieve</typeparam>
        /// <returns>An <see cref="Option{T}" /></returns>
        public static Option<T> GetCustomAttribute<T>(AttributeTargets target, Type t, string? name = null)
            where T : Attribute
        {
            Logging.MethodCall(_log);
            try
            {
                #pragma warning disable 8509
                var attributes = target switch
                {
                    AttributeTargets.Class => GetCustomClassAttributes<T>(t),
                    AttributeTargets.Event => GetCustomEventAttributes<T>(t, name),
                    AttributeTargets.Field => GetCustomFieldAttributes<T>(t, name),
                    AttributeTargets.Method => GetCustomMethodAttributes<T>(t, name),
                    AttributeTargets.Property => GetCustomPropertyAttributes<T>(t, name)
                };
                #pragma warning restore 8509
                if (attributes.Length > 0)
                {
                    return Option<T>.Some(attributes[0]);
                }
            }
            catch (Exception)
            {
                Logging.Warning(_log, "An exception occurred looking for a custom attribute");
                return Option<T>.None;
            }

            return Option<T>.None;
        }

        /// <summary>
        ///     General method that attempts to extract all instances of an <see cref="Attribute" /> from a given type, based on the supplied
        ///     <see cref="AttributeTargets" /> value provided. By default, this method will return an <see cref="Option{T}" /> in response and will
        ///     swallow any exceptions which occur further down the stack. Note that this method will only return the <i>first</i>
        ///     instance of the attribute if found.  In order to improve performance, a static cache is maintained containing instances of
        ///     custom attributes associated with previously method calls.
        /// </summary>
        /// <param name="site">The <see cref="AttributeSite" /> relating to the attribute to retrieve</param>
        /// <param name="t">The type to inspect</param>
        /// <param name="name">The name of the site which should have the custom attribute associated with it</param>
        /// <typeparam name="T">The type of the custom <see cref="Attribute" /> to retrieve</typeparam>
        /// <returns>An <see cref="Option{T}" /></returns>
        public static Option<T[]> GetCustomAttributes<T>(AttributeTargets target, Type t, string? name = null)
            where T : Attribute
        {
            Logging.MethodCall(_log);
            try
            {
                #pragma warning disable 8509
                var attributes = target switch
                {
                    AttributeTargets.Class => GetCustomClassAttributes<T>(t),
                    AttributeTargets.Event => GetCustomEventAttributes<T>(t, name),
                    AttributeTargets.Field => GetCustomFieldAttributes<T>(t, name),
                    AttributeTargets.Method => GetCustomMethodAttributes<T>(t, name),
                    AttributeTargets.Property => GetCustomPropertyAttributes<T>(t, name)
                };
                #pragma warning restore 8509
                if (attributes.Length > 0)
                {
                    return Option<T[]>.Some(attributes);
                }
            }
            catch (Exception)
            {
                Logging.Warning(_log, "An exception occurred looking for a custom attribute");
                return Option<T[]>.None;
            }

            return Option<T[]>.None;
        }

        /// <summary>
        ///     Retrieves custom <see cref="Attribute" /> instances associated with a given class type
        /// </summary>
        /// <param name="t">The type to look for attributes on</param>
        /// <typeparam name="T">The type of the custom attribute</typeparam>
        /// <returns>
        ///     An array of <see cref="Attribute" /> instances of type <typeparamref name="T" />
        ///     <returns>
        ///         <exception cref="ArgumentException">Thrown if the specified type isn't actually a class</exception>
        private static T[] GetCustomClassAttributes<T>(Type t)
            where T : Attribute
        {
            Logging.MethodCall(_log);
            if (t.IsClass)
            {
                var attributes = Attribute.GetCustomAttributes(t, typeof(T));
                return (T[]) attributes;
            }

            throw new ArgumentException($"The specified target \"{t.Name}\" is not a class");
        }

        /// <summary>
        ///     Retrieve instances of a custom <see cref="Attribute" /> from a specified event defined on a type
        /// </summary>
        /// <param name="t">The type to look for the attribute on </param>
        /// <param name="name">The name of the event on <paramref name="t" /> which should have the attribute</param>
        /// <typeparam name="T">The type of the custom attribute to retrieve</typeparam>
        /// <returns>
        ///     An array of <see cref="Attribute" /> instances of type <typeparamref name="T" />
        /// </returns>
        private static T[] GetCustomEventAttributes<T>(Type t, string? name)
            where T : Attribute
        {
            Logging.MethodCall(_log);
            var target = t.GetEvents(DefaultFlags).First(e => e.Name.Equals(name));
            if (target != null)
            {
                return (T[]) Attribute.GetCustomAttributes(target, typeof(T));
            }

            throw new ArgumentException($"The specified target \"{t.Name}\" is not an event");
        }

        /// <summary>
        ///     Retrieve instances of a given <see cref="Attribute" /> on a method specified by <paramref name="name" />
        /// </summary>
        /// <param name="t">The type to look for the method on</param>
        /// <param name="name">The name of the method</param>
        /// <typeparam name="T">The type of the custom <see cref="Attribute" /> to look for</typeparam>
        /// <returns>An array of <see cref="Attribute" /> instances of type <typeparamref name="T" /></returns>
        /// <exception cref="ArgumentException">If the method can't be found</exception>
        private static T[] GetCustomMethodAttributes<T>(Type t, string? name)
            where T : Attribute
        {
            Logging.MethodCall(_log);
            var target = t.GetMethods(DefaultFlags).First(m => m.Name.Equals(name));
            if (target != null)
            {
                return (T[]) Attribute.GetCustomAttributes(target, typeof(T));
            }

            throw new ArgumentException($"The specified target \"{t.Name}\" is not an method");
        }

        /// <summary>
        ///     Retrieve instances of a given <see cref="Attribute" /> on a field specified by <paramref name="name" />
        /// </summary>
        /// <param name="t">The type to look for the field on</param>
        /// <param name="name">The name of the field to look for</param>
        /// <typeparam name="T">The type of the custom <see cref="Attribute" /> to look for</typeparam>
        /// <returns>
        ///     An array of <see cref="Attribute" /> instances of type <typeparamref name="T" />
        /// </returns>
        /// <exception cref="ArgumentException">If the field can't be found</exception>
        private static T[] GetCustomFieldAttributes<T>(Type t, string? name)
            where T : Attribute
        {
            Logging.MethodCall(_log);
            var target = t.GetFields(DefaultFlags).First(f => f.Name.Equals
            (name));
            if (target != null)
            {
                return (T[]) Attribute.GetCustomAttributes(target, typeof(T));
            }

            throw new ArgumentException($"The specified target \"{t.Name}\" is not an field");
        }

        /// <summary>
        ///     Retrieve custom attributes of type <typeparamref name="T" /> associated with a property specified by
        ///     <paramref name="name" />
        /// </summary>
        /// <param name="t">The type to look for the property on</param>
        /// <param name="name">The name of the property to look for</param>
        /// <typeparam name="T">The type of the custom <see cref="Attribute" /> to retrieve</typeparam>
        /// <returns>An array of <see cref="Atrribute" /> instances of type <typeparamref name="T" /></returns>
        /// <exception cref="ArgumentException">If the property can't be located on the parent type</exception>
        private static T[] GetCustomPropertyAttributes<T>(Type t, string? name)
            where T : Attribute
        {
            Logging.MethodCall(_log);
            var target = t.GetProperties(DefaultFlags).First(p => p.Name.Equals(name));
            if (target != null)
            {
                return (T[]) Attribute.GetCustomAttributes(target, typeof(T));
            }

            throw new ArgumentException($"The specified target \"{t.Name}\" is not an property");
        }

        /// <summary>
        ///     Checks whether a given attribute exists for a given type <paramref name="t" />
        /// </summary>
        /// <param name="target">The <see cref="AttributeTargets" /> relating to the <see cref="Attribute" /> to check for</param>
        /// <param name="t">The type to check for <see cref="Attribute" /> instances</param>
        /// <param name="name">The name of the <see cref="Attribute" /> to look for</param>
        /// <typeparam name="T">The type of the attribute to look for</typeparam>
        /// <returns>True or false</returns>
        public static bool HasCustomAttribute<T>(AttributeTargets target, Type t, string name)
            where T : Attribute
        {
            return GetCustomAttribute<T>(target, t, name).IsSome();
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
            return GetCustomAttribute<T>(AttributeTargets.Class, t).IsSome();
        }
        
    }
}