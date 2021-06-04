/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

#endregion

namespace JCS.Neon.Glow.Data.Repository.Mongo
{
    /// <summary>
    ///     Abstract base class for objects stored via an implementor of the  <see cref="IRepository{T}" /> interface
    /// </summary>
    public abstract class RepositoryObject
    {
        /// <summary>
        ///     The unique, system assigned identifier for the object
        /// </summary>
        [BsonId]
        public ObjectId Id { get; set; }

        /// <summary>
        ///     The <see cref="IVersionToken{T}" /> used by repository objects
        /// </summary>
        public IVersionToken<long> VersionToken { get; set; } = new RandomLongVersionToken();

        /// <summary>
        ///     The creation timestamp
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc, DateOnly = false)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        ///     The last modified timestamp
        /// </summary>
        public DateTime LastModified { get; set; } = DateTime.UtcNow;

        /// <summary>
        ///     The deletion timestamp for the object
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        ///     Used for soft deletions
        /// </summary>
        public bool Deleted { get; set; } = false;
    }
}