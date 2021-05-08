/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using JCS.Neon.Glow.Types;
using MongoDB.Driver;

#endregion

namespace JCS.Neon.Glow.Data.Repository.Mongo
{
    /// <summary>
    ///     A builder for creating new instances of <see cref="CreateCollectionOptions" />
    /// </summary>
    public class MongoCreateCollectionOptionsBuilder : IBuilder<CreateCollectionOptions>
    {
        /// <summary>
        ///     The internal instance of <see cref="CreateCollectionOptions" />
        /// </summary>
        private readonly CreateCollectionOptions _options = new();


        /// <summary>
        ///     Builds and returns a new instance of <see cref="CreateCollectionOptions" />
        /// </summary>
        /// <returns>A new instance of <see cref="CreateCollectionOptions" /></returns>
        public CreateCollectionOptions Build()
        {
            return _options;
        }

        /// <summary>
        ///     Whether or not the collection should be capped
        /// </summary>
        /// <param name="value">True if the collection is to be capped</param>
        /// <returns>The current instance of the builder</returns>
        public MongoCreateCollectionOptionsBuilder Capped(bool value)
        {
            _options.Capped = value;
            return this;
        }
    }
}