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

            AnsiConsole.SetTitle("Cursor Position Tests");
            
            // resetting the cursor
            AnsiConsole.ClearDisplay(true);
            AnsiConsole.WriteLine($"Current dimensions are ({AnsiConsole.Columns}, {AnsiConsole.Rows})");
            
            // positioning the cursor
            ThreadHelpers.SleepCurrentThread(TestInterval);
            AnsiConsole.HideCursor();
            var colcoords = Rng.BoundedSequence(5000, 1, AnsiConsole.Columns).ToArray();
            var rowcoords = Rng.BoundedSequence(5000, 1, AnsiConsole.Rows).ToArray();
            
            for (ushort i = 0; i < 5000; i++)
            {
                AnsiConsole.SetCursorPosition(2, 1);
                AnsiConsole.Write($"Plot coords: ({rowcoords[i]},{colcoords[i]})");
                AnsiConsole.SetCursorPosition((uint)rowcoords[i],(uint)colcoords[i]);
                AnsiConsole.Write($"{char.ConvertFromUtf32(0x1f4a5)}");
                ThreadHelpers.SleepCurrentThread(TimeSpan.FromSeconds(0.1));
            }
            AnsiConsole.ShowCursor();
            System.Console.ReadKey();
        }

        public static void BufferStressTest()
        {
            AnsiConsole.SetTitle("Buffer Stress Tests");
            
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