#region

using System;
using System.Runtime.InteropServices;
using JCS.Neon.Glow.Utilities.General;
using Serilog;

#endregion

namespace JCS.Neon.Glow.Interop.Windows
{
    /// <summary>
    ///     Static class that acts as a placeholder for unmanaged imports (P/Invoke) from within kernel32.dll on Windows
    /// </summary>
    public static class Kernel32
    {
        /// <summary>
        ///     Constant taken from ProcessEnv.h in the Windows SDK
        /// </summary>
        private const int STD_INPUT_HANDLE = -10;

        /// <summary>
        ///     Constant taken from ProcessEnv.h in the Windows SDK
        /// </summary>
        private const int STD_OUTPUT_HANDLE = -11;

        /// <summary>
        ///     Constant taken from ProcessEnv.h in the Windows SDK
        /// </summary>
        private const int STD_ERROR_HANDLE = -12;

        /// <summary>
        ///     Constant taken from ProcessEnv.h in the Windows SDK
        /// </summary>
        private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

        /// <summary>
        ///     Constant taken from ProcessEnv.h in the Windows SDK
        /// </summary>
        private const uint DISABLE_NEWLINE_AUTO_RETURN = 0x0008;

        /// <summary>
        ///     Static logger
        /// </summary>
        private static readonly ILogger _log = Log.ForContext(typeof(Kernel32));

        /// <summary>
        /// </summary>
        /// <param name="hConsoleHandle"></param>
        /// <param name="lpMode"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        /// <summary>
        /// </summary>
        /// <param name="hConsoleHandle"></param>
        /// <param name="dwMode"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        /// <summary>
        /// </summary>
        /// <param name="nStdHandle"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        private static extern uint GetLastError();

        /// <summary>
        ///     This function utilises underlying Win32 console functions to enable virtual terminal capabilities.  (Required in order to
        ///     support ANSI control characters within the default windows terminal
        /// </summary>
        public static void EnableVirtualTerminal()
        {
            Logs.MethodCall(_log);
            if (!OS.Windows)
            {
                Logs.Warning(_log, "Current platform is not Windows, so unable to set virtual terminal mode");
                return;
            }

            Logs.Verbose(_log, "Attempting necessary P/Invoke calls to modify console behaviour");
            var stdOutHandle = GetStdHandle(STD_OUTPUT_HANDLE);
            if (!GetConsoleMode(stdOutHandle, out var consoleMode))
            {
                Logs.Warning(_log, $"Failed to retrieve the current console mode.  GetLastError reports a value of \"{GetLastError()}\"");
                return;
            }

            consoleMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN;
            if (!SetConsoleMode(stdOutHandle, consoleMode))
            {
                Logs.Warning(_log,
                    $"Failed to set virtual terminal mode for the console. GetLastError reports a value of \"{GetLastError()}\"");
            }
        }
    }
}