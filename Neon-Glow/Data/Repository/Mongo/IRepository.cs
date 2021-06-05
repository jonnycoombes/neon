/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System;
using System.Threading.Tasks;
using JCS.Neon.Glow.Types;
using MongoDB.Bson;
using MongoDB.Driver;

#endregion

namespace JCS.Neon.Glow.Data.Repository.Mongo
{
    /// <summary>
    ///     Interface defining the basic operations which need to be supported by a repository backed by a typed mongo collection.  The
    ///     repository functionality is dependent on the stored entities being derived (sub-classes) of
    ///     <see cref="RepositoryObject" /> which ensures that certain basic functionality can be implemented in a consistent manner.
    /// </summary>
    /// <typeparam name="T">The type of entity managed within the repository</typeparam>
    public interface IRepository<T>
        where T : RepositoryObject
    {
        /// <summary>
        ///     Counts all the items currently within the repository. Takes into account soft deletions.
        /// </summary>
        /// <returns>A count of objects in the repository. Note that if soft deletion is used, deleted objects will not be included</returns>
        /// <exception cref="RepositoryException"></exception>
        public Task<long> Count();

        /// <summary>
        ///     Counts all the items in the repository matching a speific filter.  Takes into account soft deletions
        /// </summary>
        /// <param name="filter">A filter lambda</param>
        /// <returns>A count of objects matching the <paramref name="filter" />. Does not include items that have been soft deleted</returns>
        /// <exception cref="RepositoryException"></exception>
        public Task<long> Count(Func<FilterDefinition<T>> filter);

        /// <summary>
        ///     Read one item from the repository, based on id
        /// </summary>
        /// <param name="id">The <see cref="ObjectId" /> of the object</param>
        /// <returns>An <see cref="Option{T}" /></returns>
        /// <exception cref="RepositoryException"></exception>
        public Task<Option<T>> ReadOne(ObjectId id);

        /// <summary>
        ///     Reads one item from the repository, based on a filter
        /// </summary>
        /// <param name="filter">Lambda which returns a filter definition</param>
        /// <returns>An <see cref="Option" /> containing (or not) the first object matching the filter</returns>
        /// <exception cref="RepositoryException"></exception>
        public Task<Option<T>> ReadOne(Func<FilterDefinition<T>> filter);

        /// <summary>
        ///     Reads many items from the repository, based on an array of object ids
        /// </summary>
        /// <param name="ids">An array of <see cref="ObjectId" /></param>
        /// <returns>A (possibly empty) array of objects type <typeparamref name="T" /></returns>
        /// <exception cref="RepositoryException"></exception>
        public Task<T[]> ReadMany(ObjectId[] ids);

        /// <summary>
        ///     Reads many items from the repository, based on a filter
        /// </summary>
        /// <param name="filter">A filter producing lambda</param>
        /// <returns>A (possibly empty) array of objects type <typeparamref name="T" /></returns>
        /// <exception cref="RepositoryException"></exception>
        public Task<T[]> ReadMany(Func<FilterDefinition<T>> filter);

        /// <summary>
        ///     Retrieves a single item from the repository and then applies a mapping function to the result.  This is basically
        ///     a projection operation
        /// </summary>
        /// <param name="id">The <see cref="ObjectId" /> of the object to retrieve</param>
        /// <param name="map">A function that will map between type <typeparamref name="T" /> and <typeparamref name="V" /></param>
        /// <typeparam name="V">The mapped type</typeparam>
        /// <returns>An <see cref="Option{T}" /></returns>
        /// <exception cref="RepositoryException"></exception>
        public Task<Option<V>> MapOne<V>(ObjectId id, Func<T, Option<V>> map) where V : notnull;

        /// <summary>
        ///     Reads many items from the repository and then applies a mapping function <paramref name="map" /> to each item in turn
        /// </summary>
        /// <param name="filter">A <see cref="FilterDefinition{TDocument}" /> generating lambda</param>
        /// <param name="map">A mapping function</param>
        /// <typeparam name="V">The return type for the mapping function</typeparam>
        /// <returns>An array of <see cref="Option{T}" /> objects</returns>
        /// <exception cref="RepositoryException"></exception>
        public Task<Option<V>[]> MapMany<V>(Func<FilterDefinition<T>> filter, Func<T, Option<V>> map) where V : notnull;

        /// <summary>
        ///     Creates a single new item within the repository
        /// </summary>
        /// <param name="value">The object to store</param>
        /// <returns>The stored value, with updated fields including the <see cref="ObjectId" /> and creation date</returns>
        /// <exception cref="RepositoryException"></exception>
        public Task<T> CreateOne(T value);

        /// <summary>
        ///     Creates a number of items within the repository
        /// </summary>
        /// <param name="values">A list of values to create</param>
        /// <returns>The stored value, with updated fields including the <see cref="ObjectId" /> and creation date</returns>
        /// <exception cref="RepositoryException"></exception>
        public Task<T[]> CreateMany(T[] values);

        /// <summary>
        ///     Creates a number of items within the repository, based on a count of items to create and a generator function which can
        ///     produce new instances of type <typeparamref name="T" />
        /// </summary>
        /// <param name="count">The number of items to create</param>
        /// <param name="generator">A generator lambda</param>
        /// <returns>An array of newly created items</returns>
        /// <exception cref="RepositoryException"></exception>
        public Task<T[]> CreateMany(long count, Func<int, T> generator);

        /// <summary>
        ///     Deletes a single object from the repository
        /// </summary>
        /// <param name="value">The object to delete.  Should have a valid <see cref="ObjectId" /></param>
        /// <returns>The number of items deleted</returns>
        /// <exception cref="RepositoryException"></exception>
        public Task<long> DeleteOne(T value);

        /// <summary>
        ///     Deletes a single object from the repository, based on <see cref="ObjectId" />
        /// </summary>
        /// <param name="Id">The <see cref="ObjectId" /> of the item to delete</param>
        /// <param name="behaviour">The deletion behaviour to use</param>
        /// <returns>The number of items deleted</returns>
        /// <exception cref="RepositoryException"></exception>
        public Task<long> DeleteOne(ObjectId Id);

        /// <summary>
        ///     Deletes multiple values from the repository
        /// </summary>
        /// <param name="values">An array containing the items to be deleted</param>
        /// <returns>The number of items deleted</returns>
        /// <exception cref="RepositoryException"></exception>
        public Task<long> DeleteMany(T[] values);

        /// <summary>
        ///     Deletes multiple values from the repository, based on a filter lambda
        /// </summary>
        /// <param name="filter">A lambda producing an instance of <see cref="FilterDefinition{TDocument}" /></param>
        /// <returns>The nubmer of items deleted</returns>
        /// <exception cref="RepositoryException"></exception>
        public Task<long> DeleteMany(Func<FilterDefinition<T>> filter);

        /// <summary>
        ///     Purges all items that have been marked for "soft deletion" within the repository
        /// </summary>
        /// <returns>The number of items purged</returns>
        /// <exception cref="RepositoryException"></exception>
        public Task<long> Purge();

        /// <summary>
        ///     Purges all items that have been marked for "soft deletion" and also match the supplied filter
        /// </summary>
        /// <param name="filter">Lambda producing a <see cref="FilterDefinition{TDocument}" /></param>
        /// <returns>The number of items purged</returns>
        /// <exception cref="RepositoryException"></exception>
        public Task<long> Purge(Func<FilterDefinition<T>> filter);

        /// <summary>
        ///     Updates a single value within the repository
        /// </summary>
        /// <param name="value">The value of type <typeparamref name="T" /> to update</param>
        /// <returns>The updated value</returns>
        /// <exception cref="RepositoryException"></exception>
        public Task<T> UpdateOne(T value);

        /// <summary>
        ///     Updates multiple items within the repository
        /// </summary>
        /// <param name="values">The values to update</param>
        /// <returns>The updated items.  Last modified times may be updated</returns>
        /// <exception cref="RepositoryException"></exception>
        public Task<T[]> UpdateMany(T[] values);
    }
}