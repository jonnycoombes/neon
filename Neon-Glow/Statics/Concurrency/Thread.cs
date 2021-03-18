#region

using System;

#endregion

namespace JCS.Neon.Glow.Statics.Concurrency
{
    /// <summary>
    ///     Placeholder for various threading related utilities
    /// </summary>
    public static class Thread
    {
        /// <summary>
        ///     Suspends the currently executing thread for a specified number of seconds
        /// </summary>
        /// <param name="seconds">The number of seconds to suspend for</param>
        public static void SleepCurrentThread(uint seconds)
        {
            System.Threading.Thread.Sleep((int) seconds * 1000);
        }

        /// <summary>
        ///     Suspends the current executing thread for a duration specified by a <see cref="TimeSpan" />
        /// </summary>
        /// <param name="span">A <see cref="TimeSpan" /> defining the duration of the sleep</param>
        public static void SleepCurrentThread(TimeSpan span)
        {
            System.Threading.Thread.Sleep(span.Milliseconds);
        }
    }
}