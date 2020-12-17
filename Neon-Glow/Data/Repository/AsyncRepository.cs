using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        
        /// <inheritdoc cref="IAsyncRepository{K,V}.CountAsync"/> 
        public async Task<int> CountAsync()
        {
            return await _context.Set<V>().CountAsync();
        }

        /// <inheritdoc cref="IAsyncRepository{K,V}.CountAsyncWhere"/> 
        public async Task<long> CountAsyncWhere(Expression<Func<V, bool>> expression)
        {
            return await _context.Set<V>().Where(expression).CountAsync();
        }

        /// <inheritdoc cref="IAsyncRepository{K,V}.ReadOneAsync"/> 
        public async Task<V?> ReadOneAsync(K key)
        {
            try
            {
                return await _context.Set<V>()
                    .Where(v => v.Id.Equals(key))
                    .FirstAsync();
            }
            catch
            {
                return null;
            }
        }

        /// <inheritdoc cref="IAsyncRepository{K,V}.ReadManyAsync"/> 
        public async Task<IEnumerable<V>> ReadManyAsync(IEnumerable<K> keys)
        {
            return await _context.Set<V>()
                .Where(v => keys.Contains(v.Id)).ToArrayAsync();
        }

        /// <inheritdoc cref="IAsyncRepository{K,V}.ReadAllAsync"/> 
        public async Task<IEnumerable<V>> ReadAllAsync()
        {
            throw new NotImplementedException();
        }
    }
}