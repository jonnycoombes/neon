/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System.Text;
using JCS.Neon.Glow.Types;
using MongoDB.Bson;
using MongoDB.Driver;

#endregion

namespace JCS.Neon.Glow.Data.Repository.Mongo
{
    /// <summary>
    ///     Builder class for new instancefs of <see cref="MongoCollectionSettings" />
    /// </summary>
    public class CollectionSettingsBuilder : IBuilder<MongoCollectionSettings>
    {
        /// <summary>
        ///     The acutal settings
        /// </summary>
        private readonly MongoCollectionSettings _settings = new();

        /// <summary>
        ///     Returns the built <see cref="MongoCollectionSettings" />
        /// </summary>
        /// <returns>A built instance of <see cref="MongoCollectionSettings" /></returns>
        public MongoCollectionSettings Build()
        {
            return _settings;
        }

        /// <summary>
        ///     Sets the default <see cref="ReadConcern" /> for a collection
        /// </summary>
        /// <param name="concern">A valid <see cref="ReadConcern" /></param>
        /// <returns>The current builder instance</returns>
        public CollectionSettingsBuilder ReadConcern(ReadConcern concern)
        {
            _settings.ReadConcern = concern;
            return this;
        }

        /// <summary>
        ///     Sets the <see cref="UTF8Encoding" /> to use for read operations
        /// </summary>
        /// <param name="encoding">An instance of <see cref="UTF8Encoding" /></param>
        /// <returns>The current builder instance</returns>
        public CollectionSettingsBuilder ReadEncoding(UTF8Encoding encoding)
        {
            _settings.ReadEncoding = encoding;
            return this;
        }

        /// <summary>
        ///     Sets the default <see cref="ReadPreference" /> for the collection
        /// </summary>
        /// <param name="preference">A valid <see cref="ReadPreference" /></param>
        /// <returns>The current builder instance</returns>
        public CollectionSettingsBuilder ReadPreference(ReadPreference preference)
        {
            _settings.ReadPreference = preference;
            return this;
        }

        /// <summary>
        ///     Sets the <see cref="WriteConcern" /> for the collection
        /// </summary>
        /// <param name="concern">A valid <see cref="WriteConcern" /></param>
        /// <returns>The current builder instance</returns>
        public CollectionSettingsBuilder WriteConcern(WriteConcern concern)
        {
            _settings.WriteConcern = concern;
            return this;
        }

        /// <summary>
        ///     Sets the <see cref="UTF8Encoding" /> to use for write operations
        /// </summary>
        /// <param name="encoding">A valid <see cref="UTF8Encoding" /> instance</param>
        /// <returns>The current builder instance</returns>
        public CollectionSettingsBuilder WriteEncoding(UTF8Encoding encoding)
        {
            _settings.WriteEncoding = encoding;
            return this;
        }

        /// <summary>
        /// Sets whether an id should be assigned on insertion
        /// </summary>
        /// <param name="assignId"></param>
        /// <returns></returns>
        public CollectionSettingsBuilder SetIdOnInsert(bool assignId)
        {
            _settings.AssignIdOnInsert = assignId;
            return this;
        }

    }
}