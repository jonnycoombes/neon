/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System.Text;
using JCS.Neon.Glow.Types;
using MongoDB.Driver;

#endregion

namespace JCS.Neon.Glow.Data.Repository.Mongo
{
    /// <summary>
    ///     Build for <see cref="MongoDatabaseSettings" />
    /// </summary>
    public class MongoDatabaseSettingsBuilder : IBuilder<MongoDatabaseSettings>
    {
        /// <summary>
        ///     The internal instance of <see cref="MongoDatabaseSettings" />
        /// </summary>
        private readonly MongoDatabaseSettings _settings = new();

        /// <summary>
        ///     Builds the actual <see cref="MongoDatbaseSettings" /> instance
        /// </summary>
        /// <returns>A new <see cref="MongoDatabaseSettings" /> instance</returns>
        public MongoDatabaseSettings Build()
        {
            return _settings;
        }

        /// <summary>
        ///     Sets the default <see cref="ReadConcern" /> for a database
        /// </summary>
        /// <param name="concern">A valid <see cref="ReadConcern" /></param>
        /// <returns>The current builder instance</returns>
        public MongoDatabaseSettingsBuilder ReadConcern(ReadConcern concern)
        {
            _settings.ReadConcern = concern;
            return this;
        }

        /// <summary>
        ///     Sets the <see cref="UTF8Encoding" /> to use for read operations
        /// </summary>
        /// <param name="encoding">An instance of <see cref="UTF8Encoding" /></param>
        /// <returns>The current builder instance</returns>
        public MongoDatabaseSettingsBuilder ReadEncoding(UTF8Encoding encoding)
        {
            _settings.ReadEncoding = encoding;
            return this;
        }

        /// <summary>
        ///     Sets the default <see cref="ReadPreference" /> for the database
        /// </summary>
        /// <param name="preference">A valid <see cref="ReadPreference" /></param>
        /// <returns>The current builder instance</returns>
        public MongoDatabaseSettingsBuilder ReadPreference(ReadPreference preference)
        {
            _settings.ReadPreference = preference;
            return this;
        }

        /// <summary>
        ///     Sets the <see cref="WriteConcern" /> for the database
        /// </summary>
        /// <param name="concern">A valid <see cref="WriteConcern" /></param>
        /// <returns>The current builder instance</returns>
        public MongoDatabaseSettingsBuilder WriteConcern(WriteConcern concern)
        {
            _settings.WriteConcern = concern;
            return this;
        }

        /// <summary>
        ///     Sets the <see cref="UTF8Encoding" /> to use for write operations
        /// </summary>
        /// <param name="encoding">A valid <see cref="UTF8Encoding" /> instance</param>
        /// <returns>The current builder instance</returns>
        public MongoDatabaseSettingsBuilder WriteEncoding(UTF8Encoding encoding)
        {
            _settings.WriteEncoding = encoding;
            return this;
        }
    }
}