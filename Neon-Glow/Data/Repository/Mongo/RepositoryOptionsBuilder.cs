/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
using JCS.Neon.Glow.Types;
using MongoDB.Driver;

namespace JCS.Neon.Glow.Data.Repository.Mongo
{
    /// <summary>
    /// Builder for <see cref="RepositoryOptions"/>
    /// </summary>
    public class RepositoryOptionsBuilder : IBuilder<RepositoryOptions>
    {

        /// <summary>
        /// The internal options
        /// </summary>
        private RepositoryOptions _options = new();
        
        /// <inheritdoc cref="IBuilder{T}.Build"/>
        public RepositoryOptions Build()
        {
            return _options;
        }

        /// <summary>
        /// Sets the read concern
        /// </summary>
        /// <param name="concern">A valid <see cref="ReadConcern"/> value</param>
        /// <returns>The current builder instance</returns>
        public RepositoryOptionsBuilder ReadConcern(ReadConcern concern)
        {
            _options.ReadConcern = concern;
            return this;
        }
        
        /// <summary>
        /// Sets the write concern 
        /// </summary>
        /// <param name="concern">A valid <see cref="WriteConcern"/> value</param>
        /// <returns>The current builder instance</returns>
        public RepositoryOptionsBuilder WriteConcern(WriteConcern concern)
        {
            _options.WriteConcern = concern;
            return this;
        }

        /// <summary>
        /// Sets the read behaviour for the repository
        /// </summary>
        /// <param name="option">An value from <see cref="RepositoryOptions.ReadBehaviourOptions"/></param>
        /// <returns></returns>
        public RepositoryOptionsBuilder ReadBehaviour(RepositoryOptions.ReadBehaviourOption option)
        {
            _options.ReadBehaviour = option;
            return this;
        }

        /// <summary>
        /// Sets the deletion behaviour for the repository
        /// </summary>
        /// <param name="option">A value from the <see cref="RepositoryOptions.DeletionBehaviourOptions"/></param>
        /// <returns></returns>
        public RepositoryOptionsBuilder DeletionBehaviour(RepositoryOptions.DeletionBehaviourOption option)
        {
            _options.DeletionBehaviour = option;
            return this;
        }
    }
}