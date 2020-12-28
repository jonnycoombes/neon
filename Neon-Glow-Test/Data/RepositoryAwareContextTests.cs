using System;
using System.Collections.Generic;
using System.Data.Common;
using JCS.Neon.Glow.Test.Data.Contexts;
using JCS.Neon.Glow.Test.Data.Entity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Xunit.Abstractions;

namespace JCS.Neon.Glow.Test.Data
{
    public abstract class RepositoryAwareContextTests : IDisposable
    {
        /// <summary>
        /// The actual test context
        /// </summary>
        protected SqlLiteRepositoryAwareDbContext _context;

        /// <summary>
        /// For logging purposes
        /// </summary>
        protected ITestOutputHelper _outputHelper;

        /// <summary>
        /// The context options
        /// </summary>
        protected DbContextOptions<SqlLiteRepositoryAwareDbContext> _contextOptions;

        protected List<ModelGuidKeyedTestEntity> _testEntries = new();

        public RepositoryAwareContextTests(ITestOutputHelper outputHelperHelper)
        {
            _outputHelper = outputHelperHelper;
            CreateContextOptions();
            CreateContext();
            GenerateTestEntities();
        }

        protected void CreateContextOptions()
        {
            _contextOptions = new DbContextOptionsBuilder<SqlLiteRepositoryAwareDbContext>()
                .UseSqlite(CreateInMemoryDatabase())
                .EnableSensitiveDataLogging()
                .Options;
        }
        
        protected void CreateContext()
        {
            _context = new SqlLiteRepositoryAwareDbContext(_contextOptions);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            RelationalOptionsExtension.Extract(_contextOptions).Connection.Dispose();
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
            for (var i = 0; i < 10; i++)
            {
                _testEntries.Add(new ModelGuidKeyedTestEntity()
                {
                    StringProperty = $"Sample value {i}"
                });    
            }
        }
    }
}