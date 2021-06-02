/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System;
using System.Linq;
using System.Threading.Tasks;
using JCS.Neon.Glow.Statics;
using JCS.Neon.Glow.Types;
using MongoDB.Bson;
using MongoDB.Driver;
using Serilog;

#endregion

namespace JCS.Neon.Glow.Data.Repository.Mongo
{
    /// <summary>
    ///     Default implementation of <see cref="IRepository{T}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Repository<T> : IRepository<T>
        where T : RepositoryObject, new()
    {
        /// <summary>
        ///     Static logger
        /// </summary>
        private static readonly ILogger _log = Log.ForContext<Repository<T>>();

        /// <summary>
        ///     The internal <see cref="IMongoCollection{TDocument}" /> utilised by this repository
        /// </summary>
        private readonly IMongoCollection<T> _collection;

        /// <summary>
        ///     Internal reference to an <see cref="IDbContext" /> instance
        /// </summary>
        private IDbContext _context;

        /// <summary>
        ///     The options for this repository
        /// </summary>
        private RepositoryOptions _options;

        /// <summary>
        /// The underlying <see cref="IMongoClient"/>
        /// </summary>
        private IMongoClient _client;

        /// <summary>
        ///     Default constructor
        /// </summary>
        /// <param name="context">A valid instance of <see cref="IDbContext" /></param>
        public Repository(DbContext context, RepositoryOptions options)
        {
            Logging.MethodCall(_log);
            _context = context;
            _options = options;
            _collection = context.Collection<T>(builder =>
            {
                builder
                    .ReadConcern(options.ReadConcern)
                    .WriteConcern(options.WriteConcern);
            });
            _client = _collection.Database.Client;
        }

        /// <inheritdoc cref="IRepository{T}.ReadOneById" />
        public async Task<Option<T>> ReadOne(ObjectId id)
        {
            Logging.MethodCall(_log);
            try
            {
                var result = await _collection.FindAsync(Builders<T>.Filter.Eq(t => t.Id, id));
                await result.MoveNextAsync();
                if (result.Current.Any())
                {
                    return Option<T>.Some(result.Current.First());
                }
                else
                {
                    return Option<T>.None;
                }
            }
            catch (Exception ex)
            {
                throw Exceptions.LoggedException<RepositoryException>(_log, $"Repository exception: \"{ex.Message}\"", ex);
            }
        }

        /// <inheritdoc cref="IRepository{T}.MapOneById{V}"/>
        public async Task<Option<V>> MapOne<V>(ObjectId id, Func<T, V> fMap) where V : notnull
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="IRepository{T}.CreateOne" />
        public async Task<T> CreateOne(T value)
        {
            Logging.MethodCall(_log);
            try
            {
                value.CreatedAt = DateTime.Now;
                await _collection.InsertOneAsync(value);
                return value;
            }
            catch (Exception ex)
            {
                throw Exceptions.LoggedException<RepositoryException>(_log, $"Repository exception: \"{ex.Message}\"", ex);
            }
        }

        /// i<inheritdoc cref="IRepository{T}.DeleteOne(T)"/>
        public async Task DeleteOne(T value)
        {
            Logging.MethodCall(_log);
            try
            {
                await _collection.DeleteOneAsync(Builders<T>.Filter.Eq(t => t.Id, value.Id));
                value.Id = new ObjectId();
            }
            catch (Exception ex)
            {
                throw Exceptions.LoggedException<RepositoryException>(_log, $"Repository exception: \"{ex.Message}\"", ex);
            }
        }

        /// <inheritdoc cref="IRepository{T}.DeleteOne(MongoDB.Bson.ObjectId)"/>
        public async Task DeleteOne(ObjectId Id)
        {
            Logging.MethodCall(_log);
            try
            {
                await _collection.DeleteOneAsync(Builders<T>.Filter.Eq(t => t.Id, Id));
            }
            catch (Exception ex)
            {
                throw Exceptions.LoggedException<RepositoryException>(_log, $"Repository exception: \"{ex.Message}\"", ex);
            }
        }

        /// <inheritdoc cref="IRepository{T}.UpdateOne"/>
        public async Task<T> UpdateOne(T value)
        {
            throw new NotImplementedException();
        }
    }
}