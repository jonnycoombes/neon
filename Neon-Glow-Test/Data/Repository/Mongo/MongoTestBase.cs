#region

using System;
using JCS.Neon.Glow.Data.Repository.Mongo;
using MongoDB.Driver.Core.Configuration;
using Xunit.Abstractions;

#endregion

namespace JCS.Neon.Glow.Test.Data.Repository.Mongo
{
    /// <summary>
    ///     Abstract base class for Mongo-related test suites and cases
    /// </summary>
    public abstract class MongoTestBase : TestBase, IDisposable
    {
        protected MongoTestBase(ITestOutputHelper output) : base(output)
        {
        }

        /// <summary>
        ///     Nothing here to currently dispose...
        /// </summary>
        public void Dispose()
        {
        }

        protected void ConfigureContextOptions(MongoDbContextOptionsBuilder builder)
        {
            builder.Host(MongoDbContextOptions.DefaultServerHost)
                .Port(MongoDbContextOptions.DefaultServerPort)
                .Scheme(ConnectionStringScheme.MongoDB)
                .Database("neon-glow-test")
                .AuthenticationDatabase("admin")
                .Application("neon-glow")
                .User("root")
                .Password("root");
        }
    }
}