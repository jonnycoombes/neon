/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
﻿#region

using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

#endregion

namespace JCS.Neon.Glow.Statics.OS
{
    /// <summary>
    ///     Class that just contains some simple utilities for OS detection etc...
    /// </summary>
    public static class PlatformInformation
    {
        /// <summary>
        ///     Windows OS check
        /// </summary>
        /// <returns>true if the current platform is Windows</returns>
        public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        /// <summary>
        ///     Linux OS check
        /// </summary>
        /// <returns>true is the current platform is Linux</returns>
        public static bool IsLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        /// <summary>
        ///     OSX OS check
        /// </summary>
        /// <returns>true if the current platform is OSX</returns>
        public static bool IsOSX => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        /// <summary>
        ///     FreeBSD OS check
        /// </summary>
        /// <returns>true if the current platform is FreeBSD</returns>
        public static bool IsFreeBSD => RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD);

        /// <summary>
        ///     Returns true if the current architecture is x86
        /// </summary>
        public static bool IsX86 => RuntimeInformation.OSArchitecture == Architecture.X86;

        /// <summary>
        ///     Returns true if the current architecture is x64
        /// </summary>
        public static bool IsX64 => RuntimeInformation.OSArchitecture == Architecture.X64;
    }
}