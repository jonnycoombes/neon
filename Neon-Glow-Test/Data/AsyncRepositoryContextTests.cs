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
    [Trait("Category", "Unit")]
    public class AsyncRepositoryContextTests : RepositoryAwareContextTests
    {
        public AsyncRepositoryContextTests(ITestOutputHelper outputHelperHelper) : base(outputHelperHelper)
        {
        }

        /// <summary>
        /// Can we create a valid repository?
        /// </summary>
        [Fact(DisplayName = "Can create a model entity repository")]
        [Trait("Category", "Data")]
        public void CheckCreateValidRepository()
        {
            Assert.NotNull(_context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>());
        }

        [Fact(DisplayName = "Can't create a non-model entity repository")]
        [Trait("Category", "Data")]
        public void CheckCreateInvalidRepository()
        {
            Assert.Throws<RepositoryAwareDbContextException>(() =>
            {
                var repository = _context.CreateAsyncRepository<Guid, NonModelGuidKeyedEntity>();    
            });
        }

        [Fact(DisplayName = "Count items in the repository")]
        [Trait("Category", "Data")]
        public async void CountRepositoryItems()
        {
            var repository = _context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>();
            var total = await repository.CountAsync();
            AddTestEntries();
            Assert.Equal(total + 10, await repository.CountAsync());
        }

        [Fact(DisplayName = "Count items matching a predicate")]
        [Trait("Category", "Data")]
        public async void CountMatchingRepositoryItems()
        {
            var repository = _context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            Assert.Equal(1, await repository.CountAsyncWhere(p => p.StringProperty == "Sample value 1", default));
        }

        [Fact(DisplayName = "Can check for the existence of know item")]
        [Trait("Category", "Data")]
        public async void CheckForItemWithKey()
        {
            var repository = _context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            Assert.True(await repository.HasItemWithKey(_testEntries[0].Id));
        }

        [Fact(DisplayName = "Can select known values based on an expression")]
        [Trait("Category", "Data")]
        public async void SelectKnownValues()
        {
            var repository = _context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            var item = await repository.SelectOneAsync(v => v.StringProperty.Equals("Sample value 3"));
            Assert.False(item.IsNone);
        }

        [Fact(DisplayName = "Can't select an unknown value based on an expression")]
        [Trait("Category", "Data")]
        public async void SelectUnknownValues()
        {
            var repository = _context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            var item = await repository.SelectOneAsync(v => v.StringProperty.Equals("Invalid property value"));
            Assert.True(item.IsNone);
        }

        [Fact(DisplayName = "Can enumerate all items asynchronously")]
        [Trait("Category", "Data")]
        public async void CheckAsyncEnumeration()
        {
            var repository = _context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            var enumerator = (repository.GetAsyncEnumerator());
            while (await enumerator.MoveNextAsync())
            {
                var item = enumerator.Current;
                Assert.IsType<ModelGuidKeyedTestEntity>(item);
            }
        }

        [Fact(DisplayName = "Can select a known entity based on key value")]
        [Trait("Category", "Data")]
        public async void SelectOneByKnownKey()
        {
            var repository = _context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            var item = await repository.SelectOneAsync(_testEntries[0].Id);
            Assert.False(item.IsNone);
        }

        [Fact(DisplayName = "Can select multiple known entries based on key values")]
        [Trait("Category", "Data")]
        public async void SelectManyByKnownKeys()
        {
            var repository = _context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            var items = await repository.SelectManyAsync(new Guid[] {_testEntries[0].Id, _testEntries[1].Id});
            Assert.Equal(2, items.Count());
        }

        [Fact(DisplayName = "Can select multiple entries based on an expression")]
        [Trait("Category", "Data")]
        public async void SelectManyByExpression()
        {
            var repository = _context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            var items = await repository.SelectManyAsync(v => v.StringProperty.StartsWith("Sample"));
            Assert.Equal(_testEntries.Count, items.Count());
        }

        [Fact(DisplayName = "Can create a single new item")]
        [Trait("Category", "Data")]
        public async void CreateSingleItem()
        {
            var repository = _context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>();
            var item = await repository.CreateOneAsync(new ModelGuidKeyedTestEntity()
            {
                StringProperty = "Test value"
            });
            Assert.Equal(1, await repository.CountAsync());
        }

        [Fact(DisplayName = "Can create multiple new items")]
        [Trait("Category", "Data")]
        public async void CreateMultipleItems()
        {
            var repository = _context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>();
            var items = await repository.CreateManyAsync(_testEntries.ToArray());
            Assert.Equal(_testEntries.Count(), await repository.CountAsync());
        }

        [Fact(DisplayName = "Can select multiple keys")]
        [Trait("Category", "Data")]
        public async void SelectMultipleKeyValues()
        {
            var repository = _context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            var keys = await repository.SelectManyKeysAsync(v => v.StringProperty.StartsWith("Sample"));
            Assert.Equal(_testEntries.Count(), keys.Count());
        }

        [Fact(DisplayName = "Can delete existing item")]
        [Trait("Category", "Data")]
        public async void DeleteSingleItem()
        {
            var repository = _context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            await repository.DeleteOneAsync(_testEntries[0]);
            Assert.Equal(_testEntries.Count - 1, await repository.CountAsync());
        }

        [Fact(DisplayName = "Can delete multiple items")]
        [Trait("Category", "Data")]
        public async void DeleteMultipleItems()
        {
            var repository = _context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            await repository.DeleteManyAsync(_testEntries.ToArray());
            Assert.Equal(0, await repository.CountAsync());            
        }

        [Fact(DisplayName = "Can update an existing item")]
        [Trait("Category", "Data")]
        public async void UpdateExistingItem()
        {
            var repository = _context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            _testEntries[1].StringProperty = "Test update";
            await repository.UpsertOneAsync(_testEntries[1]);
            var o = (await repository.SelectOneAsync(v => v.Id.Equals(_testEntries[1].Id)));
            var t = o.Fold(x => x.StringProperty, () => "Failed");
            Assert.Equal("Test update", t);
        }

        [Fact(DisplayName = "Can update multiple existing items")]
        [Trait("Category", "Data")]
        public async void UpdateExistingItems()
        {
            var repository = _context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            _testEntries[1].StringProperty = "Test update";
            _testEntries[2].StringProperty = "Test update";
            await repository.UpsertManyAsync(new ModelGuidKeyedTestEntity[] {_testEntries[1], _testEntries[2]});
            ModelGuidKeyedTestEntity r1;
            ModelGuidKeyedTestEntity r2;
            (await repository.SelectOneAsync(v => v.Id.Equals(_testEntries[1].Id))).IsSome(out r1);
            (await repository.SelectOneAsync(v => v.Id.Equals(_testEntries[2].Id))).IsSome(out r2);
            Assert.Equal("Test update", r1.StringProperty);
            Assert.Equal("Test update", r2.StringProperty);
        }

        [Fact(DisplayName = "Can select and transform existing items to type W")]
        [Trait("Category", "Data")]
        public async void SelectAndProjectItems()
        {
            var repository = _context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            var selected = await repository.SelectAndProjectManyAsync<string>(v => v.StringProperty.StartsWith("Sample"), v => v.StringProperty);
            Assert.Equal(10, selected.Count());
        }
    }
}