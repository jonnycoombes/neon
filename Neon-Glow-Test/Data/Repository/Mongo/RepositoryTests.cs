/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */

using MongoDB.Driver;
using Xunit;
using Xunit.Abstractions;

namespace JCS.Neon.Glow.Test.Data.Repository.Mongo
{
    /// <summary>
    /// Tests for <see cref="IRepository{T}"/> functionality
    /// </summary>
    public class RepositoryTests : TestBase, IClassFixture<Fixtures>
    {
        /// <summary>
        ///     The fixtures to be used by this test
        /// </summary>
        protected Fixtures Fixtures { get; set; }
        
        public RepositoryTests(ITestOutputHelper output,Fixtures fixtures) : base(output)
        {
            Fixtures = fixtures;
        }

        [Fact(DisplayName = "Can add and then retrieve items to a repository")]
        [Trait("Category", "Data:Mongo")]
        public async void AddAndRetrieveTests()
        {
            var repository = Fixtures.DbContext.BindRepository<RepositoryEntity>(builder =>
            {
                builder.WriteConcern(WriteConcern.Acknowledged);
            });
            var added = new RepositoryEntity();
            added= await repository.CreateOne(added);
            var retrieved = await repository.FindOneById(added.Id);
            Assert.True(retrieved.IsSome());
            if (retrieved.IsSome(out var value))
            {
                Assert.Equal(value.Id, added.Id);
            }
            await repository.DeleteOne(added);
            await repository.DeleteOneById(added.Id);
        }
        
    }
}