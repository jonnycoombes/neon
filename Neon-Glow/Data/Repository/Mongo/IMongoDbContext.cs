/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using MongoDB.Driver;

#endregion

namespace JCS.Neon.Glow.Data.Repository.Mongo
{
    /// <summary>
    ///     Interface definition for a database context backed by Mongo DB
    /// </summary>
    public interface IMongoDbContext
    {
        /// <summary>
        ///     The <see cref="MongoClient" /> currently being used by the context
        /// </summary>
        public MongoClient Client { get; }

        /// <summary>
        ///     The currently configured set of <see cref="MongoDbContextOptions" /> being used by the context
        /// </summary>
        public MongoDbContextOptions Options { get; }

        /// <summary>
        ///     The bound <see cref="IMongoDatabase" /> being used by the context
        /// </summary>
        public IMongoDatabase Database { get; }
    }
}