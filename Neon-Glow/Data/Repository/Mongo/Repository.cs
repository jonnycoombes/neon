/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System;
using System.Collections.Generic;
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

        /// <inheritdoc cref="IRepository{T}.Count" />
        public async Task<long> Count()
        {
            try
            {
                return await _collection.CountDocumentsAsync(AdjustFilterForDeletions(Builders<T>.Filter.Empty));
            }
            catch (Exception ex)
            {
                throw Exceptions.LoggedException<RepositoryException>(_log, $"Repository exception: \"{ex.Message}\"", ex);
            }
        }

        /// <inheritdoc cref="IRepository{T}.Count(System.Func{MongoDB.Driver.FilterDefinition{T}})" />
        public async Task<long> Count(Func<FilterDefinition<T>> filter)
        {
            try
            {
                return await _collection.CountDocumentsAsync(AdjustFilterForDeletions(filter()));
            }
            catch (Exception ex)
            {
                throw Exceptions.LoggedException<RepositoryException>(_log, $"Repository exception: \"{ex.Message}\"", ex);
            }
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

                if (documents.Count > 1)
                {
                    throw Exceptions.LoggedException<RepositoryException>(_log, $"Multiple repository items with same id \"{id}\"");
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
            Logging.MethodCall(_log);
            if ((await ReadOne(id)).IsSome(out var value))
            {
                return map(value);
            }

            return Option<V>.None;
        }

        /// <inheritdoc cref="IRepository{T}.MapMany{V}" />
        public async Task<Option<V>[]> MapMany<V>(Func<FilterDefinition<T>> filter, Func<T, Option<V>> map) where V : notnull
        {
            Logging.MethodCall(_log);
            try
            {
                var results = new List<Option<V>>();
                foreach (var x in await ReadMany(filter))
                {
                    results.Add(map(x));
                }

                return results.ToArray();
            }
            catch (Exception ex)
            {
                throw Exceptions.LoggedException<RepositoryException>(_log, $"Repository exception: \"{ex.Message}\"", ex);
            }
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

        /// <inheritdoc cref="IRepository{T}.CreateMany" />
        public async Task<T[]> CreateMany(T[] values)
        {
            Logging.MethodCall(_log);
            try
            {
                var models = new InsertOneModel<T>[values.Length];
                for (var i = 0; i < values.Length; i++)
                {
                    values[i].CreatedAt = DateTime.UtcNow;
                    models[i] = new InsertOneModel<T>(values[i]);
                }

                await _collection.BulkWriteAsync(models, new BulkWriteOptions());
                return values;
            }
            catch (Exception ex)
            {
                throw Exceptions.LoggedException<RepositoryException>(_log, $"Repository exception: \"{ex.Message}\"", ex);
            }
        }

        /// <inheritdoc cref="IRepository{T}.CreateMany(long,System.Func{int,System.Threading.Tasks.Task{T}})" />
        public async Task<T[]> CreateMany(long count, Func<int, T> generator)
        {
            Logging.MethodCall(_log);
            try
            {
                var models = new InsertOneModel<T>[count];
                var created = new T[count];
                for (var i = 0; i < count; i++)
                {
                    created[i] = generator(i);
                    created[i].CreatedAt = DateTime.UtcNow;
                    models[i] = new InsertOneModel<T>(created[i]);
                }

                await _collection.BulkWriteAsync(models, new BulkWriteOptions());
                return created;
            }
            catch (Exception ex)
            {
                throw Exceptions.LoggedException<RepositoryException>(_log, $"Repository exception: \"{ex.Message}\"", ex);
            }
        }

        /// <inheritdoc cref="IRepository{T}.DeleteOne(T)" />
        public async Task<long> DeleteOne(T value)
        {
            Logging.MethodCall(_log);
            try
            {
                long deleted;
                if (_options.DeletionBehaviour == RepositoryOptions.DeletionBehaviourOption.Hard)
                {
                    var result = await _collection.DeleteOneAsync(Builders<T>.Filter.Eq(t => t.Id, value.Id));
                    value.Id = new ObjectId();
                    deleted = result.DeletedCount;
                }
                else
                {
                    value.Deleted = true;
                    value.DeletedAt = DateTime.UtcNow;
                    await UpdateOne(value);
                    deleted = 1;
                }

                return deleted;
            }
            catch (Exception ex)
            {
                throw Exceptions.LoggedException<RepositoryException>(_log, $"Repository exception: \"{ex.Message}\"", ex);
            }
        }

        /// <inheritdoc cref="IRepository{T}.DeleteOne(MongoDB.Bson.ObjectId)" />
        public async Task<long> DeleteOne(ObjectId Id)
        {
            Logging.MethodCall(_log);
            try
            {
                long deleted = 0;
                if (_options.DeletionBehaviour == RepositoryOptions.DeletionBehaviourOption.Hard)
                {
                    var result = await _collection.DeleteOneAsync(Builders<T>.Filter.Eq(t => t.Id, Id));
                    deleted = result.DeletedCount;
                }
                else
                {
                    var result = await ReadOne(() => IdFilter(Id));
                    if (result.IsSome(out var value))
                    {
                        value.Deleted = true;
                        value.DeletedAt = DateTime.UtcNow;
                        await UpdateOne(value);
                        deleted = 1;
                    }
                }

                return deleted;
            }
            catch (Exception ex)
            {
                throw Exceptions.LoggedException<RepositoryException>(_log, $"Repository exception: \"{ex.Message}\"", ex);
            }
        }

        /// <inheritdoc cref="IRepository{T}.DeleteMany(T[])" />
        public async Task<long> DeleteMany(T[] values)
        {
            Logging.MethodCall(_log);
            try
            {
                long deleted = 0;
                if (_options.DeletionBehaviour == RepositoryOptions.DeletionBehaviourOption.Hard)
                {
                    var models = new DeleteOneModel<T>[values.Length];
                    for (var i = 0; i < values.Length; i++)
                    {
                        models[i] = new DeleteOneModel<T>(IdFilter(values[i].Id));
                    }

                    var result = await _collection.BulkWriteAsync(models);
                    deleted = result.DeletedCount;
                }
                else
                {
                    var models = new ReplaceOneModel<T>[values.Length];
                    for (var i = 0; i < values.Length; i++)
                    {
                        values[i].Deleted = true;
                        values[i].DeletedAt = DateTime.UtcNow;
                        models[i] = new ReplaceOneModel<T>(IdFilter(values[i].Id), values[i]);
                    }

                    var result = await _collection.BulkWriteAsync(models);
                    deleted = result.ModifiedCount;
                }

                return deleted;
            }
            catch (Exception ex)
            {
                throw Exceptions.LoggedException<RepositoryException>(_log, $"Repository exception: \"{ex.Message}\"", ex);
            }
        }

        /// <inheritdoc cref="IRepository{T}.DeleteMany(System.Func{MongoDB.Driver.FilterDefinition{T}})" />
        public async Task<long> DeleteMany(Func<FilterDefinition<T>> filter)
        {
            Logging.MethodCall(_log);
            try
            {
                long deleted = 0;
                if (_options.DeletionBehaviour == RepositoryOptions.DeletionBehaviourOption.Hard)
                {
                    var result = await _collection.DeleteManyAsync(AdjustFilterForDeletions(filter()));
                    deleted = result.DeletedCount;
                }
                else
                {
                    var deletions = await ReadMany(filter);
                    foreach (var d in deletions)
                    {
                        d.Deleted = true;
                    }

                    await UpdateMany(deletions);
                    deleted = deletions.Length;
                }

                return deleted;
            }
            catch (Exception ex)
            {
                throw Exceptions.LoggedException<RepositoryException>(_log, $"Repository exception: \"{ex.Message}\"", ex);
            }
        }

        /// <inheritdoc cref="IRepository{T}.Purge" />
        public async Task<long> Purge()
        {
            Logging.MethodCall(_log);
            try
            {
                var result = await _collection.DeleteManyAsync(DeletionFilter(true));
                return result.DeletedCount;
            }
            catch (Exception ex)
            {
                throw Exceptions.LoggedException<RepositoryException>(_log, $"Repository exception: \"{ex.Message}\"", ex);
            }
        }

        /// <inheritdoc cref="IRepository{T}.Purge(System.Func{MongoDB.Driver.FilterDefinition{T}})" />
        public async Task<long> Purge(Func<FilterDefinition<T>> filter)
        {
            Logging.MethodCall(_log);
            try
            {
                var result = await _collection.DeleteManyAsync(Builders<T>.Filter.And(DeletionFilter(true), filter()));
                return result.DeletedCount;
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

        /// <inheritdoc cref="IRepository{T}.UpdateMany" />
        public async Task<T[]> UpdateMany(T[] values)
        {
            Logging.MethodCall(_log);
            try
            {
                var models = new ReplaceOneModel<T>[values.Length];
                for (var i = 0; i < values.Length; i++)
                {
                    values[i].LastModified = DateTime.UtcNow;
                    values[i].VersionToken.Increment();
                    models[i] = new ReplaceOneModel<T>(IdFilter(values[i].Id), values[i]);
                }

                var result = await _collection.BulkWriteAsync(models);
                return values;
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