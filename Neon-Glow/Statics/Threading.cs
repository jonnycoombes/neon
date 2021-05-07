#region

using System;
using System.Threading;

#endregion

namespace JCS.Neon.Glow.Statics
{
    /// <summary>
    ///     Placeholder for various threading related utilities
    /// </summary>
    public static class Threading
    {
        /// <summary>
        ///     Suspends the currently executing thread for a specified number of seconds
        /// </summary>
        /// <param name="seconds">The number of seconds to suspend for</param>
        public static void SleepCurrentThread(uint seconds)
        {
            Thread.Sleep((int) seconds * 1000);
        }

        /// <summary>
        ///     Suspends the current executing thread for a duration specified by a <see cref="TimeSpan" />
        /// </summary>
        /// <param name="span">A <see cref="TimeSpan" /> defining the duration of the sleep</param>
        public static void SleepCurrentThread(TimeSpan span)
        {
            Thread.Sleep(span.Milliseconds);
        }
    }
}