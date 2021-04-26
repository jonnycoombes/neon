#region

using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Xunit.Abstractions;

#endregion

namespace JCS.Neon.Glow.Test.Data.Repository.EFCore
{
    public abstract class EFCoreTestBase : TestBase, IDisposable
    {
        /// <summary>
        ///     The actual test context
        /// </summary>
        protected SqlLiteAsyncRepositoryAwareDbContext _context;


        /// <summary>
        ///     The context options
        /// </summary>
        protected DbContextOptions<SqlLiteAsyncRepositoryAwareDbContext> _contextOptions;

        /// <summary>
        ///     For logging purposes
        /// </summary>
        protected ITestOutputHelper _outputHelper;

        /// <summary>
        ///     List of test entities
        /// </summary>
        protected List<ModelGuidKeyedTestEntity> _testEntries = new();

        public EFCoreTestBase(ITestOutputHelper outputHelperHelper)
        {
            _outputHelper = outputHelperHelper;
            CreateContextOptions();
            CreateContext();
            GenerateTestEntities();
        }

        public void Dispose()
        {
            RelationalOptionsExtension.Extract(_contextOptions).Connection.Dispose();
        }

        protected void CreateContextOptions()
        {
            _contextOptions = new DbContextOptionsBuilder<SqlLiteAsyncRepositoryAwareDbContext>()
                .UseSqlite(CreateInMemoryDatabase())
                .EnableSensitiveDataLogging()
                .Options;
        }

        protected void CreateContext()
        {
            _context = new SqlLiteAsyncRepositoryAwareDbContext(_contextOptions);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        protected static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            return connection;
        }

        protected async void AddTestEntries()
        {
            await _context.GuidEntries.AddRangeAsync(_testEntries);
            await _context.SaveChangesAsync();
        }

        protected async void RemoveTestEntries()
        {
            _context.GuidEntries.RemoveRange(_testEntries);
            await _context.SaveChangesAsync();
        }

        protected void GenerateTestEntities()
        {
            for (var i = 0; i < 1000; i++)
            {
                _testEntries.Add(new ModelGuidKeyedTestEntity
                {
                    StringProperty = $"Sample value {i}"
                });
            }
        }
    }
}