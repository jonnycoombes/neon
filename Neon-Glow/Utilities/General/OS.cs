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
        public static bool Windows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        /// <summary>
        ///     Linux OS check
        /// </summary>
        /// <returns>true is the current platform is Linux</returns>
        public static bool Linux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        /// <summary>
        ///     OSX OS check
        /// </summary>
        /// <returns>true if the current platform is OSX</returns>
        public static bool OSX => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        /// <summary>
        ///     FreeBSD OS check
        /// </summary>
        /// <returns>true if the current platform is FreeBSD</returns>
        public static bool FreeBSD => RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD);

        /// <summary>
        /// Returns true if the current architecture is x86
        /// </summary>
        public static bool X86 => RuntimeInformation.OSArchitecture == Architecture.X86;

        /// <summary>
        /// Returns true if the current architecture is x64
        /// </summary>
        public static bool X64 => RuntimeInformation.OSArchitecture == Architecture.X64;
    }
}