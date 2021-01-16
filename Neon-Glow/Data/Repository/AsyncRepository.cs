using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using JCS.Neon.Glow.Data.Entity;
using JCS.Neon.Glow.Types;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Serilog;
using static JCS.Neon.Glow.Helpers.General.ExceptionHelpers;
using static JCS.Neon.Glow.Helpers.General.LogHelpers;

namespace JCS.Neon.Glow.Data.Repository
{
    /// <summary>
    /// Default implementation of <see cref="IAsyncRepository{K,V}"/>.  This implementation essentially
    /// translates operations to an underlying <see cref="RepositoryAwareDbContext"/> instance.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public class AsyncRepository<K, V> : IAsyncRepository<K, V>
        where K : IComparable<K>, IEquatable<K>
        where V : KeyedEntity<K>
    {
        /// <summary>
        /// The undelrying context instace
        /// </summary>
        private RepositoryAwareDbContext _context;

        /// <summary>
        /// Constructor which takes a supporting <see cref="RepositoryAwareDbContext"/> instance
        /// </summary>
        /// <param name="context"></param>
        public AsyncRepository(RepositoryAwareDbContext context)
        {
            LogMethodCall(_log);
            _context = context;
        }

        /// <summary>
        /// <see cref="ILogger"/> instance
        /// </summary>
        private ILogger _log => Log.ForContext(typeof(AsyncRepository<K, V>));

        /// <param name="cancellationToken"></param>
        /// <inheritdoc cref="IAsyncRepository{K,V}.Count"/> 
        public async Task<int> Count(CancellationToken cancellationToken = default)
        {
            LogMethodCall(_log);
            try
            {
                return await _context.Set<V>().CountAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw DbSpecificException(ex);
            }
        }

        /// <inheritdoc cref="IAsyncRepository{K,V}.CountWhere"/> 
        public async Task<long> CountWhere(Expression<Func<V, bool>> expression, CancellationToken cancellationToken = default)
        {
            LogMethodCall(_log);
            try
            {
                return await _context.Set<V>().Where(expression).CountAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw DbSpecificException(ex);
            }
        }

        /// <inheritdoc cref="IAsyncRepository{K,V}.HasItemWithKey"/> 
        public async Task<bool> HasItemWithKey(K key, CancellationToken cancellationToken = default)
        {
            LogMethodCall(_log);
            try
            {
                return !(await SelectOne(v => v.Id.Equals(key))).IsNone;
            }
            catch (Exception ex)
            {
                throw DbSpecificException(ex);
            }
        }

        /// <inheritdoc cref="IAsyncRepository{K,V}.ReadOneAsync"/> 
        public async Task<Option<V>> SelectOne(K key, CancellationToken cancellationToken = default)
        {
            LogMethodCall(_log);
            try
            {
                return Option<V>.Some(await _context.Set<V>()
                    .Where(v => v.Id.Equals(key))
                    .FirstAsync(cancellationToken));
            }
            catch
            {
                return Option<V>.None;
            }
        }

        /// <inheritdoc cref="IAsyncRepository{K,V}.ReadOneAsync"/> 
        public async Task<Option<V>> SelectOne(Expression<Func<V, bool>> expression, CancellationToken cancellationToken = default)
        {
            LogMethodCall(_log);
            try
            {
                return Option<V>.Some(await _context.Set<V>()
                    .Where(expression)
                    .FirstAsync(cancellationToken));
            }
            catch
            {
                return Option<V>.None;
            }
        }

        /// <inheritdoc cref="IAsyncRepository{K,V}.SelectAndProjectOne{W}"/> 
        public async Task<Option<W>> SelectAndProjectOne<W>(Expression<Func<V, bool>> expression, Func<V, W> f,
            CancellationToken cancellationToken = default)
        {
            LogMethodCall(_log);
            var selection = await SelectOne(expression, cancellationToken);
            return selection.Fold(
                v => Option<W>.Some(f(v)),
                () => Option<W>.None);
        }

        /// <inheritdoc cref="IAsyncRepository{K,V}.SelectManyAsync"/> 
        public async Task<IEnumerable<V>> SelectMany(K[] keys, CancellationToken cancellationToken = default)
        {
            LogMethodCall(_log);
            try
            {
                return await _context.Set<V>()
                    .Where(v => keys.Contains(v.Id)).ToArrayAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw DbSpecificException(ex);
            }
        }

        /// <inheritdoc cref="IAsyncRepository{K,V}.ReadManyAsync()"/> 
        public async Task<IEnumerable<V>> SelectMany(Expression<Func<V, bool>> expression,
            CancellationToken cancellationToken = default)
        {
            LogMethodCall(_log);
            try
            {
                return await _context.Set<V>()
                    .Where(expression).ToArrayAsync();
            }
            catch (Exception ex)
            {
                throw DbSpecificException(ex);
            }
        }

        /// <inheritdoc cref="IAsyncRepository{K,V}.SelectAndProjectMany{W}"/> 
        public async Task<IEnumerable<W>> SelectAndProjectMany<W>(Expression<Func<V, bool>> expression, Func<V, W> f,
            CancellationToken cancellationToken = default)
        {
            LogMethodCall(_log);
            var selection = await SelectMany(expression, cancellationToken);
            return selection.Select(v => f(v));
        }

        /// <inheritdoc cref="IAsyncRepository{K,V}.GetEnumerator"/> 
        public IAsyncEnumerator<V> GetEnumerator(CancellationToken cancellationToken = default)
        {
            LogMethodCall(_log);
            try
            {
                return _context.Set<V>().AsAsyncEnumerable().GetAsyncEnumerator();
            }
            catch (Exception ex)
            {
                throw DbSpecificException(ex);
            }
        }

        /// <inheritdoc cref="IAsyncRepository{K,V}.CreateOne"/>
        public async Task<V> CreateOne(V newItem, CancellationToken cancellationToken = default)
        {
            LogMethodCall(_log);
            try
            {
                var entityEntry = await _context.AddAsync(newItem, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return entityEntry.Entity;
            }
            catch (Exception ex)
            {
                throw DbSpecificException(ex);
            }
        }

        /// <inheritdoc cref="IAsyncRepository{K,V}.CreateMany"/>
        public async Task<IEnumerable<V>> CreateMany(V[] newItems, CancellationToken cancellationToken = default)
        {
            LogMethodCall(_log);
            try
            {
                for (var i = 0; i < newItems.Length; i++)
                {
                    var entityEntry = await _context.AddAsync(newItems[i]);
                    newItems[i] = entityEntry.Entity;
                }

                await _context.SaveChangesAsync();
                return newItems;
            }
            catch (Exception ex)
            {
                throw DbSpecificException(ex);
            }
        }

        /// <inheritdoc cref="IAsyncRepository{K,V}.SelectManyKeys"/>
        public async Task<IEnumerable<K>> SelectManyKeys(Expression<Func<V, bool>> expression,
            CancellationToken cancellationToken = default)
        {
            LogMethodCall(_log);
            return await SelectAndProjectMany<K>(expression, v => v.Id, cancellationToken);
        }

        /// <inheritdoc cref="IAsyncRepository{K,V}.UpsertOne"/>
        public async Task<V> UpsertOne(V item, CancellationToken cancellationToken = default)
        {
            LogMethodCall(_log);
            try
            {
                _context.Update(item);
                await _context.SaveChangesAsync(cancellationToken);
                return item;
            }
            catch (Exception ex)
            {
                throw DbSpecificException(ex);
            }
        }

        /// <inheritdoc cref="IAsyncRepository{K,V}.UpsertMany"/>
        public async Task<IEnumerable<V>> UpsertMany(V[] items, CancellationToken cancellationToken = default)
        {
            LogMethodCall(_log);
            try
            {
                _context.UpdateRange(items);
                await _context.SaveChangesAsync();
                return items;
            }
            catch (Exception ex)
            {
                throw DbSpecificException(ex);
            }
        }

        /// <inheritdoc cref="IAsyncRepository{K,V}.DeleteOne"/>
        public async Task DeleteOne(V item, CancellationToken cancellationToken = default)
        {
            LogMethodCall(_log);
            try
            {
                _context.Set<V>().Remove(item);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw DbSpecificException(ex);
            }
        }

        /// <inheritdoc cref="IAsyncRepository{K,V}.DeleteMany"/>
        public async Task DeleteMany(V[] items, CancellationToken cancellationToken = default)
        {
            LogMethodCall(_log);
            try
            {
                _context.Set<V>().RemoveRange(items);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw DbSpecificException(ex);
            }
        }

        /// <summary>
        /// Method that will inspect a general exception and see if it can determine any detailed information from
        /// the exception, dependent on the database provider
        /// </summary>
        /// <param name="ex"></param>
        /// <exception cref="AsyncRepositoryException"></exception>
        protected Exception DbSpecificException(Exception ex)
        {
            LogMethodCall(_log);
            switch (ex.InnerException)
            {
                case PostgresException pex:
                    var message = $"{pex.MessageText}";
                    return LoggedException<AsyncRepositoryException>(_log, message, pex);
                default:
                    return LoggedException<AsyncRepositoryException>(_log,
                        $"DB Exception caught: \"{ex.Message}\"", ex);
            }
        }
    }
}