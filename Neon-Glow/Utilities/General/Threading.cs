using System.Threading;

namespace JCS.Neon.Glow.Utilities.General
{
    /// <summary>
    /// Placeholder for various threading related utilities
    /// </summary>
    public static class Threading
    {
        /// <summary>
        /// Suspends the currently executing thread for a specified number of seconds
        /// </summary>
        /// <param name="seconds">The number of seconds to suspend for</param>
        public static void SleepCurrentThread(uint seconds)
        {
            Thread.Sleep((int)seconds * 1000);
        }
    }
}