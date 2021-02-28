#region

using System;
using System.Runtime.InteropServices;
using JCS.Neon.Glow.Logging;
using Serilog;

// ReSharper disable InconsistentNaming

#endregion

namespace JCS.Neon.Glow.OS.Interop.Windows
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
        private const int INVALID_HANDLE_VALUE = -1;

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
        /// Gets the current console mode bits
        /// </summary>
        /// <param name="hConsoleHandle">Console handle</param>
        /// <param name="lpMode">Store the current mode in here</param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        /// <summary>
        /// Sets the console mode bits
        /// </summary>
        /// <param name="hConsoleHandle">Console handle</param>
        /// <param name="dwMode">Set the mode to this</param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        /// <summary>
        /// Retrieves a standard handle (e.g. in,out, err)
        /// </summary>
        /// <param name="nStdHandle">The handle to retrieve</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        /// <summary>
        ///  Get the last known error code
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        private static extern uint GetLastError();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleScreenBufferInfo(IntPtr hConsoleOutput, 
            out CONSOLE_SCREEN_BUFFER_INFO ConsoleScreenBufferInfo);

        /// <summary>
        ///     This function utilises underlying Win32 console functions to enable virtual terminal capabilities.  (Required in order to
        ///     support ANSI control characters within the default windows terminal
        /// </summary>
        public static void EnableVirtualTerminal()
        {
            LogHelper.MethodCall(_log);
            if (!PlatformInformation.IsWindows)
            {
                LogHelper.Warning(_log, "Current platform is not Windows, so unable to set virtual terminal mode");
                return;
            }

            LogHelper.Verbose(_log, "Attempting necessary P/Invoke calls to modify console behaviour");
            var stdOutHandle = GetStdHandle(STD_OUTPUT_HANDLE);
            if (!GetConsoleMode(stdOutHandle, out var consoleMode))
            {
                LogHelper.Warning(_log,
                    $"Failed to retrieve the current console mode.  GetLastError reports a value of \"{GetLastError()}\"");
                return;
            }

            consoleMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN;
            if (!SetConsoleMode(stdOutHandle, consoleMode))
            {
                LogHelper.Warning(_log,
                    $"Failed to set virtual terminal mode for the console. GetLastError reports a value of \"{GetLastError()}\"");
            }
        }

        /// <summary>
        ///     <para>Checks that the console is currently in a workable state.  Check that: </para>
        ///     <para>
        ///         <ul>
        ///             <li>Standard handles are currently available and can be queried</li>
        ///             <li>We can retrieve the current <see cref="CONSOLE_SCREEN_BUFFER_INFO" /> for each of the standard handles</li>
        ///         </ul>
        ///     </para>
        /// </summary>
        /// <returns>Returns false if any of the standard I/O handles is not currently obtainable</returns>
        public static bool ConsoleWritable()
        {
            if (PlatformInformation.IsWindows)
            {
                // handle checks first
                var stdOutHandle = GetStdHandle(STD_OUTPUT_HANDLE);
                if ((int) stdOutHandle == INVALID_HANDLE_VALUE)
                {
                    return false;
                }

                // try and retrieve information about the current screen buffer - this can fail during window resizes etc...as the 
                // underlying Win32 code appears to destroy/recreate the console during such operations
                CONSOLE_SCREEN_BUFFER_INFO csbi;
                if (!GetConsoleScreenBufferInfo(stdOutHandle, out csbi))
                {
                    return false;
                }
                return true;
            }
            return true;
        }

        /// <summary>
        ///     COORD
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct COORD
        {
            public short X;
            public short Y;
        }

        /// <summary>
        ///     SMALL_RECT
        /// </summary>
        public struct SMALL_RECT
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }

        /// <summary>
        ///     CONSOLE_SCREEN_BUFFER_INFO
        /// </summary>
        public struct CONSOLE_SCREEN_BUFFER_INFO
        {
            public COORD dwSize;
            public COORD dwCursorPosition;
            public short wAttributes;
            public SMALL_RECT srWindow;
            public COORD dwMaximumWindowSize;
        }
    }
}