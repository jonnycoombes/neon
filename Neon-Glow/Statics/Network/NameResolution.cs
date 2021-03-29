#region

using System;
using System.Net;
using Serilog;

#endregion

namespace JCS.Neon.Glow.Statics.Network
{
    /// <summary>
    ///     Contains a series of static utility/helper methods relating to network resolution
    /// </summary>
    public static class NameResolution
    {
        /// <summary>
        ///     The environment variable that can be used to look up a hostname
        /// </summary>
        public const string HostNameEnvironmentKey = "HOSTNAME";

        /// <summary>
        ///     Static logger
        /// </summary>
        private static readonly ILogger _log = Log.ForContext(typeof(NameResolution));

        /// <summary>
        ///     Attempts to retrieve the current host name.  Will try and do this in a few different ways:
        ///     1. Dns resolution
        ///     2. NetBIOS machine name call
        ///     3. Extraction from a named environment variable - HOSTNAME
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentHostName()
        {
            Logging.MethodCall(_log);
            try
            {
                return Dns.GetHostName();
            }
            catch (Exception ex)
            {
                Logging.Warning(_log, $"Exception whilst attempting the lookup of current hostname \"{ex.Message}\"");
                return Environment.MachineName;
            }
        }
    }
}