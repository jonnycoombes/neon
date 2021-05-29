/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using Xunit;
using Xunit.Abstractions;

#endregion

namespace JCS.Neon.Glow.Test.Data.Repository.Mongo
{
    /// <summary>
    ///     Tests covering basic collection functionality such as binding, checking CRUD operations etc...
    /// </summary>
    public class CollectionTests : TestBase, IClassFixture<Fixtures>
    {
        /// <summary>
        ///     Default constructor
        /// </summary>
        /// <param name="output"></param>
        /// <param name="fixtures"></param>
        public CollectionTests(ITestOutputHelper output, Fixtures fixtures) : base(output)
        {
            Fixtures = fixtures;
        }

        /// <summary>
        ///     The fixtures to be used by this test
        /// </summary>
        protected Fixtures Fixtures { get; set; }

        [Fact(DisplayName = "Can bind to a non-attributed collections")]
        [Trait("Category", "Data:Mongo")]
        public void CheckNonAttributedCollectionBind()
        {
            var collection = Fixtures.DbContext.Collection<NonAttributedEntity>();
            Assert.True(collection.CollectionNamespace.CollectionName == "nonAttributedEntity");
        }

        [Fact(DisplayName = "Can bind to a attributed collections")]
        [Trait("Category", "Data:Mongo")]
        public void CheckAttributedCollectionBind()
        {
            var collection = Fixtures.DbContext.Collection<AttributedEntity>();
            Assert.True(collection.CollectionNamespace.CollectionName == "TestCollection");
        }
    }
}