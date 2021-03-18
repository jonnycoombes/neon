﻿#region

using System;
using JCS.Neon.Glow.Statics.Network;
using Xunit;

#endregion

namespace JCS.Neon.Glow.Test.Statics.Network
{
    /// <summary>
    ///     Test suite for <see cref="NameResolution" />
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
            var hostName = NameResolution.GetCurrentHostName();
            Assert.NotNull(hostName);
        }
    }
}