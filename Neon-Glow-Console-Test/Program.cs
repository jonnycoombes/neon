using System.Threading;

namespace JCS.Neon.Glow.Console.Test
{
    internal static class Program
    {

        /// <summary>
        /// Convenience function which just suspends the current thread for a specified number of seconds
        /// </summary>
        /// <param name="seconds"></param>
        private static void SleepCurrentThread(int seconds)
        {
            if (seconds > 0)
            {
                Thread.Sleep(seconds * 1000);
            }
        }
        
        /// <summary>
        /// Main entry point for the test suite
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            AnsiConsole.WriteLine("Starting Neon Glow AnsiConsole Test Suite");
            BasicTests.CursorPositionTests();
            BasicTests.BufferStressTest();
            System.Console.ReadKey();
        }
    }
}