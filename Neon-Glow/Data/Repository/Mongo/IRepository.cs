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
        
        public Task<Option<T>> ReadOne(ObjectId id);
        
        public Task<Option<T>> ReadOne(Action<FilterDefinitionBuilder<T>> f);

        public Task<Option<V>> MapOne<V>(ObjectId id, Func<T, Option<V>> fMap) where V : notnull;

        public Task<T> CreateOne(T value);

        public Task DeleteOne(T value);

        public Task DeleteOne(ObjectId Id);

        public Task<T> UpdateOne(T value);
    }
}