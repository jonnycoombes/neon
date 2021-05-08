/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System.Globalization;
using JCS.Neon.Glow.Statics.Crypto;

#endregion

namespace JCS.Neon.Glow.Types.Extensions
{
    /// <summary>
    ///     Extension methods for string go into this class
    /// </summary>
    public static class String
    {
        /// <summary>
        ///     Encodes a string to Base64
        /// </summary>
        /// <param name="s"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Base64Encode(this string s, ByteEncoding encoding = ByteEncoding.Utf8)
        {
            return Encodings.Base64Encode(s, encoding);
        }

        /// <summary>
        ///     Decodes a string from Base64
        /// </summary>
        /// <param name="s"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Base64Decode(this string s, ByteEncoding encoding = ByteEncoding.Utf8)
        {
            return Encodings.Base64Decode(s, encoding);
        }

        /// <summary>
        ///     Encodes a string to Base64
        /// </summary>
        /// <param name="s"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Base64UrlEncode(this string s, ByteEncoding encoding = ByteEncoding.Utf8)
        {
            return Encodings.Base64UrlEncode(s, encoding);
        }

        /// <summary>
        ///     Decodes a string from Base64
        /// </summary>
        /// <param name="s"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Base64UrlDecode(this string s, ByteEncoding encoding = ByteEncoding.Utf8)
        {
            return Encodings.Base64UrlDecode(s, encoding);
        }

        /// <summary>
        ///     Returns the head of a string
        /// </summary>
        /// <param name="s">The string</param>
        /// <returns>Either null or the first character of the string</returns>
        public static char? Head(this string s)
        {
            if (s.Length == 0)
            {
                return null;
            }

            return s[0];
        }

        /// <summary>
        ///     Returns the tail of a given string
        /// </summary>
        /// <param name="s">The string</param>
        /// <returns>Either null or last n-1 characters of a string</returns>
        public static string? Tail(this string s)
        {
            return s.Length == 0 ? null : s[1..];
        }

        /// <summary>
        ///     Converts a string to camel case using the current culture
        /// </summary>
        /// <param name="s">The string</param>
        /// <returns>A camel case version of the string</returns>
        public static string ToCamelCase(this string s)
        {
            return $"{s.Head()}".ToLower() + s.Tail();
        }
    }
}