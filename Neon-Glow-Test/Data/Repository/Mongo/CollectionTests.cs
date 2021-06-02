/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System.Collections.Generic;
using System.Linq;
using JCS.Neon.Glow.Statics.Crypto;
using MongoDB.Driver;
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
            Assert.True(collection.CollectionNamespace.CollectionName == "AttributedEntity");
        }

        [Fact(DisplayName = "Can perform basic collection operations on attributed entities")]
        [Trait("Category", "Data:Mongo")]
        public async void CheckAttributedCollectionBasicOps()
        {
            var entities = new List<AttributedEntity>();
            for (var i = 0; i < 10000; i++)
            {
                entities.Add(new AttributedEntity
                {
                    StringProperty = $"Entity {i}",
                    IntProperty = i,
                    FloatProperty = i * Rng.NonZeroNegativeInteger(500)
                });
            }

            var collection = Fixtures.DbContext.Collection<AttributedEntity>();
            await collection.InsertManyAsync(entities);
            Assert.True(await collection.EstimatedDocumentCountAsync() > 0);

            var cursor = await collection.FindAsync(Builders<AttributedEntity>.Filter.Where(e => e.IntProperty == 5));
            Assert.True(await cursor.AnyAsync());

            await collection.DeleteManyAsync(Builders<AttributedEntity>.Filter.Empty);
            Assert.True(await collection.EstimatedDocumentCountAsync() == 0);
        }

        [Fact(DisplayName = "Can perform basic collection operations on non-attributed entities")]
        [Trait("Category", "Data:Mongo")]
        public async void CheckNonAttributedCollectionBasicOps()
        {
            var entities = new List<NonAttributedEntity>();
            for (var i = 0; i < 1000; i++)
            {
                entities.Add(new NonAttributedEntity
                {
                    StringProperty = $"Entity {i}",
                    IntProperty = i,
                    FloatProperty = i * Rng.NonZeroNegativeInteger(500)
                });
            }

            var collection = Fixtures.DbContext.Collection<NonAttributedEntity>();
            await collection.InsertManyAsync(entities);
            Assert.True(await collection.EstimatedDocumentCountAsync() == 1000);

            var cursor = await collection.FindAsync(Builders<NonAttributedEntity>.Filter.Where(e => e.IntProperty == 5));
            Assert.True(await cursor.AnyAsync());

            await collection.DeleteManyAsync(Builders<NonAttributedEntity>.Filter.Empty);
            Assert.True(await collection.EstimatedDocumentCountAsync() == 0);
        }

        [Fact(DisplayName = "Can store binary data")]
        [Trait("Category", "Data:Mongo")]
        public async void CheckBinaryCollectionOps()
        {
            var entities = new List<AttributedEntity>();
            for (var i = 0; i < 1000; i++)
            {
                entities.Add(new AttributedEntity
                {
                    StringProperty = $"Entity {i}",
                    IntProperty = i,
                    FloatProperty = Rng.NonZeroNegativeInteger(500),
                    ByteArrayProperty = Rng.BoundedSequence(256).ToArray()
                });
            }

            var collection = Fixtures.DbContext.Collection<AttributedEntity>();
            await collection.InsertManyAsync(entities);
            Assert.True(await collection.EstimatedDocumentCountAsync() > 0);

            var cursor = await collection.FindAsync(Builders<AttributedEntity>.Filter.Where(e => e.IntProperty == 5));
            Assert.True(await cursor.AnyAsync());

            await collection.DeleteManyAsync(Builders<AttributedEntity>.Filter.Empty);
            Assert.True(await collection.EstimatedDocumentCountAsync() == 0);
        }

        [Fact(DisplayName = "Can perform operations on a polymorphic collection")]
        [Trait("Category", "Data:Mongo")]
        public async void CheckPolymorpicCollectionOps()
        {
            var classAEntities = new List<SubClassA>();
            var classBEntities = new List<SubClassB>();
            for (var i = 0; i < 1000; i++)
            {
                if (i % 2 == 0)
                {
                    classAEntities.Add(new SubClassA(){ClassAProperty = $"Value {i}"});
                }
                else
                {
                    classBEntities.Add(new SubClassB(){ClassBProperty = $"Value {i}"});
                }
            }

            var collection = Fixtures.DbContext.Collection<PolymorphicBase>();
            await collection.InsertManyAsync(classAEntities);
            await collection.InsertManyAsync(classBEntities);
            Assert.True((await collection.EstimatedDocumentCountAsync()) == 1000);

            var cursor = await collection.FindAsync("{'_t' : 'SubClassA'}");
            Assert.True(await cursor.AnyAsync());
            
            await collection.DeleteManyAsync(Builders<PolymorphicBase>.Filter.Empty);
            Assert.True(await collection.EstimatedDocumentCountAsync() == 0);
            
        }
    }
}