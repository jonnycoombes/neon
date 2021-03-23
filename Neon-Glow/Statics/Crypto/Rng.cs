#region

using System;
using System.Collections.Generic;
using Serilog;

#endregion

namespace JCS.Neon.Glow.Statics.Crypto
{
    /// <summary>
    ///     Class containing utilities for the generation of random numbers
    /// </summary>
    public static class Rng
    {
        /// <summary>
        ///     Static logger
        /// </summary>
        private static readonly ILogger _log = Log.ForContext(typeof(Rng));

        /// <summary>
        ///     Internal RNG instance
        /// </summary>
        private static readonly Random _rng = new();

        /// <summary>
        ///     Returns a random integer in the range -1 >= n >= -INTMAX
        /// </summary>
        /// <param name="max">The maximum value for the number generated</param>
        /// <returns></returns>
        public static int NonZeroNegativeInteger(int max)
        {
            Logging.Logging.MethodCall(_log);
            return -_rng.Next(1, max);
        }

        /// <summary>
        ///     Returns a random integer in the range 1 <= n <= INTMAX
        /// </summary>
        /// <param name="max">The maximum value for the number generated</param>
        /// <returns></returns>
        public static int NonZeroPositiveInteger(int max)
        {
            Logging.Logging.MethodCall(_log);
            return _rng.Next(1, max);
        }

        /// <summary>
        ///     Generates a sequence of random integers within a given bounded range.
        /// </summary>
        /// <param name="count">The number of integers to generate</param>
        /// <param name="min">The minimum value for any generated integers</param>
        /// <param name="max">The maximum value for any generated integers</param>
        /// <returns>An <see cref="IEnumerable{T}" /> which can be used to iterate over the generated integers</returns>
        public static IEnumerable<int> BoundedSequence(uint count, int min, int max)
        {
            Logging.Logging.MethodCall(_log);
            while (count != 0)
            {
                yield return _rng.Next(min, max);
                count--;
            }
        }

        /// <summary>
        ///     Generates a sequence of random doubles scaled by a supplied factor.
        /// </summary>
        /// <param name="count">The number of doubles to generate</param>
        /// <
        /// <param name="scale">Each generated double will be scaled by this value </param>
        /// <returns>An <see cref="IEnumerable{T}" /> which can be used to iterate over the generated doubles</returns>
        public static IEnumerable<double> BoundedSequence(uint count, double scale)
        {
            Logging.Logging.MethodCall(_log);
            while (count != 0)
            {
                yield return _rng.NextDouble() * scale;
                count--;
            }
        }

        /// <summary>
        ///     Generates a random sequence of byte values
        /// </summary>
        /// <param name="count">The number of generated bytes</param>
        /// <returns>An <see cref="IEnumerable{T}" /> which can be used to iterate over the generated bytes</returns>
        public static IEnumerable<byte> BoundedSequence(uint count)
        {
            Logging.Logging.MethodCall(_log);
            while (count != 0)
            {
                var output = new byte[1];
                _rng.NextBytes(output);
                yield return output[0];
                count--;
            }
        }
    }
}