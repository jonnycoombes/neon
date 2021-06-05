/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using JCS.Neon.Glow.Data.Repository.Mongo;
using JCS.Neon.Glow.Types;
using MongoDB.Driver;
using Xunit;
using Xunit.Abstractions;

#endregion

namespace JCS.Neon.Glow.Test.Data.Repository.Mongo
{
    /// <summary>
    ///     Tests for <see cref="IRepository{T}" /> functionality
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

            // CREATE
            var added = await repository.CreateOne(new RepositoryEntity
            {
                IntProperty = 5,
                StringProperty = "test"
            });

            // READ
            Assert.True((await repository.ReadOne(added.Id)).IsSome());
            Assert.True((await repository.ReadOne(() => { return Builders<RepositoryEntity>.Filter.Eq(t => t.Id, added.Id); })).IsSome());

            // MAP
            var mapped = await repository.MapOne(added.Id, e => Option<int>.Some(e.IntProperty));
            Assert.True(mapped.IsSome());

            // UPDATE
            added.IntProperty = 8;
            var modified = await repository.UpdateOne(added);
            Assert.True(modified.IntProperty == 8);

            // DELETE
            await repository.DeleteOne(added);
            Assert.True((await repository.ReadOne(added.Id)).IsNone);
        }

        [Fact(DisplayName = "Can perform basic single object polymorphic CRUD operations within a repository")]
        [Trait("Category", "Data:Mongo")]
        public async void RepositorySinglePolymorhpicCRUDTest()
        {
            var repository = new Fixtures().DbContext.BindRepository<RepositoryEntity, PolymorphicEntity>(builder =>
            {
                builder.WriteConcern(WriteConcern.Acknowledged)
                    .DeletionBehaviour(RepositoryOptions.DeletionBehaviourOption.Soft)
                    .ReadBehaviour(RepositoryOptions.ReadBehaviourOption.IgnoreDeleted);
            });

            // CREATE
            var added = await repository.CreateOne(new PolymorphicEntity
            {
                IntProperty = 5,
                StringProperty = "test"
            });

            // READ
            Assert.True((await repository.ReadOne(added.Id)).IsSome());
            Assert.True((await repository.ReadOne(() => { return Builders<PolymorphicEntity>.Filter.Eq(t => t.Id, added.Id); })).IsSome());
            // MAP
            var mapped = await repository.MapOne(added.Id, e => Option<int>.Some(e.IntProperty));
            Assert.True(mapped.IsSome());

            // UPDATE
            added.IntProperty = 8;
            var modified = await repository.UpdateOne(added);
            Assert.True(modified.IntProperty == 8);

            // DELETE
            await repository.DeleteOne(added);
            Assert.True((await repository.ReadOne(added.Id)).IsNone);
        }
    }
}