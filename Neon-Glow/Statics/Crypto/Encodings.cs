#region

using System;
using Serilog;

#endregion

namespace JCS.Neon.Glow.Statics.Crypto
{
    #region Exceptions

    public class EncodingException : System.Exception
    {
        public EncodingException(string? message) : base(message)
        {
        }

        public EncodingException(string? message, System.Exception? innerException) : base(message, innerException)
        {
        }
    }

    #endregion

    /// <summary>
    ///     An enumeration of encodings used to transcode between strings and byte arrays
    /// </summary>
    public enum ByteEncoding
    {
        /// <summary>
        ///     ASCII encoding
        /// </summary>
        Ascii,

        /// <summary>
        ///     UTF-8 encoding
        /// </summary>
        Utf8,

        /// <summary>
        ///     UTF-32 encoding
        /// </summary>
        Utf32,

        /// <summary>
        ///     Unicode encoding
        /// </summary>
        Unicode,

        /// <summary>
        ///     Big-endian unicode encoding
        /// </summary>
        BigEndianUnicode,

        /// <summary>
        ///     Latin1 encoding
        /// </summary>
        Latin1
    }

    /// <summary>
    ///     Static class containing helper functions for various encodings
    /// </summary>
    public static class Encodings
    {
        /// <summary>
        ///     Padding character for use in Base64 encodings
        /// </summary>
        public const char Base64PaddingChar = '=';

        /// <summary>
        ///     Static logger
        /// </summary>
        private static readonly ILogger _log = Log.ForContext(typeof(Encodings));

        /// <summary>
        ///     Encodes a string using a given encoding
        /// </summary>
        /// <param name="source">The source string</param>
        /// <param name="encoding">The <see cref="ByteEncoding" /> to use</param>
        /// <returns>A byte array containing the encoded string</returns>
        /// <exception cref="EncodingException"></exception>
        public static byte[] StringToBytes(string source, ByteEncoding encoding = ByteEncoding.Utf8)
        {
            Logging.MethodCall(_log);
            try
            {
                return encoding switch
                {
                    ByteEncoding.Ascii => System.Text.Encoding.ASCII.GetBytes(source),
                    ByteEncoding.Utf8 => System.Text.Encoding.UTF8.GetBytes(source),
                    ByteEncoding.Utf32 => System.Text.Encoding.UTF32.GetBytes(source),
                    ByteEncoding.Unicode => System.Text.Encoding.Unicode.GetBytes(source),
                    ByteEncoding.BigEndianUnicode => System.Text.Encoding.BigEndianUnicode.GetBytes(source),
                    ByteEncoding.Latin1 => System.Text.Encoding.Latin1.GetBytes(source),
                    _ => System.Text.Encoding.UTF8.GetBytes(source)
                };
            }
            catch (System.Exception ex)
            {
                throw Exceptions.LoggedException<EncodingException>(_log,
                    "Unable to convert a string to bytes given the current encoding", ex);
            }
        }

        /// <summary>
        ///     Decodes a byte array into a string using a specified encoding
        /// </summary>
        /// <param name="source">The source to decode</param>
        /// <param name="encoding">The <see cref="ByteEncoding" /> to use</param>
        /// <returns></returns>
        public static string BytesToString(byte[] source, ByteEncoding encoding = ByteEncoding.Utf8)
        {
            Logging.MethodCall(_log);
            try
            {
                return encoding switch
                {
                    ByteEncoding.Ascii => System.Text.Encoding.ASCII.GetString(source),
                    ByteEncoding.Utf8 => System.Text.Encoding.UTF8.GetString(source),
                    ByteEncoding.Utf32 => System.Text.Encoding.UTF32.GetString(source),
                    ByteEncoding.Unicode => System.Text.Encoding.Unicode.GetString(source),
                    ByteEncoding.BigEndianUnicode => System.Text.Encoding.BigEndianUnicode.GetString(source),
                    ByteEncoding.Latin1 => System.Text.Encoding.Latin1.GetString(source),
                    _ => System.Text.Encoding.UTF8.GetString(source)
                };
            }
            catch (System.Exception ex)
            {
                throw Exceptions.LoggedException<EncodingException>(_log,
                    "Unable to convert a string to bytes given the current encoding", ex);
            }
        }

        /// <summary>
        ///     Encode in tweaked Base64 URL format
        /// </summary>
        /// <param name="source">The source to encode</param>
        /// <param name="encoding">The underlying character encoding</param>
        /// <returns></returns>
        public static string EncodeBase64Url(string source, ByteEncoding encoding = ByteEncoding.Utf8)
        {
            Logging.MethodCall(_log);
            var bytes = StringToBytes(source, encoding);
            return Convert.ToBase64String(bytes).TrimEnd(Base64PaddingChar).Replace('+', '-').Replace('/', '_');
        }

        /// <summary>
        ///     Decode form a Base64 URL format
        /// </summary>
        /// <param name="source">The encoded string</param>
        /// <param name="encoding">The underlying character encoding</param>
        /// <returns></returns>
        public static string DecodeBase64Url(string source, ByteEncoding encoding = ByteEncoding.Utf8)
        {
            Logging.MethodCall(_log);
            source = source.Replace('-', '+').Replace('_', '/');
            switch (source.Length % 4)
            {
                case 2:
                    source += "==";
                    break;
                case 3:
                    source += "=";
                    break;
            }

            var bytes = Convert.FromBase64String(source);
            return BytesToString(bytes, encoding);
        }

        /// <summary>
        ///     Encode a string as Base64 with a given character encoding
        /// </summary>
        /// <param name="source">Source string</param>
        /// <param name="encoding">The character encoding to use</param>
        /// <returns></returns>
        public static string EncodeBase64(string source, ByteEncoding encoding = ByteEncoding.Utf8)
        {
            Logging.MethodCall(_log);
            var bytes = StringToBytes(source, encoding);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        ///     Decode a string from Base64 using the given character encoding
        /// </summary>
        /// <param name="source">The source in Base64 format</param>
        /// <param name="encoding">The character encoding to use</param>
        /// <returns></returns>
        public static string DecodeBase64(string source, ByteEncoding encoding = ByteEncoding.Utf8)
        {
            Logging.MethodCall(_log);
            var bytes = Convert.FromBase64String(source);
            return BytesToString(bytes, encoding);
        }
    }
}