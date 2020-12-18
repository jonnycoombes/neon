using System;
using System.Runtime.Serialization;
using JCS.Neon.Glow.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace JCS.Neon.Glow.Data.Repository
{

    /// <summary>
    /// Exception type specific to <see cref="IAsyncRepository{K,V}"/> aware contexts
    /// </summary>
    #region Exceptions
    public class RepositoryAwareDbContextException : Exception
    {
        public RepositoryAwareDbContextException()
        {
        }

        protected RepositoryAwareDbContextException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public RepositoryAwareDbContextException(string? message) : base(message)
        {
        }

        public RepositoryAwareDbContextException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
    #endregion
    
    /// <summary>
    /// A <see cref="DbContext"/> that understands how to create instances of <see cref="IAsyncRepository{K,V}"/>
    /// based on its model elements
    /// </summary>
    public abstract class RepositoryAwareDbContext : DbContext
    {
        /// <summary>
        /// Default protected constructor which just
        /// </summary>
        /// <param name="options"></param>
        protected RepositoryAwareDbContext(DbContextOptions options) : base(options)
        {
            
        }

        /// <summary>
        /// Attempts to instantiate an instance of <see cref="IAsyncRepository{K,V}"/> which satifies
        /// the type parameters.  Contexts which want to support this functionality should derive their
        /// model elements from <see cref="KeyedEntity{T}"/> in order to ensure uniformity and consistency
        /// in repository behaviour.
        /// </summary>
        /// <typeparam name="R">The type of the repository to instantiate</typeparam>
        /// <typeparam name="K">The key type of the underlying model entity type</typeparam>
        /// <typeparam name="V">The actual type of the underlying model entity type, derived from <see cref="KeyedEntity{T}"/></typeparam>
        /// <returns></returns>
        public IAsyncRepository<K, V> CreateAsyncRepository<K, V>() 
            where K : IComparable<K>, IEquatable<K>
            where V : KeyedEntity<K>
        {
            var entityType = Model.FindEntityType(typeof(V).FullName);
            if (entityType == null)
            {
                throw new RepositoryAwareDbContextException($"Context doesn't appear to include type ({typeof(V).Name}) within model");
            }
            return new AsyncRepository<K, V>(this);
        }
        
    }
}