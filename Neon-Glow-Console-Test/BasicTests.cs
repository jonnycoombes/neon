#region

using System;
using System.Drawing;
using System.Linq;
using JCS.Neon.Glow.Concurrency;
using JCS.Neon.Glow.Cryptography;

#endregion

namespace JCS.Neon.Glow.Console.Test
{
    /// <summary>
    ///     Basic test cases for the Neon Glow ANSI console
    /// </summary>
    public static class BasicTests
    {
        /// <summary>
        ///     Interval of pause between tests in seconds
        /// </summary>
        private const int TestInterval = 2;

        public static void CursorPositionTests()
        {
            if (!AnsiConsole.Enabled) AnsiConsole.Enable();
            AnsiConsole.SetTitle("Cursor Position Tests");
            
            // resetting the cursor
            AnsiConsole.ClearDisplay(true);

            // positioning the cursor
            ThreadHelper.SleepCurrentThread(TestInterval);
            AnsiConsole.HideCursor();
            for (ushort i = 0; i < 5000; i++)
            {
                try
                {
                    if (i % 100 == 0)
                    {
                        AnsiConsole.SwitchBuffer(false);
                        AnsiConsole.SetTitle(AnsiConsole.CurrentBuffer.ToString());
                    }

                    // move to origin
                    AnsiConsole.SetCursorPosition(AnsiConsole.Origin);
                    // delete line
                    AnsiConsole.EraseCurrentLine();
                    AnsiConsole.Write($"Current dimensions: [rows: {AnsiConsole.Rows}, columns : {AnsiConsole.Columns}]");
                    
                    AnsiConsole.SetCursorPosition(new Point(RngHelper.NonZeroPositiveInteger(AnsiConsole.Columns),
                        RngHelper.NonZeroPositiveInteger(AnsiConsole.Rows)));
                    AnsiConsole.Write($"{char.ConvertFromUtf32(0x1f196)}");
                    ThreadHelper.SleepCurrentThread(TimeSpan.FromSeconds(0.5));
                }
                catch (AnsiConsole.AnsiConsoleCursorBoundsError)
                {
                    System.Console.WriteLine("OOB");
                }
                catch (AnsiConsole.AnsiConsoleException)
                {
                    ThreadHelper.SleepCurrentThread(TimeSpan.FromSeconds(1));
                }
            }

            AnsiConsole.ShowCursor();
        }
    }
}