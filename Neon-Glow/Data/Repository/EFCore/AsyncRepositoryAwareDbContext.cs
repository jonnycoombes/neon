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

namespace JCS.Neon.Glow.Data.Repository.EFCore
{
    /// <summary>
    ///     A <see cref="DbContext" /> that understands how to create instances of <see cref="IAsyncRepository{K,V}" />
    ///     based on its model elements
    /// </summary>
    public abstract class AsyncRepositoryAwareDbContext : DbContext
    {
        /// <summary>
        ///     <see cref="ILogger" /> instance
        /// </summary>
        private static ILogger _log => Log.ForContext(typeof(AsyncRepositoryAwareDbContext));

        /// <summary>
        ///     Default protected constructor which just
        /// </summary>
        /// <param name="options"></param>
        protected AsyncRepositoryAwareDbContext(DbContextOptions options) : base(options)
        {
            Logging.MethodCall(_log);
        }


        /// <summary>
        ///     Attempts to instantiate an instance of <see cref="IAsyncRepository{K,V}" /> which satifies
        ///     the type parameters.  Contexts which want to support this functionality should derive their
        ///     model elements from <see cref="KeyedEntity{T}" /> in order to ensure uniformity and consistency
        ///     in repository behaviour.
        /// </summary>
        /// <typeparam name="K">The key type of the underlying model entity type</typeparam>
        /// <typeparam name="V">
        ///     The actual type of the underlying model entity type, derived from <see cref="KeyedEntity{T}" />
        /// </typeparam>
        /// <returns></returns>
        public IAsyncRepository<K, V> CreateRepository<K, V>()
            where K : IComparable<K>, IEquatable<K>
            where V : KeyedEntity<K>
        {
            Logging.MethodCall(_log);
            Logging.Verbose(_log, $"Creating new instance of IAsyncRepository for entity type {typeof(V)}");
            var fullName = typeof(V).FullName;
            if (string.IsNullOrEmpty(fullName))
            {
                throw Exceptions.LoggedException<AsyncRepositoryAwareDbContextException>(_log, "Failed to locate value type name");
            }

            var entityType = Model.FindEntityType(typeof(V).FullName!);
            if (entityType != null)
            {
                return new AsyncRepository<K, V>(this);
            }

            var message = $"Context doesn't appear to include type ({typeof(V).Name}) within model";
            Logging.Error(_log, message);
            throw Exceptions.LoggedException<AsyncRepositoryAwareDbContextException>(_log, message);
        }
    }
}