#region

using System;
using System.Runtime.InteropServices;
using Xunit;

#endregion

namespace JCS.Neon.Glow.Test.OS
{
    /// <summary>
    ///     Test suite for the OS utility class
    /// </summary>
    [Trait("Category", "OS")]
    public class PlatformInformationTests : TestBase, IDisposable
    {
        public void Dispose()
        {
        }

        [Fact(DisplayName = "OS should be correctly identified")]
        [Trait("Category", "OS")]
        public void CheckOSSignature()
        {
            if (Glow.OS.PlatformInformation.IsLinux) Assert.True(RuntimeInformation.IsOSPlatform(OSPlatform.Linux));
            if (Glow.OS.PlatformInformation.IsWindows) Assert.True(RuntimeInformation.IsOSPlatform(OSPlatform.Windows));
            if (Glow.OS.PlatformInformation.IsOSX) Assert.True(RuntimeInformation.IsOSPlatform(OSPlatform.OSX));
            if (Glow.OS.PlatformInformation.IsFreeBSD) Assert.True(RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD));
        }
    }
}