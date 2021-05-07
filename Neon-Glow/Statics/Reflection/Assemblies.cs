#region

using System.Reflection;
using Serilog;

#endregion

namespace JCS.Neon.Glow.Statics.Reflection
{
    /// <summary>
    ///     Static containing utilities and helpers relating to assemblies
    /// </summary>
    public static class Assemblies
    {
        /// <summary>
        ///     Static logger
        /// </summary>
        private static readonly ILogger _log = Log.ForContext(typeof(Activation));

        /// <summary>
        ///     Returns a string containing the version of the currently executing assembly
        /// </summary>
        /// <returns></returns>
        public static string GetApplicationAssemblyVersion(bool includeAssemblyName = false)
        {
            Logging.MethodCall(_log);
            var assembly = Assembly.GetEntryAssembly();
            return includeAssemblyName ? $"{assembly?.FullName} - {assembly?.GetName().Version}" : $"{assembly?.GetName().Version}";
        }
    }
}