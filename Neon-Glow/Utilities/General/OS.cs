#region

using System.Runtime.InteropServices;

#endregion

namespace JCS.Neon.Glow.Utilities.General
{
    /// <summary>
    ///     Class that just contains some simple utilities for OS detection etc...
    /// </summary>
    public static class OS
    {
        /// <summary>
        ///     Windows OS check
        /// </summary>
        /// <returns>true if the current platform is Windows</returns>
        public static bool IsWindows()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }

        /// <summary>
        ///     Linux OS check
        /// </summary>
        /// <returns>true is the current platform is Linux</returns>
        public static bool IsLinux()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        }

        /// <summary>
        ///     OSX OS check
        /// </summary>
        /// <returns>true if the current platform is OSX</returns>
        public static bool IsOSX()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        }

        /// <summary>
        ///     FreeBSD OS check
        /// </summary>
        /// <returns>true if the current platform is FreeBSD</returns>
        public static bool IsFreeBSD()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD);
        }
    }
}