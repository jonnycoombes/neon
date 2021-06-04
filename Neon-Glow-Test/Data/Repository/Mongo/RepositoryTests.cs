/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */

using JCS.Neon.Glow.Data.Repository.Mongo;
using JCS.Neon.Glow.Types;
using MongoDB.Driver;
using Xunit;
using Xunit.Abstractions;

namespace JCS.Neon.Glow.Test.Data.Repository.Mongo
{
    /// <summary>
    /// Tests for <see cref="IRepository{T}"/> functionality
    /// </summary>
    [Collection("Mongo:Sequential")]
    public class RepositoryTests : TestBase
    {
        public RepositoryTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact(DisplayName = "Can perform basic single object CRUD operations within a repository")]
        [Trait("Category", "Data:Mongo")]
        public async void RepositorySingleCRUDTest()
        {
            var repository = new Fixtures().DbContext.BindRepository<RepositoryEntity>(builder =>
            {
                builder.WriteConcern(WriteConcern.Acknowledged)
                    .DeletionBehaviour(RepositoryOptions.DeletionBehaviourOption.Soft)
                    .ReadBehaviour(RepositoryOptions.ReadBehaviourOption.IgnoreDeleted);
            });
            
            var added= await repository.CreateOne(new RepositoryEntity()
            {
                IntProperty = 5,
                StringProperty = "test"
            });
            
            var id = added.Id;
            var retrieved = await repository.ReadOne(id);
            Assert.True(retrieved.IsSome());
            if (retrieved.IsSome(out var value))
            {
                Assert.Equal(value.Id, added.Id);
            }

            var mapped = await repository.MapOne(id, e => Option<int>.Some(e.IntProperty));
            Assert.True(mapped.IsSome());

            added.IntProperty = 8;
            var modified = await repository.UpdateOne(added);
            Assert.True(modified.IntProperty == 8);
            
            await repository.DeleteOne(added);
            Assert.True((await repository.ReadOne(added.Id)).IsNone);
            
        }
        
    }
}