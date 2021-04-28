#region

using System;
using JCS.Neon.Glow.Data.Repository.Mongo;
using JCS.Neon.Glow.Test.Data.Repository.EFCore;
using MongoDB.Driver.Core.Configuration;
using Xunit;

#endregion

namespace JCS.Neon.Glow.Test.Data.Repository.Mongo
{
    /// <summary>
    ///     Series of connection-related unit tests.  Check various different configuration options, depending on the test
    ///     environment
    /// </summary>
    public class MongoConnectionTests : MongoTestBase
    {
        /// <summary>
        /// Check whether we can create a context using all the defaults
        /// </summary>
        [Fact(DisplayName = "Can create a context against (localhost,27017) with anonymous authentication")]
        [Trait("Category", "Data:Mongo")]
        public void CheckDefaultContextCreation()
        {
            var context = new MongoTestContext(builder =>
            {
                builder.SetServerHost(MongoDbContextOptions.DefaultServerHost);
                builder.SetServerPort(MongoDbContextOptions.DefaultServerPort);
                builder.SetScheme(ConnectionStringScheme.MongoDB);
            });
            Assert.True(false);
        }
    }
}