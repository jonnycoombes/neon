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
            AnsiConsole.SetTitle("Cursor Position Tests");

            // resetting the cursor
            AnsiConsole.ClearDisplay(true);

            // positioning the cursor
            ThreadHelpers.SleepCurrentThread(TestInterval);
            AnsiConsole.HideCursor();
            for (ushort i = 0; i < 5000; i++)
            {
                try
                {
                    if (i % 100 == 0)
                    {
                        AnsiConsole.SwitchBuffer(false);
                        AnsiConsole.SetCursorPosition(AnsiConsole.Origin);
                        if (AnsiConsole.CurrentBuffer == AnsiConsole.AnsiConsoleBuffer.Primary)
                        {
                            AnsiConsole.Write("Primary buffer");
                        }
                        else
                        {
                            AnsiConsole.Write("Alternate buffer");
                        }
                    }

                    AnsiConsole.SetCursorPosition(new Point(Rng.NonZeroPositiveInteger(AnsiConsole.Columns),
                        Rng.NonZeroPositiveInteger(AnsiConsole.Rows)));
                    AnsiConsole.Write($"{char.ConvertFromUtf32(0x1f196)}");
                    ThreadHelpers.SleepCurrentThread(TimeSpan.FromSeconds(0.03));
                }
                catch (AnsiConsole.AnsiConsoleCursorBoundsError ex)
                {
                    System.Console.WriteLine("OOB exception caught");
                }
            }

            AnsiConsole.ShowCursor();
        }
    }
}