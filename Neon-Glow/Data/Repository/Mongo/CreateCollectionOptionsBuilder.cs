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
    public class CreateCollectionOptionsBuilder : IBuilder<CreateCollectionOptions>
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
        public CreateCollectionOptionsBuilder Capped(bool value)
        {
            _options.Capped = value;
            return this;
        }

        /// <summary>
        ///     Sets the maximum size for a collection
        /// </summary>
        /// <param name="size">The maximum size of a collection in bytes</param>
        /// <returns>The current builder instance</returns>
        public CreateCollectionOptionsBuilder MaxSize(long? size)
        {
            _options.MaxSize = size;
            return this;
        }

        /// <summary>
        ///     Sets the maximum number of documents in the collection.  Only applies for capped collections
        /// </summary>
        /// <param name="count">The maximum number of documents</param>
        /// <returns>The current builder instance</returns>
        public CreateCollectionOptionsBuilder MaxDocuments(long? count)
        {
            _options.MaxDocuments = count;
            return this;
        }

        /// <summary>
        ///     Whether the collection should not use padding
        /// </summary>
        /// <param name="noPadding">True or false</param>
        /// <returns>The current builder instance</returns>
        public CreateCollectionOptionsBuilder NoPadding(bool noPadding)
        {
            _options.NoPadding = noPadding;
            return this;
        }

        /// <summary>
        ///     Whether the collection should use power of 2 sizes
        /// </summary>
        /// <param name="usePowerOf2Sizes">True or false</param>
        /// <returns>The current builder instance</returns>
        public CreateCollectionOptionsBuilder UsePowerOf2Sizes(bool usePowerOf2Sizes)
        {
            _options.UsePowerOf2Sizes = usePowerOf2Sizes;
            return this;
        }

        /// <summary>
        ///     Sets the <see cref="DocumentValidationAction" /> for the collection
        /// </summary>
        /// <param name="action">A value from the <see cref="DocumentValidationAction" /> enumeration</param>
        /// <returns>The current builder instance</returns>
        public CreateCollectionOptionsBuilder ValidationAction(DocumentValidationAction action)
        {
            _options.ValidationAction = action;
            return this;
        }

        /// <summary>
        ///     Sets the validation level for the collection
        /// </summary>
        /// <param name="level">A value from the <see cref="DocumentValidationLevel" /> enumeration</param>
        /// <returns>The current builder instance</returns>
        public CreateCollectionOptionsBuilder ValidationLevel(DocumentValidationLevel level)
        {
            _options.ValidationLevel = level;
            return this;
        }
    }
}