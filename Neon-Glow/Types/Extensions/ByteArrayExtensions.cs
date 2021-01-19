using System;
using System.Security.Cryptography;

namespace JCS.Neon.Glow.Types.Extensions
{
    /// <summary>
    /// Useful extension methods for byte[] 
    /// </summary>
    public static class ByteArrayExtensions
    {
        /// <summary>
        /// Resizes an array, and concatenates a separate array onto the end of the original array
        /// </summary>
        /// <param name="dest">The original array</param>
        /// <param name="src">The array to appen</param>
        /// <returns>The resized array (this is an extension method)</returns>
        /// <exception cref="ArgumentException">If the source array is of zero length</exception>
        public static byte[] Concatenate(this byte[]? dest, byte[]? src)
        {
            if (dest == null) throw new ArgumentException("This operation cannot be applied to a null array");
            if (src == null || src.Length == 0) throw new ArgumentException("Source array is null or of zero-length");
            Array.Resize(ref dest, dest.Length + src.Length);
            Array.Copy(src, 0, dest, dest.Length - src.Length, src.Length);
            return dest;
        }

        /// <summary>
        /// Populates the array with a cryptographically generated sequence of random bytes
        /// </summary>
        /// <param name="dest">The source array</param>
        /// <param name="nonZero">Whether or not non-zero bytes should be used in the randomisation</param>
        /// <returns>The original array (this is an extension method)</returns>
        public static byte[] Randomise(this byte[]? dest, bool nonZero = false)
        {
            if (dest == null || dest.Length == 0) throw new ArgumentException("Zero-length or null array cannot be randomised");
            using (var rng = new RNGCryptoServiceProvider())
            {
                if (nonZero)
                    rng.GetNonZeroBytes(dest);
                else
                    rng.GetBytes(dest);
            }

            return dest;
        }

        /// <summary>
        /// Returns the head of the array
        /// </summary>
        /// <param name="arr">The array</param>
        /// <returns>First byte or null</returns>
        public static byte? Head(this byte[] arr)
        {
            if (arr.Length == 0) return null;
            return arr[0];
        }

        /// <summary>
        /// Returns the tail of the array
        /// </summary>
        /// <param name="arr">The array</param>
        /// <returns>Either null or last n-1 elements</returns>
        public static byte[]? Tail(this byte[] arr)
        {
            if (arr.Length == 0) return null;
            return arr[1..^0];
        }
    }
}