/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System;
using System.Linq;
using System.Threading;
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
        /// <param name="token">An optional <see cref="CancellationToken" /></param>
        /// <typeparam name="T">The type of the entities within the collection. Must have a parameterless constructor</typeparam>
        /// <returns>A <see cref="IMongoCollection{TDocument}" /> instance, bound to an undelrying collection.</returns>
        public IMongoCollection<T> Collection<T>(MongoCollectionSettings? settings = null, CancellationToken token = default)
            where T : new();

        /// <summary>
        ///     Returns a <see cref="IQueryable{T}" /> interface for interacting with objects of type <see cref="T" />
        /// </summary>
        /// <param name="settings">
        ///     An optional <see cref="MongoCollectionSettings" /> instance.  If not provided, the defaults will
        ///     be set by the context.
        /// </param>
        /// <param name="token">An optional <see cref="CancellationToken" /></param>
        /// <typeparam name="T">The type of objects stored within the underlying collection. Must have a parameterless constructor</typeparam>
        /// <returns>A <see cref="IQueryable{T}" /> interface bound to an underlying collection.</returns>
        public IQueryable<T> Queryable<T>(MongoCollectionSettings? settings = null, CancellationToken token = default) where T : new();

        /// <summary>
        /// Binds and then returns an instance of <see cref="IRepository{T}"/> for a given <see cref="RepositoryObject"/> derivative
        /// </summary>
        /// <param name="f">An optional <see cref="Action"/> which will be called to setup <see cref="RepositoryOptions"/> </param>
        /// <typeparam name="T">The type of the entities to be managed by the repository</typeparam>
        /// <returns>A bound instance of <see cref="IRepository{T}"/></returns>
        public IRepository<T> BindRepository<T>(Action<RepositoryOptionsBuilder>? f) where T : RepositoryObject;
    }
}