#region

using Xunit;
using Xunit.Abstractions;

#endregion

namespace JCS.Neon.Glow.Test.Data.Repository.Mongo
{
    /// <summary>
    ///     Series of connection-related unit tests.  Check various different configuration options, depending on the test
    ///     environment
    /// </summary>
    public class MongoDatabaseTests : MongoTestBase
    {
        public MongoDatabaseTests(ITestOutputHelper output) : base(output)
        {
        }

        /// <summary>
        ///     Check whether we can create a context using all the defaults
        /// </summary>
        [Fact(DisplayName = "Required database is dynamically generated with options by the context")]
        [Trait("Category", "Data:Mongo")]
        public void CheckDatabaseCreation()
        {
            var context = new MongoTestContext(ConfigureContextOptions);
            var database = context.Database;
            Assert.True(false);
        }

        [Fact(DisplayName = "Required database is dynamically generated with the correct settings overridden by a context subclass")]
        [Trait("Category", "Data:Mongo")]
        public void CheckDatabaseSettings()
        {
            var context = new MongoTestContext(ConfigureContextOptions);
            var database = context.Database;
            Assert.True(false);
        }
    }
}