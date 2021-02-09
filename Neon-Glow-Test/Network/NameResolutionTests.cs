#region

using System;
using JCS.Neon.Glow.Network;
using Xunit;

#endregion

namespace JCS.Neon.Glow.Test.Network
{
    /// <summary>
    ///     Test suite for <see cref="NameResolutionHelpers" />
    /// </summary>
    [Trait("Category", "Network")]
    public class NameResolutionTests : TestBase, IDisposable
    {
        public void Dispose()
        {
        }

        [Fact(DisplayName = "Must be able to resolve the current host name")]
        [Trait("Category", "General")]
        public void ResolveHostName()
        {
            var hostName = NameResolutionHelpers.GetCurrentHostName();
            Assert.NotNull(hostName);
        }
    }
}