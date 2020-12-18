using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using JCS.Neon.Glow.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace JCS.Neon.Glow.Data.Repository
{
    /// <summary>
    /// Default implementation of <see cref="IAsyncRepository{K,V}"/>.  This implementation essentially
    /// translates operations to an underlying <see cref="RepositoryAwareDbContext"/> instance.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public class AsyncRepository<K,V> : IAsyncRepository<K, V> 
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
            _context = context;
        }

        /// <param name="cancellationToken"></param>
        /// <inheritdoc cref="IAsyncRepository{K,V}.CountAsync()"/> 
        public async Task<int> CountAsync(CancellationToken cancellationToken= default)
        {
            return await _context.Set<V>().CountAsync(cancellationToken);
        }

        /// <inheritdoc cref="IAsyncRepository{K,V}.CountAsyncWhere"/> 
        public async Task<long> CountAsyncWhere(Expression<Func<V, bool>> expression, CancellationToken cancellationToken= default)
        {
            return await _context.Set<V>().Where(expression).CountAsync(cancellationToken);
        }

        /// <inheritdoc cref="IAsyncRepository{K,V}.HasItemWithKey"/> 
        public async Task<bool> HasItemWithKey(K key, CancellationToken cancellationToken = default)
        {
            return (await SelectOneAsync(v => v.Id.Equals(key)) != null);
        }

        /// <inheritdoc cref="IAsyncRepository{K,V}.ReadOneAsync"/> 
        public async Task<V?> SelectOneAsync(K key, CancellationToken cancellationToken= default)
        {
            try
            {
                return await _context.Set<V>()
                    .Where(v => v.Id.Equals(key))
                    .FirstAsync(cancellationToken);
            }
            catch
            {
                return null;
            }
        }

        /// <inheritdoc cref="IAsyncRepository{K,V}.ReadOneAsync"/> 
        public async Task<V?> SelectOneAsync(Expression<Func<V, bool>> expression, CancellationToken cancellationToken= default)
        {
            try
            {
                return await _context.Set<V>()
                    .Where(expression)
                    .FirstAsync(cancellationToken);
            }
            catch
            {
                return null;
            } 
        }

        /// <inheritdoc cref="IAsyncRepository{K,V}.SelectManyAsync"/> 
        public async Task<IEnumerable<V>> SelectManyAsync(K[] keys, CancellationToken cancellationToken= default)
        {
            return await _context.Set<V>()
                .Where(v => keys.Contains(v.Id)).ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc cref="IAsyncRepository{K,V}.ReadManyAsync()"/> 
        public async Task<IEnumerable<V>> SelectManyAsync(Expression<Func<V, bool>> expression, CancellationToken cancellationToken= default)
        {
            return await _context.Set<V>()
                .Where(expression).ToArrayAsync();
        }

        /// <inheritdoc cref="IAsyncRepository{K,V}.GetAsyncEnumerator"/> 
        public IAsyncEnumerator<V> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return _context.Set<V>().AsAsyncEnumerable().GetAsyncEnumerator();
        }

        /// <inheritdoc cref="IAsyncRepository{K,V}.CreateOneAsync"/>
        public async Task<V> CreateOneAsync(V newItem, CancellationToken cancellationToken = default)
        {
            try
            {
                var entityEntry= await _context.AddAsync(newItem, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return entityEntry.Entity;
            }
            catch(Exception ex)
            {
                throw new AsyncRepositoryException($"Exception caught whilst attempting to add new entries: {ex.Message}", ex);
            }
        }

        /// <inheritdoc cref="IAsyncRepository{K,V}.CreateManyAsync"/>
        public async Task<IEnumerable<V>> CreateManyAsync(V[] newItems, CancellationToken cancellationToken = default)
        {
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
            catch(Exception ex)
            {
                throw new AsyncRepositoryException($"Exception caught whilst attempting to add new entries: {ex.Message}", ex);
            }
        }

        /// <inheritdoc cref="IAsyncRepository{K,V}.SelectManyKeysAsync"/>
        public async Task<IEnumerable<K>> SelectManyKeysAsync(Expression<Func<V, bool>> expression, CancellationToken cancellationToken = default)
        {
            return (await _context.Set<V>()
                .Where(expression).ToArrayAsync()).Select(v => v.Id);
        }

        /// <inheritdoc cref="IAsyncRepository{K,V}.UpsertOneAsync"/>
        public async Task<V> UpsertOneAsync(V item, CancellationToken cancellationToken = default)
        {
            _context.Update(item);
            await _context.SaveChangesAsync(cancellationToken);
            return item;
        }

        /// <inheritdoc cref="IAsyncRepository{K,V}.UpsertManyAsync"/>
        public async Task<IEnumerable<V>> UpsertManyAsync(V[] items, CancellationToken cancellationToken = default)
        {
            _context.UpdateRange(items);
            await _context.SaveChangesAsync();
            return items;
        }

        /// <inheritdoc cref="IAsyncRepository{K,V}.DeleteOneAsync"/>
        public async Task DeleteOneAsync(V item, CancellationToken cancellationToken = default)
        {
            _context.Set<V>().Remove(item);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc cref="IAsyncRepository{K,V}.DeleteManyAsync"/>
        public async Task DeleteManyAsync(V[] items, CancellationToken cancellationToken = default)
        {
            _context.Set<V>().RemoveRange(items);
            await _context.SaveChangesAsync();
        }
    }
}