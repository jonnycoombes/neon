#region

using System;

#endregion

namespace JCS.Neon.Glow.Data.Repository.EntityFramework
{
    /// <summary>
    ///     Interface which defines operations that relate to the creation of <see cref="IRepository{K,V}" /> instances
    /// </summary>
    public interface IRepositoryAware
    {
        /// <summary>
        ///     Attempts to instantiate an instance of <see cref="IRepository{K,V}" /> which satifies
        ///     the type parameters.  Contexts which want to support this functionality should derive their
        ///     model elements from <see cref="Entity{T}" /> in order to ensure uniformity and consistency
        ///     in repository behaviour.
        /// </summary>
        /// <typeparam name="K">The key type of the underlying model entity type</typeparam>
        /// <typeparam name="V">
        ///     The actual type of the underlying model entity type, derived from <see cref="Entity{T}" />
        /// </typeparam>
        /// <returns></returns>
        public IRepository<K, V> CreateRepository<K, V>()
            where K : IComparable<K>, IEquatable<K>
            where V : Entity<K>;
    }
}