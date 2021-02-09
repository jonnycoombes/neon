#region

using System;
using JCS.Neon.Glow.Test.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime;

#endregion

namespace JCS.Neon.Glow.Test.Data.Contexts
{
    public class SqlLiteRepositoryAwareDbContext : Glow.Data.Repository.RepositoryAwareDbContext
    {
        public SqlLiteRepositoryAwareDbContext(DbContextOptions<SqlLiteRepositoryAwareDbContext> options)
            : base(options)
        {
        }

        public DbSet<ModelGuidKeyedTestEntity> GuidEntries { get; set; }

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

            modelBuilder.Entity<ModelGuidKeyedTestEntity>()
                .Property(g => g.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<ModelGuidKeyedTestEntity>()
                .Property(g => g.CreationTime)
                .HasConversion(instantConverter);

            base.OnModelCreating(modelBuilder);
        }
    }
}