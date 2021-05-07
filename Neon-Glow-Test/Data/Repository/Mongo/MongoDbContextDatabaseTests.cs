/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using MongoDB.Driver;
using Xunit;
using Xunit.Abstractions;

#endregion

namespace JCS.Neon.Glow.Test.Data.Repository.Mongo
{
    /// <summary>
    ///     Series of connection-related unit tests.  Check various different configuration options, depending on the test
    ///     environment
    /// </summary>
    public class MongoDbContextDatabaseTests : MongoTestBase
    {
        public MongoDbContextDatabaseTests(ITestOutputHelper output) : base(output)
        {
        }

        /// <summary>
        ///     Check whether we can bind to a database
        /// </summary>
        [Theory(DisplayName = "Can bind to a new database based on default context options")]
        [Trait("Category", "Data:Mongo")]
        [InlineData("neon-test")]
        public void CheckNewDatabaseBind(string databaseName)
        {
            var context = new MongoTestContext(ConfigureContextOptions(databaseName));
            var database = context.Database;
            Assert.True(database.Settings.ReadConcern.Equals(ReadConcern.Default));
            Assert.True(database.DatabaseNamespace.DatabaseName == databaseName);
        }

        [Theory(DisplayName = "Can bind to the system databases")]
        [Trait("Category", "Data:Mongo")]
        [InlineData("local")]
        [InlineData("admin")]
        [InlineData("config")]
        public void CheckSystemDatabaseBind(string databaseName)
        {
            var context = new MongoTestContext(ConfigureContextOptions(databaseName));
            var database = context.Database;
            Assert.True(database.DatabaseNamespace.DatabaseName == databaseName);
        }
    }
}