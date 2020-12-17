using System;
using System.Collections.Generic;
using JCS.Neon.Glow.Data.Repository;
using JCS.Neon.Glow.Test.Data.Entity;
using NuGet.Frameworks;
using Xunit;
using Xunit.Abstractions;

namespace JCS.Neon.Glow.Data.Tests
{
    /// <summary>
    /// Test suite for <see cref="JCS.Neon.Glow.Data.Repository.IAsyncRepository"/>
    /// </summary>
    public class AsyncRepositoryContextTests : RepositoryAwareContextTest
    {
        public AsyncRepositoryContextTests(ITestOutputHelper outputHelperHelper) : base(outputHelperHelper)
        {
        }

        /// <summary>
        /// Can we create a valid repository?
        /// </summary>
        [Fact(DisplayName = "Can create a model entity repository")]
        public void CheckCreateValidRepository()
        {
            Assert.NotNull(_context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>());
        }

        [Fact(DisplayName = "Can't create a non-model entity repository")]
        public void CheckCreateInvalidRepository()
        {
            Assert.Throws<RepositoryAwareDbContextException>(() =>
            {
                var repository = _context.CreateAsyncRepository<Guid, NonModelGuidKeyedEntity>();    
            });
        }

        [Fact(DisplayName = "Count items in the repository")]
        public async void CountRepositoryItems()
        {
            var repository = _context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>();
            var total = await repository.CountAsync();
            AddTestEntries();
            Assert.Equal(await repository.CountAsync(), total + 10);
        }

        [Fact(DisplayName = "Count items matching a predicate")]
        public async void CountMatchingRepositoryItems()
        {
            var repository = _context.CreateAsyncRepository<Guid, ModelGuidKeyedTestEntity>();
            AddTestEntries();
            Assert.Equal(await repository.CountAsyncWhere(p => p.StringProperty == "Sample value 1"), 1);
        }
    }
}