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
        ///     The options for this repository
        /// </summary>
        private readonly RepositoryOptions _options;

        /// <summary>
        ///     The underlying <see cref="IMongoClient" />
        /// </summary>
        private IMongoClient _client;

        /// <summary>
        ///     Internal reference to an <see cref="IDbContext" /> instance
        /// </summary>
        private IDbContext _context;

        /// <summary>
        ///     Default constructor
        /// </summary>
        /// <param name="context">A valid instance of <see cref="IDbContext" /></param>
        /// <param name="options">The options for the repository</param>
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

        /// <summary>
        ///     Constructor which takes a pre-bound collection
        /// </summary>
        /// <param name="context">The supporting <see cref="IDbContext" /></param>
        /// <param name="collection">A pre-bound <see cref="IMongoCollection{TDocument}" /></param>
        /// <param name="options">The options for the repository</param>
        public Repository(DbContext context, IMongoCollection<T> collection, RepositoryOptions options)
        {
            Logging.MethodCall(_log);
            _context = context;
            _options = options;
            _collection = collection;
            _client = _collection.Database.Client;
        }

        /// <inheritdoc cref="IRepository{T}.ReadOneById" />
        public async Task<Option<T>> ReadOne(ObjectId id)
        {
            Logging.MethodCall(_log);
            try
            {
                using var cursor = await _collection.FindAsync(AdjustFilterForDeletions(IdFilter(id)));
                var documents = await cursor.ToListAsync();
                if (documents.Count == 1)
                {
                    return Option<T>.Some(documents.First());
                }

                return Option<T>.None;
            }
            catch (Exception ex)
            {
                throw Exceptions.LoggedException<RepositoryException>(_log, $"Repository exception: \"{ex.Message}\"", ex);
            }
        }

        public async Task<Option<T>> ReadOne(Func<FilterDefinition<T>> filter)
        {
            Logging.MethodCall(_log);
            try
            {
                using var cursor = await _collection.FindAsync(AdjustFilterForDeletions(filter()));
                var documents = await cursor.ToListAsync();
                if (documents.Count == 1)
                {
                    return Option<T>.Some(documents.First());
                }

                return Option<T>.None;
            }
            catch (Exception ex)
            {
                throw Exceptions.LoggedException<RepositoryException>(_log, $"Repository exception: \"{ex.Message}\"", ex);
            }
        }

        /// <inheritdoc cref="IRepository{T}.ReadMany(MongoDB.Bson.ObjectId[])" />
        public async Task<T[]> ReadMany(ObjectId[] ids)
        {
            Logging.MethodCall(_log);
            try
            {
                using var cursor = await _collection.FindAsync(AdjustFilterForDeletions(Builders<T>.Filter.In(t => t.Id, ids)));
                return (await cursor.ToListAsync()).ToArray();
            }
            catch (Exception ex)
            {
                throw Exceptions.LoggedException<RepositoryException>(_log, $"Repository exception: \"{ex.Message}\"", ex);
            }
        }

        /// <inheritdoc cref="IRepository{T}.ReadMany(System.Func{MongoDB.Driver.FilterDefinition{T}})" />
        public async Task<T[]> ReadMany(Func<FilterDefinition<T>> filter)
        {
            Logging.MethodCall(_log);
            try
            {
                using var cursor = await _collection.FindAsync(AdjustFilterForDeletions(filter()));
                return (await cursor.ToListAsync()).ToArray();
            }
            catch (Exception ex)
            {
                throw Exceptions.LoggedException<RepositoryException>(_log, $"Repository exception: \"{ex.Message}\"", ex);
            }
        }

        /// <inheritdoc cref="IRepository{T}.MapOneById{V}" />
        public async Task<Option<V>> MapOne<V>(ObjectId id, Func<T, Option<V>> map) where V : notnull
        {
            if ((await ReadOne(id)).IsSome(out var value))
            {
                return map(value);
            }

            return Option<V>.None;
        }

        /// <inheritdoc cref="IRepository{T}.CreateOne" />
        public async Task<T> CreateOne(T value)
        {
            Logging.MethodCall(_log);
            try
            {
                value.CreatedAt = DateTime.UtcNow;
                await _collection.InsertOneAsync(value);
                return value;
            }
            catch (Exception ex)
            {
                throw Exceptions.LoggedException<RepositoryException>(_log, $"Repository exception: \"{ex.Message}\"", ex);
            }
        }

        /// <inheritdoc cref="IRepository{T}.DeleteOne(T)" />
        public async Task DeleteOne(T value)
        {
            Logging.MethodCall(_log);
            try
            {
                switch (_options.DeletionBehaviour)
                {
                    case RepositoryOptions.DeletionBehaviourOption.Hard:
                        await _collection.DeleteOneAsync(Builders<T>.Filter.Eq(t => t.Id, value.Id));
                        value.Id = new ObjectId();
                        break;
                    case RepositoryOptions.DeletionBehaviourOption.Soft:
                        value.Deleted = true;
                        value.DeletedAt = DateTime.UtcNow;
                        await UpdateOne(value);
                        break;
                }
            }
            catch (Exception ex)
            {
                throw Exceptions.LoggedException<RepositoryException>(_log, $"Repository exception: \"{ex.Message}\"", ex);
            }
        }

        /// <inheritdoc cref="IRepository{T}.DeleteOne(MongoDB.Bson.ObjectId)" />
        public async Task DeleteOne(ObjectId Id)
        {
            Logging.MethodCall(_log);
            try
            {
                switch (_options.DeletionBehaviour)
                {
                    case RepositoryOptions.DeletionBehaviourOption.Hard:
                        await _collection.DeleteOneAsync(Builders<T>.Filter.Eq(t => t.Id, Id));
                        break;
                    case RepositoryOptions.DeletionBehaviourOption.Soft:
                        var result = await ReadOne(() => IdFilter(Id));
                        if (result.IsSome(out var value))
                        {
                            value.Deleted = true;
                            value.DeletedAt = DateTime.UtcNow;
                            await UpdateOne(value);
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                throw Exceptions.LoggedException<RepositoryException>(_log, $"Repository exception: \"{ex.Message}\"", ex);
            }
        }

        /// <inheritdoc cref="IRepository{T}.UpdateOne" />
        public async Task<T> UpdateOne(T value)
        {
            Logging.MethodCall(_log);
            try
            {
                value.LastModified = DateTime.UtcNow;
                value.VersionToken.Increment();
                return await _collection.FindOneAndReplaceAsync(IdFilter(value), value,
                    new FindOneAndReplaceOptions<T>
                    {
                        ReturnDocument = ReturnDocument.After
                    });
            }
            catch (Exception ex)
            {
                throw Exceptions.LoggedException<RepositoryException>(_log, $"Repository exception: \"{ex.Message}\"", ex);
            }
        }

        /// <summary>
        ///     Takes a pre-defined <see cref="FilterDefinition{TDocument}" /> and then applies valid deletion clause to it
        /// </summary>
        /// <param name="filter">The pre-existing filter</param>
        /// <returns></returns>
        private FilterDefinition<T> AdjustFilterForDeletions(FilterDefinition<T> filter)
        {
            if (_options.ReadBehaviour == RepositoryOptions.ReadBehaviourOption.IgnoreDeleted)
            {
                return Builders<T>.Filter.And(filter, DeletionFilter());
            }

            return filter;
        }

        /// <summary>
        ///     Creates a filter definition for ignoring or including deleted items
        /// </summary>
        /// <param name="status">The deletion status</param>
        /// <returns></returns>
        private static FilterDefinition<T> DeletionFilter(bool status = false)
        {
            return Builders<T>.Filter.Eq(t => t.Deleted, status);
        }

        /// <summary>
        ///     Builds a filter matching on a given object (of type <typeparamref name="T" />) id
        /// </summary>
        /// <param name="value">An instance deriving from <see cref="RepositoryObject" /></param>
        /// <returns>A new filter definition which matches on the object id</returns>
        private static FilterDefinition<T> IdFilter(T value)
        {
            return Builders<T>.Filter.Eq(t => t.Id, value.Id);
        }

        /// <summary>
        ///     Builds an id filter given a <see cref="ObjectId" />
        /// </summary>
        /// <param name="id">The <see cref="ObjectId" /> to build the filter on</param>
        /// <returns></returns>
        private static FilterDefinition<T> IdFilter(ObjectId id)
        {
            return Builders<T>.Filter.Eq(t => t.Id, id);
        }
    }
}