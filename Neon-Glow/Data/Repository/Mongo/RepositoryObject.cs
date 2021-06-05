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
    public abstract class RepositoryObject : IEquatable<RepositoryObject>
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

        /// <summary>
        ///     Repository object equality.  Checks both object id and also the current version token
        /// </summary>
        /// <param name="other">The object to compare to</param>
        /// <returns>true or false based on equality</returns>
        public bool Equals(RepositoryObject? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Id.Equals(other.Id) && VersionToken.Value.Equals(other.VersionToken.Value);
        }

        /// <summary>
        ///     Equality operator
        /// </summary>
        /// <param name="obj">The object to compare to</param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((RepositoryObject) obj);
        }

        /// <summary>
        ///     Overridden hash code
        /// </summary>
        /// <returns>int hash for this object</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, VersionToken.Value);
        }
    }
}