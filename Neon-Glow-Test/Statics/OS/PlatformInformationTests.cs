#region

using System;
using System.Runtime.InteropServices;
using JCS.Neon.Glow.Statics.OS;
using Xunit;

#endregion

namespace JCS.Neon.Glow.Test.Statics.OS
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
            if (PlatformInformation.IsLinux) Assert.True(RuntimeInformation.IsOSPlatform(OSPlatform.Linux));
            if (PlatformInformation.IsWindows) Assert.True(RuntimeInformation.IsOSPlatform(OSPlatform.Windows));
            if (PlatformInformation.IsOSX) Assert.True(RuntimeInformation.IsOSPlatform(OSPlatform.OSX));
            if (PlatformInformation.IsFreeBSD) Assert.True(RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD));
        }
    }
}