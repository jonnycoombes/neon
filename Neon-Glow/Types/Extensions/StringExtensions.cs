using System;
using JCS.Neon.Glow.Helpers.Crypto;
using static JCS.Neon.Glow.Helpers.Crypto.EncodingHelpers;

namespace JCS.Neon.Glow.Types.Extensions
{
    /// <summary>
    /// Extension methods for string go into this class
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Encodes a string to Base64
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Base64Encode(this string s, ByteEncoding encoding = ByteEncoding.Utf8)
        {
            return EncodeBase64(s, encoding);
        }

        /// <summary>
        /// Decodes a string from Base64
        /// </summary>
        /// <param name="s"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Base64Decode(this string s, ByteEncoding encoding = ByteEncoding.Utf8)
        {
            return DecodeBase64(s, encoding);
        }

        /// <summary>
        /// Encodes a string to Base64
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Base64UrlEncode(this string s, ByteEncoding encoding = ByteEncoding.Utf8)
        {
            return EncodeBase64Url(s, encoding);
        }

        /// <summary>
        /// Decodes a string from Base64
        /// </summary>
        /// <param name="s"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Base64UrlDecode(this string s, ByteEncoding encoding = ByteEncoding.Utf8)
        {
            return DecodeBase64Url(s, encoding);
        }

        /// <summary>
        /// Returns the head of a string
        /// </summary>
        /// <param name="s">The string</param>
        /// <returns>Either null or the first character of the string</returns>
        public static char? Head(this string s)
        {
            if (s.Length == 0) return null;
            return s[0];
        }

        /// <summary>
        /// Returns the tail of a given string
        /// </summary>
        /// <param name="s">The string</param>
        /// <returns>Either null or last n-1 characters of a string</returns>
        public static string? Tail(this string s)
        {
            if (s.Length == 0) return null;
            return s[1..^0];
        }
    }
}