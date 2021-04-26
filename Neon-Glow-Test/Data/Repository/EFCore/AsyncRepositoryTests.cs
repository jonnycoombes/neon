#region

using System;
using System.Linq;
using JCS.Neon.Glow.Data.Repository.EFCore;
using JCS.Neon.Glow.Types;
using Xunit;
using Xunit.Abstractions;

#endregion

namespace JCS.Neon.Glow.Test.Data.Repository.EFCore
{
    /// <summary>
    ///     Test suite for <see cref="JCS.Neon.Glow.Data.Repository.IAsyncRepository" />
    /// </summary>
    [Trait("Category", "Data:EFCore")]
    public class AsyncRepositoryTests : EFCoreTestBase
    {
        public AsyncRepositoryTests(ITestOutputHelper outputHelperHelper) : base(outputHelperHelper)
        {
        }

        /// <summary>
        ///     Can we create a valid repository?
        /// </summary>
        [Fact(DisplayName = "Can create a model entity repository")]
        [Trait("Category", "Data:EFCore")]
        public void CheckCreateValidRepository()
        {
            Assert.NotNull(_context.CreateRepository<Guid, ModelGuidKeyedTestEntity>());
        }

        [Fact(DisplayName = "Can't create a non-model entity repository")]
        [Trait("Category", "Data:EFCore")]
        public void CheckCreateInvalidRepository()
        {
            Assert.Throws<AsyncRepositoryAwareDbContextException>(() =>
            {
                var repository = _context.CreateRepository<Guid, NonModelGuidKeyedEntity>();
            });
        }

        [Fact(DisplayName = "Count items in the repository")]
        [Trait("Category", "Data:EFCore")]
        public async void CountRepositoryItems()
        {
            var repository = _context.CreateRepository<Guid, ModelGuidKeyedTestEntity>();
            var total = await repository.Count();
            AddTestEntries();
            Assert.Equal(total + _testEntries.Count, await repository.Count());
        }

        [Fact(DisplayName = "Count items matching a predicate")]
        [Trait("Category", "Data:EFCore")]
        public async void CountMatchingRepositoryItems()
        {
            var repository = _context.CreateRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            Assert.Equal(1, await repository.CountWhere(p => p.StringProperty == "Sample value 1"));
        }

        [Fact(DisplayName = "Can check for the existence of known item in repository")]
        [Trait("Category", "Data:EFCore")]
        public async void CheckForItemWithKey()
        {
            var repository = _context.CreateRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            Assert.True(await repository.HasItemWithKey(_testEntries[0].Id));
        }

        [Fact(DisplayName = "Can select known repository values based on an expression")]
        [Trait("Category", "Data:EFCore")]
        public async void SelectKnownValues()
        {
            var repository = _context.CreateRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            var item = await repository.SelectOne(v => v.StringProperty.Equals("Sample value 3"));
            Assert.False(item.IsNone);
        }

        [Fact(DisplayName = "Can't select an unknown repository value based on an expression")]
        [Trait("Category", "Data:EFCore")]
        public async void SelectUnknownValues()
        {
            var repository = _context.CreateRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            var item = await repository.SelectOne(v => v.StringProperty.Equals("Invalid property value"));
            Assert.True(item.IsNone);
        }

        [Fact(DisplayName = "Can enumerate all repository items asynchronously")]
        [Trait("Category", "Data:EFCore")]
        public async void CheckAsyncEnumeration()
        {
            var repository = _context.CreateRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            var enumerator = repository.GetEnumerator();
            while (await enumerator.MoveNextAsync())
            {
                var item = enumerator.Current;
                Assert.IsType<ModelGuidKeyedTestEntity>(item);
            }
        }

        [Fact(DisplayName = "Can select a known repository entity based on key value")]
        [Trait("Category", "Data:EFCore")]
        public async void SelectOneByKnownKey()
        {
            var repository = _context.CreateRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            var item = await repository.SelectOne(_testEntries[0].Id);
            Assert.False(item.IsNone);
        }

