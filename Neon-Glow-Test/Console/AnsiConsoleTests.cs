using System.Threading;
using JCS.Neon.Glow.Console;
using Xunit;

namespace JCS.Neon.Glow.Test.Console
{
    [Collection("ConsoleSequential")]
    [Trait("Category", "Console")]
    public class AnsiConsoleTests 
    {

        private static void SleepCurrentThread(int count)
        {
            Thread.Sleep(count * 1000);
        }
        
        [Fact(DisplayName = "Clearing the display")]
        public void ClearConsoleTests()
        {
            AnsiConsole.WriteLine("Clearing display (no buffer wipe)");
            SleepCurrentThread(1);
            AnsiConsole.ClearDisplay(false);
        AnsiConsole.ReportCursorPosition(TODO);
            SleepCurrentThread(1);
            AnsiConsole.WriteLine("Clearing display (buffer wipe)");
            AnsiConsole.ClearDisplay();
            AnsiConsole.ReportCursorPosition(TODO);
        }

        [Fact(DisplayName = "Clearing from cursor to end")]
        public void ClearToEnd()
        {
            AnsiConsole.WriteLine("Clearing to end of display");
            AnsiConsole.ReportCursorPosition(TODO);
            SleepCurrentThread(5);
            AnsiConsole.ClearToEnd();
            AnsiConsole.ReportCursorPosition(TODO);
        }

        [Fact(DisplayName = "Clearing to the current cursor position")]
        public void ClearToCursor()
        {
            AnsiConsole.WriteLine("Clearing to cursor");
            AnsiConsole.ReportCursorPosition(TODO);
            SleepCurrentThread(5);
            AnsiConsole.ClearToCursor();
        }
    }
}