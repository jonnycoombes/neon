using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using JCS.Neon.Glow.Data.Entity;

namespace JCS.Neon.Glow.Data.Repository
{
    /// <summary>
    /// General repository interface for storing and manipulating items of type V which derive from <see cref="KeyedEntity{T}"/>
    /// with key type K.
    /// </summary>
    /// <typeparam name="K">Key type</typeparam>
    /// <typeparam name="V">Value type</typeparam>
    public interface IAsyncRepository<K, V> 
        where K : IComparable<K>, IEquatable<K>
        where V : KeyedEntity<K>
    {
        /// <summary>
        /// Get the total number of items within the repository
        /// </summary>
        /// <returns></returns>
        public Task<int> CountAsync();

        /// <summary>
        /// The total number of items matching a given predicate expression
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Task<long> CountAsyncWhere(Expression<Func<V, bool>> expression);
        
        /// <summary>
        /// Read one item from the repository given a key value of type K
        /// </summary>
        /// <param name="key">The key value</param>
        /// <returns>A nullable V</returns>
        public Task<V?> ReadOneAsync(K key);

        /// <summary>
        /// Reads zero of more items from the repository given a collection of
        /// key values
        /// </summary>
        /// <param name="keys">An enumeration of key values</param>
        /// <returns>A potentially empty enumeration of values</returns>
        public Task<IEnumerable<V>> ReadManyAsync(IEnumerable<K> keys);

        /// <summary>
        /// Reads all items from the repository 
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<V>> ReadAllAsync();

    }
}