        [Fact(DisplayName = "Can select multiple known repository entries based on key values")]
        [Trait("Category", "Data:EFCore")]
        public async void SelectManyByKnownKeys()
        {
            var repository = _context.CreateRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            var items = await repository.SelectMany(new[] {_testEntries[0].Id, _testEntries[1].Id});
            Assert.Equal(2, items.Count());
        }

        [Fact(DisplayName = "Can select multiple repository entries based on an expression")]
        [Trait("Category", "Data:EFCore")]
        public async void SelectManyByExpression()
        {
            var repository = _context.CreateRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            var items = await repository.SelectMany(v => v.StringProperty.StartsWith("Sample"));
            Assert.Equal(_testEntries.Count, items.Count());
        }

        [Fact(DisplayName = "Can create a single new repository item")]
        [Trait("Category", "Data:EFCore")]
        public async void CreateSingleItem()
        {
            var repository = _context.CreateRepository<Guid, ModelGuidKeyedTestEntity>();
            var item = await repository.CreateOne(new ModelGuidKeyedTestEntity
            {
                StringProperty = "Test value"
            });
            Assert.Equal(1, await repository.Count());
        }

        [Fact(DisplayName = "Can create multiple new repository items")]
        [Trait("Category", "Data:EFCore")]
        public async void CreateMultipleItems()
        {
            var repository = _context.CreateRepository<Guid, ModelGuidKeyedTestEntity>();
            var items = await repository.CreateMany(_testEntries.ToArray());
            Assert.Equal(_testEntries.Count(), await repository.Count());
        }

        [Fact(DisplayName = "Can select multiple repository keys")]
        [Trait("Category", "Data:EFCore")]
        public async void SelectMultipleKeyValues()
        {
            var repository = _context.CreateRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            var keys = await repository.SelectManyKeys(v => v.StringProperty.StartsWith("Sample"));
            Assert.Equal(_testEntries.Count(), keys.Count());
        }

        [Fact(DisplayName = "Can delete existing repository item")]
        [Trait("Category", "Data:EFCore")]
        public async void DeleteSingleItem()
        {
            var repository = _context.CreateRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            await repository.DeleteOne(_testEntries[0]);
            Assert.Equal(_testEntries.Count - 1, await repository.Count());
        }

        [Fact(DisplayName = "Can delete multiple repository items")]
        [Trait("Category", "Data:EFCore")]
        public async void DeleteMultipleItems()
        {
            var repository = _context.CreateRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            await repository.DeleteMany(_testEntries.ToArray());
            Assert.Equal(0, await repository.Count());
        }

        [Fact(DisplayName = "Can update an existing repository item")]
        [Trait("Category", "Data:EFCore")]
        public async void UpdateExistingItem()
        {
            var repository = _context.CreateRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            _testEntries[1].StringProperty = "Test update";
            await repository.UpsertOne(_testEntries[1]);
            var o = await repository.SelectOne(v => v.Id.Equals(_testEntries[1].Id));
            var t = o.Fold(x => x.StringProperty, () => "Failed");
            Assert.Equal("Test update", t);
        }

        [Fact(DisplayName = "Can update multiple existing repository items")]
        [Trait("Category", "Data:EFCore")]
        public async void UpdateExistingItems()
        {
            var repository = _context.CreateRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            _testEntries[1].StringProperty = "Test update";
            _testEntries[2].StringProperty = "Test update";
            await repository.UpsertMany(new[] {_testEntries[1], _testEntries[2]});
            ModelGuidKeyedTestEntity r1;
            ModelGuidKeyedTestEntity r2;
            (await repository.SelectOne(v => v.Id.Equals(_testEntries[1].Id))).IsSome(out r1);
            (await repository.SelectOne(v => v.Id.Equals(_testEntries[2].Id))).IsSome(out r2);
            Assert.Equal("Test update", r1.StringProperty);
            Assert.Equal("Test update", r2.StringProperty);
        }

        [Fact(DisplayName = "Can select and transform existing repository items to type W")]
        [Trait("Category", "Data:EFCore")]
        public async void SelectAndProjectItems()
        {
            var repository = _context.CreateRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            var selected = await repository.SelectAndProjectMany(v => v.StringProperty.StartsWith("Sample"), v => v.StringProperty);
            Assert.Equal(_testEntries.Count, selected.Count());
        }
    }
}