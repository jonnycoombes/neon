/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using JCS.Neon.Glow.Data.Repository.Mongo;
using JCS.Neon.Glow.Statics.Crypto;
using JCS.Neon.Glow.Types;
using JCS.Neon.Glow.Types.Extensions;
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

        [Theory(DisplayName = "Can perform basic single object CRUD operations within a repository")]
        [Trait("Category", "Data:Mongo")]
        [InlineData(RepositoryOptions.DeletionBehaviourOption.Hard)]
        [InlineData(RepositoryOptions.DeletionBehaviourOption.Soft)]
        public async void SingleCRUDTest(RepositoryOptions.DeletionBehaviourOption deletionBehaviour)
        {
            var repository = new Fixtures().DbContext.BindRepository<RepositoryEntity>(builder =>
            {
                builder.WriteConcern(WriteConcern.Acknowledged)
                    .DeletionBehaviour(deletionBehaviour)
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

            // PURGE
            await repository.Purge();
        }

        [Theory(DisplayName = "Can perform basic single object polymorphic CRUD operations within a repository")]
        [Trait("Category", "Data:Mongo")]
        [InlineData(RepositoryOptions.DeletionBehaviourOption.Hard)]
        [InlineData(RepositoryOptions.DeletionBehaviourOption.Soft)]
        public async void SinglePolymorhpicCRUDTest(RepositoryOptions.DeletionBehaviourOption deletionBehaviour)
        {
            var repository = new Fixtures().DbContext.BindRepository<RepositoryEntity, PolymorphicEntity>(builder =>
            {
                builder.WriteConcern(WriteConcern.Acknowledged)
                    .DeletionBehaviour(deletionBehaviour)
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

            // PURGE 
            await repository.Purge();
        }


        [Theory(DisplayName = "Can perform basic bulk CRUD operations within a repository")]
        [Trait("Category", "Data:Mongo")]
        [InlineData(RepositoryOptions.DeletionBehaviourOption.Hard)]
        [InlineData(RepositoryOptions.DeletionBehaviourOption.Soft)]
        public async void BulkCRUDTest(RepositoryOptions.DeletionBehaviourOption deletionBehaviour)
        {
            var repository = new Fixtures().DbContext.BindRepository<RepositoryEntity>(builder =>
            {
                builder.WriteConcern(WriteConcern.Acknowledged)
                    .DeletionBehaviour(deletionBehaviour)
                    .ReadBehaviour(RepositoryOptions.ReadBehaviourOption.IgnoreDeleted);
            });

            // CREATE
            var random = Passphrase.GenerateRandomPassphrase();
            var created = await repository.CreateMany(1000, i => new RepositoryEntity {IntProperty = i, StringProperty = random});

            // READ
            var read = await repository.ReadMany(() => Builders<RepositoryEntity>.Filter.Eq(t => t.StringProperty, random));
            Assert.True(read.Length == 1000);

            // MAP
            var mapped = await repository.MapMany(() => Builders<RepositoryEntity>.Filter.Eq(t => t.StringProperty, random),
                entity => Option<string>.Some(entity.StringProperty));
            Assert.True(mapped[Rng.NonZeroPositiveInteger(mapped.Length)].IsSome());
            
            // UPDATE
            foreach (var v in created)
            {
                v.ByteArrayProperty = new byte[4].Randomise();
            }

            var updated = await repository.UpdateMany(created);

            // DELETE
            var deletedCount = await repository.DeleteMany(updated);
            Assert.True(deletedCount == 1000);
        }
    }
}