#region

using System;
using JCS.Neon.Glow.Utilities.Network;
using Xunit;

#endregion

namespace JCS.Neon.Glow.Test.Utilities.Network
{
    /// <summary>
    ///     Test suite for <see cref="Resolution" />
    /// </summary>
    [Trait("Category", "Network")]
    public class ResolutionTests : TestBase, IDisposable
    {
        public void Dispose()
        {
        }

        [Fact(DisplayName = "Must be able to resolve the current host name")]
        [Trait("Category", "General")]
        public void ResolveHostName()
        {
            var hostName = Resolution.GetCurrentHostName();
            Assert.NotNull(hostName);
        }
    }
}