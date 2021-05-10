/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using JCS.Neon.Glow.Data.Repository.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime;
using DbContext = JCS.Neon.Glow.Data.Repository.EntityFramework.DbContext;

#endregion

namespace JCS.Neon.Glow.Test.Data.Repository.EntityFramework
{
    public class SqlLiteDbContext : DbContext
    {
        public SqlLiteDbContext(DbContextOptions<SqlLiteDbContext> options)
            : base(options)
        {
        }

        public DbSet<ModelGuidTestEntity> GuidEntries { get; set; }

        /// <summary>
        ///     Various tweaks and overrides to the default mappings happen here - for example,
        ///     <see cref="Instant" /> objects need to be converted to a suitable supported type
        ///     for SqlLite
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var instantConverter =
                new ValueConverter<Instant, System.DateTime>(v =>
                        v.ToDateTimeUtc(),
                    v => Instant.FromDateTimeUtc(v));

            modelBuilder.Entity<ModelGuidTestEntity>()
                .Property(g => g.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<ModelGuidTestEntity>()
                .Property(g => g.CreationTime)
                .HasConversion(instantConverter);

            base.OnModelCreating(modelBuilder);
        }
    }
}