using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using JCS.Neon.Glow.Data.Repository;
using JCS.Neon.Glow.Test.Data.Entity;
using JCS.Neon.Glow.Types;
using Microsoft.EntityFrameworkCore;
using NuGet.Frameworks;
using Xunit;
using Xunit.Abstractions;

namespace JCS.Neon.Glow.Test.Data
{
    /// <summary>
    /// Test suite for <see cref="JCS.Neon.Glow.Data.Repository.IAsyncRepository"/>
    /// </summary>
    [Trait("Test Type", "Unit")]
    [Trait("Target Class", "AsyncRepository")]
    public class AsyncRepositoryContextTests : RepositoryAwareContextTests
    {
        public AsyncRepositoryContextTests(ITestOutputHelper outputHelperHelper) : base(outputHelperHelper)
        {
        }

        /// <summary>
        /// Can we create a valid repository?
        /// </summary>
        [Fact(DisplayName = "Can create a model entity repository")]
        [Trait("Test Type", "Unit")]
        [Trait("Target Class", "AsyncRepository")]
        public void CheckCreateValidRepository()
        {
            Assert.NotNull(_context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>());
        }

        [Fact(DisplayName = "Can't create a non-model entity repository")]
        [Trait("Test Type", "Data")]
        [Trait("Target Class", "AsyncRepository")]
        public void CheckCreateInvalidRepository()
        {
            Assert.Throws<RepositoryAwareDbContextException>(() =>
            {
                var repository = _context.CreateAsyncRepository<Guid, NonModelGuidKeyedEntity>();    
            });
        }

        [Fact(DisplayName = "Count items in the repository")]
        [Trait("Test Type", "Unit")]
        [Trait("Target Class", "AsyncRepository")]
        public async void CountRepositoryItems()
        {
            var repository = _context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>();
            var total = await repository.Count();
            AddTestEntries();
            Assert.Equal(total + 10, await repository.Count());
        }

        [Fact(DisplayName = "Count items matching a predicate")]
        [Trait("Test Type", "Unit")]
        [Trait("Target Class", "AsyncRepository")]
        public async void CountMatchingRepositoryItems()
        {
            var repository = _context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            Assert.Equal(1, await repository.CountWhere(p => p.StringProperty == "Sample value 1", default));
        }

        [Fact(DisplayName = "Can check for the existence of know item")]
        [Trait("Test Type", "Unit")]
        [Trait("Target Class", "AsyncRepository")]
        public async void CheckForItemWithKey()
        {
            var repository = _context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            Assert.True(await repository.HasItemWithKey(_testEntries[0].Id));
        }

        [Fact(DisplayName = "Can select known values based on an expression")]
        [Trait("Test Type", "Unit")]
        [Trait("Target Class", "AsyncRepository")]
        public async void SelectKnownValues()
        {
            var repository = _context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            var item = await repository.SelectOne(v => v.StringProperty.Equals("Sample value 3"));
            Assert.False(item.IsNone);
        }

        [Fact(DisplayName = "Can't select an unknown value based on an expression")]
        [Trait("Test Type", "Unit")]
        [Trait("Target Class", "AsyncRepository")]
        public async void SelectUnknownValues()
        {
            var repository = _context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            var item = await repository.SelectOne(v => v.StringProperty.Equals("Invalid property value"));
            Assert.True(item.IsNone);
        }

        [Fact(DisplayName = "Can enumerate all items asynchronously")]
        [Trait("Test Type", "Unit")]
        [Trait("Target Class", "AsyncRepository")]
        public async void CheckAsyncEnumeration()
        {
            var repository = _context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            var enumerator = (repository.GetEnumerator());
            while (await enumerator.MoveNextAsync())
            {
                var item = enumerator.Current;
                Assert.IsType<ModelGuidKeyedTestEntity>(item);
            }
        }

        [Fact(DisplayName = "Can select a known entity based on key value")]
        [Trait("Test Type", "Unit")]
        [Trait("Target Class", "AsyncRepository")]
        public async void SelectOneByKnownKey()
        {
            var repository = _context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            var item = await repository.SelectOne(_testEntries[0].Id);
            Assert.False(item.IsNone);
        }

        [Fact(DisplayName = "Can select multiple known entries based on key values")]
        [Trait("Test Type", "Unit")]
        [Trait("Target Class", "AsyncRepository")]
        public async void SelectManyByKnownKeys()
        {
            var repository = _context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            var items = await repository.SelectMany(new Guid[] {_testEntries[0].Id, _testEntries[1].Id});
            Assert.Equal(2, items.Count());
        }

