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
        /// Read one item from the repository, based on id
        /// </summary>
        /// <param name="id">The <see cref="ObjectId"/> of the object</param>
        /// <returns>An <see cref="Option{T}"/></returns>
        public Task<Option<T>> ReadOne(ObjectId id);

        /// <summary>
        /// Reads one item from the repository, based on a filter
        /// </summary>
        /// <param name="f">Lambda which returns a filter definition</param>
        /// <returns>An <see cref="Option"/> containing (or not) the first object matching the filter</returns>
        public Task<Option<T>> ReadOne(Func<FilterDefinition<T>> f);

        /// <summary>
        /// Retrieves a single item from the repository and then applies a mapping function to the result.  This is basically
        /// a projection operation
        /// </summary>
        /// <param name="id">The <see cref="ObjectId"/> of the object to retrieve</param>
        /// <param name="f">A function that will map between type <typeparamref name="T"/> and <typeparamref name="V"/></param>
        /// <typeparam name="V">The mapped type</typeparam>
        /// <returns>An <see cref="Option{T}"/></returns>
        public Task<Option<V>> MapOne<V>(ObjectId id, Func<T, Option<V>> f) where V : notnull;

        /// <summary>
        /// Creates a single new item within the repository
        /// </summary>
        /// <param name="value">The object to store</param>
        /// <returns>The stored value, with updated fields including the <see cref="ObjectId"/> and creation date</returns>
        public Task<T> CreateOne(T value);

        /// <summary>
        /// Deletes a single object from the repository
        /// </summary>
        /// <param name="value">The object to delete.  Should have a valid <see cref="ObjectId"/></param>
        /// <returns>Nothing</returns>
        public Task DeleteOne(T value);

        /// <summary>
        /// Deletes a single object from the repository, based on <see cref="ObjectId"/>
        /// </summary>
        /// <param name="Id">The <see cref="ObjectId"/> of the item to delete</param>
        /// <param name="behaviour">The deletion behaviour to use</param>
        /// <returns>Nothing</returns>
        public Task DeleteOne(ObjectId Id);

        public Task<T> UpdateOne(T value);
    }
}