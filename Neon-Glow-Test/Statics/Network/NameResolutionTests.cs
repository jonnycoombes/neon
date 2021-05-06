/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System;
using JCS.Neon.Glow.Statics.Network;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

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
            GC.SuppressFinalize(this);
        }

        [Fact(DisplayName = "Must be able to resolve the current host name")]
        [Trait("Category", "General")]
        public void ResolveHostName()
        {
            var hostName = NameResolution.GetCurrentHostName();
            Assert.NotNull(hostName);
        }

        public NameResolutionTests(ITestOutputHelper output) : base(output)
        {
        }
    }
}