        [Fact(DisplayName = "Can select multiple entries based on an expression")]
        [Trait("Test Type", "Unit")]
        [Trait("Target Class", "AsyncRepository")]
        public async void SelectManyByExpression()
        {
            var repository = _context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            var items = await repository.SelectMany(v => v.StringProperty.StartsWith("Sample"));
            Assert.Equal(_testEntries.Count, items.Count());
        }

        [Fact(DisplayName = "Can create a single new item")]
        [Trait("Test Type", "Unit")]
        [Trait("Target Class", "AsyncRepository")]
        public async void CreateSingleItem()
        {
            var repository = _context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>();
            var item = await repository.CreateAsync(new ModelGuidKeyedTestEntity()
            {
                StringProperty = "Test value"
            });
            Assert.Equal(1, await repository.Count());
        }

        [Fact(DisplayName = "Can create multiple new items")]
        [Trait("Test Type", "Unit")]
        [Trait("Target Class", "AsyncRepository")]
        public async void CreateMultipleItems()
        {
            var repository = _context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>();
            var items = await repository.CreateAsync(_testEntries.ToArray());
            Assert.Equal(_testEntries.Count(), await repository.Count());
        }

        [Fact(DisplayName = "Can select multiple keys")]
        [Trait("Test Type", "Unit")]
        [Trait("Target Class", "AsyncRepository")]
        public async void SelectMultipleKeyValues()
        {
            var repository = _context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            var keys = await repository.SelectManyKeys(v => v.StringProperty.StartsWith("Sample"));
            Assert.Equal(_testEntries.Count(), keys.Count());
        }

        [Fact(DisplayName = "Can delete existing item")]
        [Trait("Test Type", "Unit")]
        [Trait("Target Class", "AsyncRepository")]
        public async void DeleteSingleItem()
        {
            var repository = _context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            await repository.DeleteOne(_testEntries[0]);
            Assert.Equal(_testEntries.Count - 1, await repository.Count());
        }

        [Fact(DisplayName = "Can delete multiple items")]
        [Trait("Test Type", "Unit")]
        [Trait("Target Class", "AsyncRepository")]
        public async void DeleteMultipleItems()
        {
            var repository = _context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            await repository.DeleteMany(_testEntries.ToArray());
            Assert.Equal(0, await repository.Count());            
        }

        [Fact(DisplayName = "Can update an existing item")]
        [Trait("Test Type", "Unit")]
        [Trait("Target Class", "AsyncRepository")]
        public async void UpdateExistingItem()
        {
            var repository = _context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            _testEntries[1].StringProperty = "Test update";
            await repository.UpsertOne(_testEntries[1]);
            var o = (await repository.SelectOne(v => v.Id.Equals(_testEntries[1].Id)));
            var t = o.Fold(x => x.StringProperty, () => "Failed");
            Assert.Equal("Test update", t);
        }

        [Fact(DisplayName = "Can update multiple existing items")]
        [Trait("Test Type", "Unit")]
        [Trait("Target Class", "AsyncRepository")]
        public async void UpdateExistingItems()
        {
            var repository = _context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            _testEntries[1].StringProperty = "Test update";
            _testEntries[2].StringProperty = "Test update";
            await repository.UpsertMany(new ModelGuidKeyedTestEntity[] {_testEntries[1], _testEntries[2]});
            ModelGuidKeyedTestEntity r1;
            ModelGuidKeyedTestEntity r2;
            (await repository.SelectOne(v => v.Id.Equals(_testEntries[1].Id))).IsSome(out r1);
            (await repository.SelectOne(v => v.Id.Equals(_testEntries[2].Id))).IsSome(out r2);
            Assert.Equal("Test update", r1.StringProperty);
            Assert.Equal("Test update", r2.StringProperty);
        }

        [Fact(DisplayName = "Can select and transform existing items to type W")]
        [Trait("Test Type", "Unit")]
        [Trait("Target Class", "AsyncRepository")]
        public async void SelectAndProjectItems()
        {
            var repository = _context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            var selected = await repository.SelectAndProjectMany<string>(v => v.StringProperty.StartsWith("Sample"), v => v.StringProperty);
            Assert.Equal(10, selected.Count());
        }
    }
}