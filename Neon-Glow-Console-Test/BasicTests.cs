using System.Threading;
using JCS.Neon.Glow.Utilities.General;

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
            Threading.SleepCurrentThread(TestInterval);
            AnsiConsole.ClearDisplay(true);
            
            // clearing up to the current cursor position
            AnsiConsole.WriteLine("Clearing display to cursor");
            Threading.SleepCurrentThread(TestInterval);
            AnsiConsole.ClearToCursor();
            
            // clearing from the current cursor position
            AnsiConsole.WriteLine("Clearing display from cursor to end");
            Threading.SleepCurrentThread(TestInterval);
            AnsiConsole.ClearToEnd();
        }

        public static void CursorPositionTests()
        {
            // resetting the cursor
            AnsiConsole.WriteLine("Resetting the cursor to the origin");
            Threading.SleepCurrentThread(TestInterval);
            AnsiConsole.ClearDisplay(true);
            
            // positioning the cursor
            Threading.SleepCurrentThread(TestInterval);
            AnsiConsole.HideCursor();
            for (ushort i = 1; i <= 10; i++)
            {
                AnsiConsole.SetCursorPosition(i,i);
                AnsiConsole.ReportCursorPosition();
                Threading.SleepCurrentThread((TestInterval/2));
                AnsiConsole.ClearDisplay(true);
            }
            AnsiConsole.ShowCursor();
        }

        public static void BufferHeightTest()
        {
            AnsiConsole.ClearDisplay(true);
            AnsiConsole.WriteLineRestoreCursor("Starting buffer height test...");
            Threading.SleepCurrentThread(TestInterval);
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