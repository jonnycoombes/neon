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
    public class DatabaseTests : TestBase, IClassFixture<Fixtures>
    {

        /// <summary>
        /// The fixtures to be used by this test
        /// </summary>
        protected Fixtures Fixtures { get; set; }

        public DatabaseTests(ITestOutputHelper output, Fixtures fixtures) : base(output)
        {
            Fixtures = fixtures;
        }

        /// <summary>
        ///     Check whether we can bind to a database
        /// </summary>
        [Theory(DisplayName = "Can bind to a new database based on default context options")]
        [Trait("Category", "Data:Mongo")]
        [InlineData("test")]
        public void CheckNewDatabaseBind(string databaseName)
        {
            var context = Fixtures.DbContext; 
            var database = context.Database;
            Assert.True(database.Settings.ReadConcern.Equals(ReadConcern.Default));
            Assert.True(database.DatabaseNamespace.DatabaseName == databaseName);
        }

        [Fact(DisplayName = "Can check database existence")]
        [Trait("Category", "Data:Mongo")]
        public void CheckNonExistentDatabase()
        {
            Assert.False(Fixtures.DbContext.DatabaseExists("rubbish"));
            Assert.True(Fixtures.DbContext.DatabaseExists("local"));
            Assert.True(Fixtures.DbContext.DatabaseExists("admin"));
            Assert.False(Fixtures.DbContext.DatabaseExists("cheese"));
        }
        
    }
}