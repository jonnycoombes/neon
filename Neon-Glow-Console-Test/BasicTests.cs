using System.Threading;
using JCS.Neon.Glow.Concurrency;

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
        
        /// <summary>
        ///     Test that checks that clearing the screen works ok
        /// </summary>
        public static void ClearScreenTest()
        {
            // clearing and resetting the display
            AnsiConsole.WriteLine("Clearing console display and resetting cursor");
            ThreadHelpers.SleepCurrentThread(TestInterval);
            AnsiConsole.ClearDisplay(true);
            
            // clearing up to the current cursor position
            AnsiConsole.WriteLine("Clearing display to cursor");
            ThreadHelpers.SleepCurrentThread(TestInterval);
            AnsiConsole.ClearToCursor();
            
            // clearing from the current cursor position
            AnsiConsole.WriteLine("Clearing display from cursor to end");
            ThreadHelpers.SleepCurrentThread(TestInterval);
            AnsiConsole.ClearToEnd();
        }

        public static void CursorPositionTests()
        {
            // resetting the cursor
            AnsiConsole.WriteLine("Resetting the cursor to the origin");
            ThreadHelpers.SleepCurrentThread(TestInterval);
            AnsiConsole.ClearDisplay(true);
            
            // positioning the cursor
            ThreadHelpers.SleepCurrentThread(TestInterval);
            AnsiConsole.HideCursor();
            for (ushort i = 1; i <= 10; i++)
            {
                AnsiConsole.SetCursorPosition(i,i);
                AnsiConsole.ReportCursorPosition();
                ThreadHelpers.SleepCurrentThread((TestInterval/2));
                AnsiConsole.ClearDisplay(true);
            }
            AnsiConsole.ShowCursor();
        }

        public static void BufferHeightTest()
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