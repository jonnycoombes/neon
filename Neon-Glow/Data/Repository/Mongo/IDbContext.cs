#region

using System.Linq;
using MongoDB.Driver;

#endregion

namespace JCS.Neon.Glow.Data.Repository.Mongo
{
    /// <summary>
    ///     Interface definition for a database context backed by Mongo DB
    /// </summary>
    public interface IDbContext
    {
        /// <summary>
        ///     The <see cref="MongoClient" /> currently being used by the context
        /// </summary>
        public MongoClient Client { get; }

        /// <summary>
        ///     The currently configured set of <see cref="DbContextOptions" /> being used by the context
        /// </summary>
        public DbContextOptions Options { get; }

        /// <summary>
        ///     The bound <see cref="IMongoDatabase" /> being used by the context
        /// </summary>
        public IMongoDatabase Database { get; }

        /// <summary>
        ///     Checks whether a given database exists, based on the current <see cref="MongoClient" /> settings
        /// </summary>
        /// <param name="databaseName">The name of the database to check for</param>
        /// <returns><code>true</code> if the database exists, <code>false</code> otherwise</returns>
        public bool DatabaseExists(string databaseName);

        /// <summary>
        ///     Checks whether a given collection exists, based on the current <see cref="MongoClient" /> settings and database
        /// </summary>
        /// <param name="collectionName">The name of the collection to check</param>
        /// <returns><code>true</code> if the database exists, <code>false</code> otherwise</returns>
        public bool CollectionExists(string collectionName);

        /// <summary>
        ///     Returns a typed collection interface for interacting with objects of type <see cref="T" />
        /// </summary>
        /// <param name="settings">
        ///     An optional <see cref="MongoCollectionSettings" /> instance.  If not provided, the defaults will
        ///     be set by the context.
        /// </param>
        /// <typeparam name="T">The type of the entities within the collection</typeparam>
        /// <returns>A <see cref="IMongoCollection{TDocument}" /> instance, bound to an undelrying collection.</returns>
        public IMongoCollection<T> Collection<T>(MongoCollectionSettings? settings);

        /// <summary>
        ///     Returns a <see cref="IQueryable{T}" /> interface for interacting with objects of type <see cref="T" />
        /// </summary>
        /// <param name="settings">
        ///     An optional <see cref="MongoCollectionSettings" /> instance.  If not provided, the defaults will
        ///     be set by the context.
        /// </param>
        /// <typeparam name="T">The type of objects stored within the underlying collection</typeparam>
        /// <returns>A <see cref="IQueryable{T}" /> interface bound to an underlying collection.</returns>
        public IQueryable<T> Queryable<T>(MongoCollectionSettings? settings);
    }
}