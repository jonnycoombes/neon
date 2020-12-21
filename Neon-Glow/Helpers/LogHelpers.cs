using System.IO;
using Serilog;
using Serilog.Events;

namespace JCS.Neon.Glow.Helpers
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
        public static void LogMethodCall(ILogger log, [System.Runtime.CompilerServices.CallerMemberName]
            string memberName= "", [System.Runtime.CompilerServices.CallerFilePath] string filePath= "")
        {
            var fileName= Path.GetFileName(filePath);
            LogAtLevel(log, $"[Method invocation: {memberName} - {fileName}]", LogEventLevel.Verbose);
        }
        
        /// <summary>
        /// Utility method that will log at a specific level
        /// </summary>
        /// <param name="level">The level to log at</param>
        /// <param name="log">The <see cref="ILogger"/> to use</param>
        public static void LogAtLevel(ILogger log, string message,LogEventLevel level=LogEventLevel.Information)
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