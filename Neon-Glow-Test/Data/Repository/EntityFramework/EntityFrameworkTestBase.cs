/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Xunit.Abstractions;

#endregion

namespace JCS.Neon.Glow.Test.Data.Repository.EntityFramework
{
    public abstract class EntityFrameworkTestBase : TestBase, IDisposable
    {
        /// <summary>
        ///     The actual test context
        /// </summary>
        protected SqlLiteDbContext _context;

        /// <summary>
        ///     The context options
        /// </summary>
        protected DbContextOptions<SqlLiteDbContext> _contextOptions;

        /// <summary>
        ///     For logging purposes
        /// </summary>
        protected ITestOutputHelper _outputHelper;

        /// <summary>
        ///     List of test entities
        /// </summary>
        protected List<ModelGuidTestEntity> _testEntries = new();

        public EntityFrameworkTestBase(ITestOutputHelper output) : base(output)
        {
            _outputHelper = output;
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
            _contextOptions = new DbContextOptionsBuilder<SqlLiteDbContext>()
                .UseSqlite(CreateInMemoryDatabase())
                .EnableSensitiveDataLogging()
                .Options;
        }

        protected void CreateContext()
        {
            _context = new SqlLiteDbContext(_contextOptions);
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
                _testEntries.Add(new ModelGuidTestEntity
                {
                    StringProperty = $"Sample value {i}"
                });
            }
        }
    }
}