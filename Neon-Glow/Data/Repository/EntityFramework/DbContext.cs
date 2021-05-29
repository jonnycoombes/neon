/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System;
using JCS.Neon.Glow.Statics;
using Microsoft.EntityFrameworkCore;
using Serilog;

#endregion

namespace JCS.Neon.Glow.Data.Repository.EntityFramework
{
    /// <summary>
    ///     A <see cref="Microsoft.EntityFrameworkCore.DbContext" /> that understands how to create instances of <see cref="IRepository{K,V}" />
    ///     based on its model elements
    /// </summary>
    public abstract class DbContext : Microsoft.EntityFrameworkCore.DbContext, IRepositoryAware
    {
        /// <summary>
        ///     Default protected constructor which just
        /// </summary>
        /// <param name="options"></param>
        protected DbContext(DbContextOptions options) : base(options)
        {
            Logging.MethodCall(_log);
        }

        /// <summary>
        ///     <see cref="ILogger" /> instance
        /// </summary>
        private static ILogger _log => Log.ForContext(typeof(DbContext));


        /// <inheritdoc cref="IRepositoryAware.CreateRepository{K,V}" />
        public IRepository<K, V> CreateRepository<K, V>()
            where K : IComparable<K>, IEquatable<K>
            where V : Entity<K>
        {
            Logging.MethodCall(_log);
            Logging.Debug(_log, $"Creating new instance of IAsyncRepository for entity type {typeof(V)}");
            var fullName = typeof(V).FullName;
            if (string.IsNullOrEmpty(fullName))
            {
                throw Exceptions.LoggedException<DbContextException>(_log,
                    "Failed to locate value type name");
            }

            var entityType = Model.FindEntityType(typeof(V).FullName!);
            if (entityType != null)
            {
                return new Repository<K, V>(this);
            }

            var message = $"Context doesn't appear to include type ({typeof(V).Name}) within model";
            Logging.Error(_log, message);
            throw Exceptions.LoggedException<DbContextException>(_log, message);
        }
    }
}