using System;
using System.Linq;
using System.Threading;
using JCS.Neon.Glow.Concurrency;
using JCS.Neon.Glow.Cryptography;

namespace JCS.Neon.Glow.Console.Test
{
    /// <summary>
    ///     Basic test cases for the Neon Glow ANSI console
    /// </summary>
    public static class BasicTests
    {
        /// <summary>
        /// Interval of pause between tests in seconds
        /// </summary>
        private const int TestInterval = 2;
        
        public static void CursorPositionTests()
        {
            // resetting the cursor
            AnsiConsole.ClearDisplay(true);
            AnsiConsole.WriteLine($"Current dimensions are ({AnsiConsole.Width}, {AnsiConsole.Height})");
            
            // positioning the cursor
            ThreadHelpers.SleepCurrentThread(TestInterval);
            AnsiConsole.HideCursor();
            var colcoords = Rng.BoundedSequence(500, 1, AnsiConsole.Width).ToArray();
            var rowcoords = Rng.BoundedSequence(500, 1, AnsiConsole.Height).ToArray();
            
            for (ushort i = 0; i < 500; i++)
            {
                AnsiConsole.SetCursorPosition(2, 1);
                AnsiConsole.Write($"Plot coords: ({rowcoords[i]},{colcoords[i]})");
                AnsiConsole.SetCursorPosition((uint)rowcoords[i],(uint)colcoords[i]);
                AnsiConsole.Write($"{Char.ConvertFromUtf32(0x1f4a5)}");
                ThreadHelpers.SleepCurrentThread(TimeSpan.FromSeconds(0.1));
            }
            AnsiConsole.ShowCursor();
        }

        public static void BufferStressTest()
        {
            AnsiConsole.ClearDisplay(true);
            AnsiConsole.WriteLineRestoreCursor("Starting buffer height test...");
            ThreadHelpers.SleepCurrentThread(TestInterval);
            AnsiConsole.ClearDisplay(true);
            AnsiConsole.HideCursor();
            for (var i = 1; i <= 5000; i++)
            {
                AnsiConsole.WriteRestoreCursor($"{i} - {AnsiConsole.ReportCursorPositionString()}");
            }
            AnsiConsole.ShowCursor();
        }
    }
}