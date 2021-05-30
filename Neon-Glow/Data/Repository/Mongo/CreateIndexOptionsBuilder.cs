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
    ///     A builder for new instances of <see cref="CreateIndexOptions" />
    /// </summary>
    public class CreateIndexOptionsBuilder : IBuilder<CreateIndexOptions>
    {
        /// <summary>
        ///     Private instance of <see cref="CreateIndexOptions" />
        /// </summary>
        private readonly CreateIndexOptions _options = new();

        /// <summary>
        ///     Returns an initialised instance of <see cref="CreateIndexOptions" />
        /// </summary>
        /// <returns>An instance of <see cref="CreateIndexOptions" /></returns>
        public CreateIndexOptions Build()
        {
            return _options;
        }

        /// <summary>
        ///     Sets the name of the index
        /// </summary>
        /// <param name="name"></param>
        /// <returns>The current instance of <see cref="CreateIndexOptionsBuilder" /></returns>
        public CreateIndexOptionsBuilder Name(string name)
        {
            _options.Name = name;
            return this;
        }

        /// <summary>
        ///     Sets whether or not the index is created in the backgroundf
        /// </summary>
        /// <param name="background">true or false</param>
        /// <returns>The current instance of <see cref="CreateIndexOptionsBuilder" /></returns>
        public CreateIndexOptionsBuilder Background(bool background)
        {
            _options.Background = background;
            return this;
        }

        /// <summary>
        ///     Whether or not the index is hidden
        /// </summary>
        /// <param name="hidden">true or false</param>
        /// <returns>The current instance of <see cref="CreateIndexOptionsBuilder" /></returns>
        public CreateIndexOptionsBuilder Hidden(bool hidden)
        {
            _options.Hidden = hidden;
            return this;
        }

        /// <summary>
        ///     Whether or not the index is sparse
        /// </summary>
        /// <param name="sparse">true or false</param>
        /// <returns>The current instance of <see cref="CreateIndexOptionsBuilder" /></returns>
        public CreateIndexOptionsBuilder Sparse(bool sparse)
        {
            _options.Sparse = sparse;
            return this;
        }

        /// <summary>
        ///     Whether or not the index is unique
        /// </summary>
        /// <param name="unique">true or false</param>
        /// <returns>The current instance of <see cref="CreateIndexOptionsBuilder" /></returns>
        public CreateIndexOptionsBuilder Unique(bool unique)
        {
            _options.Unique = unique;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="Collation"/> for an index
        /// </summary>
        /// <param name="collation">A valid <see cref="Collation"/> instance</param>
        /// <returns>The current instance of <see cref="CreateIndexOptionsBuilder"/></returns>
        public CreateIndexOptionsBuilder Collation(Collation collation)
        {
            _options.Collation = collation;
            return this;
        }
    }
}