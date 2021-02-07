#region

using System;
using System.Runtime.InteropServices;
using JCS.Neon.Glow.Utilities.General;
using Xunit;

#endregion

namespace JCS.Neon.Glow.Test.Utilities.General
{
    /// <summary>
    ///     Test suite for the OS utility class
    /// </summary>
    [Trait("Category", "OS")]
    public class OSTests : TestBase, IDisposable
    {
        public void Dispose()
        {
        }

        [Fact(DisplayName = "OS should be correctly identified")]
        [Trait("Category", "OS")]
        public void CheckOSSignature()
        {
            if (OS.Linux) Assert.True(RuntimeInformation.IsOSPlatform(OSPlatform.Linux));
            if (OS.Windows) Assert.True(RuntimeInformation.IsOSPlatform(OSPlatform.Windows));
            if (OS.OSX) Assert.True(RuntimeInformation.IsOSPlatform(OSPlatform.OSX));
            if (OS.FreeBSD) Assert.True(RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD));
        }
    }
}