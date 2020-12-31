using System.IO;
using System.Runtime.CompilerServices;
using Serilog;
using Serilog.Events;

namespace JCS.Neon.Glow.Helpers.General
{
    /// <summary>
    /// Static class containing methods for tidying up logging
    /// </summary>
    public static class LogHelpers
    {
        /// <summary>
        /// Will log at a verbose level a method invocation
        /// </summary>
        /// <param name="log"></param>
        /// <param name="memberName"></param>
        public static void LogMethodCall(ILogger log, [CallerMemberName]
            string memberName = "", [CallerFilePath]
            string filePath = "")
        {
            var fileName = Path.GetFileName(filePath);
            LogAtLevel(log, $"[Method invocation: {fileName}, {memberName}]", LogEventLevel.Verbose);
        }

        /// <summary>
        /// Convenience method for logging at a debug level
        /// </summary>
        /// <param name="log">The target <see cref="ILogger" instance/></param>
        /// <param name="message">The message to log</param>
        public static void LogDebug(ILogger log, string message)
        {
            LogAtLevel(log, message, LogEventLevel.Debug);
        }

        /// <summary>
        /// Convenience method for logging at a information level
        /// </summary>
        /// <param name="log">The target <see cref="ILogger" instance/></param>
        /// <param name="message">The message to log</param>
        public static void LogInformation(ILogger log, string message)
        {
            LogAtLevel(log, message, LogEventLevel.Information);
        }

        /// <summary>
        /// Convenience method for logging at a verbose level
        /// </summary>
        /// <param name="log">The target <see cref="ILogger" instance/></param>
        /// <param name="message">The message to log</param>
        public static void LogVerbose(ILogger log, string message)
        {
            LogAtLevel(log, message, LogEventLevel.Verbose);
        }

        /// <summary>
        /// Convenience method for logging at a warning level
        /// </summary>
        /// <param name="log">The target <see cref="ILogger" instance/></param>
        /// <param name="message">The message to log</param>
        public static void LogWarning(ILogger log, string message)
        {
            LogAtLevel(log, message, LogEventLevel.Warning);
        }

        /// <summary>
        /// Convenience method for logging at a error level
        /// </summary>
        /// <param name="log">The target <see cref="ILogger" instance/></param>
        /// <param name="message">The message to log</param>
        public static void LogError(ILogger log, string message)
        {
            LogAtLevel(log, message, LogEventLevel.Error);
        }

        /// <summary>
        /// Utility method that will log at a specific level
        /// </summary>
        /// <param name="level">The level to log at</param>
        /// <param name="log">The <see cref="ILogger"/> to use</param>
        public static void LogAtLevel(ILogger log, string message, LogEventLevel level = LogEventLevel.Information)
        {
            if (log.IsEnabled(level))
            {
                switch (level)
                {
                    case LogEventLevel.Debug:
                        log.Debug(message);
                        break;
                    case LogEventLevel.Error:
                        log.Error(message);
                        break;
                    case LogEventLevel.Fatal:
                        log.Fatal(message);
                        break;
                    case LogEventLevel.Information:
                        log.Information(message);
                        break;
                    case LogEventLevel.Verbose:
                        log.Verbose(message);
                        break;
                    case LogEventLevel.Warning:
                        log.Warning(message);
                        break;
                }
            }
        }
    }
}