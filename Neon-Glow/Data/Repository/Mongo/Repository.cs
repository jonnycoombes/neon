/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System;
using JCS.Neon.Glow.Statics;
using JCS.Neon.Glow.Types;
using MongoDB.Bson;
using Serilog;

#endregion

namespace JCS.Neon.Glow.Data.Repository.Mongo
{
    /// <summary>
    ///     Default implementation of <see cref="IRepository{T}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Repository<T> : IRepository<T>
        where T : RepositoryObject
    {
        /// <summary>
        /// Static logger
        /// </summary>
        private static ILogger _log = Log.ForContext<Repository<T>>();
        
        /// <summary>
        ///     Internal reference to an <see cref="IDbContext" /> instance
        /// </summary>
        private IDbContext _context;

        /// <summary>
        /// The options for this repository
        /// </summary>
        private RepositoryOptions _options;

        /// <summary>
        ///     Default constructor
        /// </summary>
        /// <param name="context">A valid instance of <see cref="IDbContext" /></param>
        public Repository(DbContext context, RepositoryOptions options)
        {
            Logging.MethodCall(_log);
            _context = context;
            _options = options;
        }

        /// <inheritdoc cref="IRepository{T}.FindOneById" />
        public Option<T> FindOneById(ObjectId id)
        {
            throw new NotImplementedException();
        }
    }
}