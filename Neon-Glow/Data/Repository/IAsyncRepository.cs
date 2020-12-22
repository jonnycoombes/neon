using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using JCS.Neon.Glow.Data.Entity;
using JCS.Neon.Glow.Types;
using Microsoft.EntityFrameworkCore;

namespace JCS.Neon.Glow.Data.Repository{
    #region Exceptions

    public class AsyncRepositoryException : Exception{
        public AsyncRepositoryException()
        {
        }

        protected AsyncRepositoryException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public AsyncRepositoryException(string? message) : base(message)
        {
        }

        public AsyncRepositoryException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }

    #endregion

    /// <summary>
    /// General repository interface for storing and manipulating items of type V which derive from <see cref="KeyedEntity{T}"/>
    /// with key type K.
    /// </summary>
    /// <typeparam name="K">Key type</typeparam>
    /// <typeparam name="V">Value type</typeparam>
    public interface IAsyncRepository<K, V>
        where K : IComparable<K>, IEquatable<K>
        where V : KeyedEntity<K>{
        /// <summary>
        /// Get the total number of items within the repository
        /// </summary>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns></returns>
        public Task<int> CountAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// The total number of items matching a given predicate expression
        /// </summary>
        /// <param name="expression">An expression which evaluates to either <c>true</c> or <c>false</c> for each item of type V</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns></returns>
        public Task<long> CountAsyncWhere(Expression<Func<V, bool>> expression, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks to see whether an item with a given key exists within the repository
        /// </summary>
        /// <param name="key">The key value to check for</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns></returns>
        public Task<bool> HasItemWithKey(K key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Read one item from the repository given a key value of type K
        /// </summary>
        /// <param name="key">The key value</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>A nullable V</returns>
        public Task<Option<V>> SelectOneAsync(K key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Read one item from the repository based on a given predicate expression
        /// </summary>
        /// <param name="expression">A predicate function which selects the item</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>Either a single item of type V or null</returns>
        public Task<Option<V>> SelectOneAsync(Expression<Func<V, bool>> expression, CancellationToken cancellationToken = default);

        /// <summary>
        /// Attempts to select a single item given an expression and if an element is found, applies function f
        /// in order to map into type W
        /// </summary>
        /// <param name="expression">The expression to select on</param>
        /// <param name="f">The projection/map function</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <typeparam name="W">The projection/map type</typeparam>
        /// <returns></returns>
        public Task<Option<W>> SelectAndProjectOneAsync<W>(Expression<Func<V, bool>> expression, Func<V, W> f,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads zero of more items from the repository given a collection of
        /// key values
        /// </summary>
        /// <param name="keys">An array of key values</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>A potentially empty enumeration of values</returns>
        public Task<IEnumerable<V>> SelectManyAsync(K[] keys, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads back the key values only for a subset of the repository matching the given predicate expression
        /// </summary>
        /// <param name="expression">A predicate expression</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns></returns>
        public Task<IEnumerable<K>> SelectManyKeysAsync(Expression<Func<V, bool>> expression,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads zero or more items from the repository given a predicate expression
        /// </summary>
        /// <param name="expression">An expression which evaluates to either <c>true</c> or <c>false</c> for each item of type V</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns></returns>
        public Task<IEnumerable<V>> SelectManyAsync(Expression<Func<V, bool>> expression, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads zero or more items from the repository based on a predicate, and then applies a projection function to
        /// transform each entry into type W
        /// </summary>
        /// <param name="expression">The expression to drive the selection</param>
        /// <param name="f">A projection/map function which takes a V, produces a W</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <typeparam name="W">The projection/map type</typeparam>
        /// <returns></returns>
        public Task<IEnumerable<W>> SelectAndProjectManyAsync<W>(Expression<Func<V, bool>> expression, Func<V, W> f,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads all items from the repository as an enumeration
        /// </summary>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns></returns>
        public IAsyncEnumerator<V> GetAsyncEnumerator(CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new item to the repository.  If the key value is set, and duplicates an existing key value then an
        /// exception may be thrown
        /// </summary>
        /// <param name="newItem">The new item. Key generation will depend on the makeup of the underlying model entity</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns></returns>
        public Task<V> CreateOneAsync(V newItem, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a sequence of new elements to the repository.
        /// </summary>
        /// <param name="newItems"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IEnumerable<V>> CreateManyAsync(V[] newItems, CancellationToken cancellationToken = default);

        /// <summary>
        /// Upserts a single item of type V.  
        /// </summary>
        /// <param name="item">The item to update</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns></returns>
        public Task<V> UpsertOneAsync(V item, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a group of items of type V.  
        /// </summary>
        /// <param name="items">An array of items to update</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns></returns>
        public Task<IEnumerable<V>> UpsertManyAsync(V[] items, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a single item from the repository
        /// </summary>
        /// <param name="item">The item to delete</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns></returns>
        public Task DeleteOneAsync(V item, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an entire array of items from the repository
        /// </summary>
        /// <param name="items">An array of items to delete</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns></returns>
        public Task DeleteManyAsync(V[] items, CancellationToken cancellationToken = default);
    }
}