using System;
using System.Text;

namespace JCS.Neon.Glow.Helpers.Crypto
{
    
    #region Exceptions

    public class EncodingHelperException : Exception
    {
        public EncodingHelperException(string? message) : base(message)
        {
        }

        public EncodingHelperException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
    #endregion

    /// <summary>
    /// An enumeration of encodings used to transcode between strings and byte arrays
    /// </summary>
    public enum ByteEncoding
    {
        /// <summary>
        /// ASCII encoding
        /// </summary>
        Ascii,

        /// <summary>
        /// UTF-8 encoding
        /// </summary>
        Utf8,

        /// <summary>
        /// UTF-32 encoding
        /// </summary>
        Utf32,
        
        /// <summary>
        /// Unicode encoding
        /// </summary>
        Unicode,
        
        /// <summary>
        /// Big-endian unicode encoding
        /// </summary>
        BigEndianUnicode,
        
        /// <summary>
        /// Latin1 encoding
        /// </summary>
        Latin1
    }

    /// <summary>
    /// Static class containing helper functions for various encodings
    /// </summary>
    public class EncodingHelpers
    {
        public const char Base64Padding = '=';
        
        /// <summary>
        /// Encodes a string using a given encoding
        /// </summary>
        /// <param name="source">The source string</param>
        /// <param name="encoding">The <see cref="ByteEncoding"/> to use</param>
        /// <returns>A byte array containing the encoded string</returns>
        /// <exception cref="EncodingHelperException"></exception>
        public static byte[] StringToBytes(string source, ByteEncoding encoding = ByteEncoding.Utf8)
        {
            try
            {
                switch (encoding)
                {
                    case ByteEncoding.Ascii:
                        return Encoding.ASCII.GetBytes(source);
                    case ByteEncoding.Utf8:
                        return Encoding.UTF8.GetBytes(source);
                    case ByteEncoding.Utf32:
                        return Encoding.UTF32.GetBytes(source);
                    case ByteEncoding.Unicode:
                        return Encoding.Unicode.GetBytes(source);
                    case ByteEncoding.BigEndianUnicode:
                        return Encoding.BigEndianUnicode.GetBytes(source);
                    case ByteEncoding.Latin1:
                        return Encoding.Latin1.GetBytes(source);
                    default:
                        return Encoding.UTF8.GetBytes(source);
                }
            }
            catch (Exception ex)
            {
                throw new EncodingHelperException("Unable to convert a string to bytes given the current encoding", ex);
            }
        }

        /// <summary>
        /// Decodes a byte array into a string using a specified encoding
        /// </summary>
        /// <param name="source">The source to decode</param>
        /// <param name="encoding">The <see cref="ByteEncoding"/> to use</param>
        /// <returns></returns>
        public static string BytesToString(byte[] source, ByteEncoding encoding = ByteEncoding.Utf8)
        {
            try
            {
                switch (encoding)
                {
                    case ByteEncoding.Ascii:
                        return Encoding.ASCII.GetString(source);
                    case ByteEncoding.Utf8:
                        return Encoding.UTF8.GetString(source);
                    case ByteEncoding.Utf32:
                        return Encoding.UTF32.GetString(source);
                    case ByteEncoding.Unicode:
                        return Encoding.Unicode.GetString(source);
                    case ByteEncoding.BigEndianUnicode:
                        return Encoding.BigEndianUnicode.GetString(source);
                    case ByteEncoding.Latin1:
                        return Encoding.Latin1.GetString(source);
                    default:
                        return Encoding.UTF8.GetString(source);

                }
            }
            catch (Exception ex)
            {
                throw new EncodingHelperException("Unable to convert a string to byte array using the specified encoding", ex);
            }
        }
        
        /// <summary>
        /// Encode in tweaked Base64 URL format
        /// </summary>
        /// <param name="source">The source to encode</param>
        /// <param name="encoding">The underlying character encoding</param>
        /// <returns></returns>
        public static string EncodeBase64Url(string source, ByteEncoding encoding = ByteEncoding.Utf8)
        {
            var bytes = StringToBytes(source, encoding);
            return Convert.ToBase64String(bytes).TrimEnd(Base64Padding).Replace('+','-').Replace('/','_');
        }

        /// <summary>
        /// Decode form a Base64 URL format
        /// </summary>
        /// <param name="source">The encoded string</param>
        /// <param name="encoding">The underlying character encoding</param>
        /// <returns></returns>
        public static string DecodeBase64Url(string source, ByteEncoding encoding = ByteEncoding.Utf8)
        {
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
        /// Encode a string as Base64 with a given character encoding
        /// </summary>
        /// <param name="source">Source string</param>
        /// <param name="encoding">The character encoding to use</param>
        /// <returns></returns>
        public static string EncodeBase64(string source, ByteEncoding encoding = ByteEncoding.Utf8)
        {
            var bytes = StringToBytes(source, encoding);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Decode a string from Base64 using the given characted encoding
        /// </summary>
        /// <param name="source">The source in Base64 format</param>
        /// <param name="encoding">The character encoding to use</param>
        /// <returns></returns>
        public static string DecodeBase64(string source, ByteEncoding encoding = ByteEncoding.Utf8)
        {
            var bytes = Convert.FromBase64String(source);
            return BytesToString(bytes, encoding);
        }
        
    }
}