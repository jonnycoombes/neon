/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
using MongoDB.Driver;

namespace JCS.Neon.Glow.Data.Repository.Mongo
{
    /// <summary>
    /// Options class for instances of <see cref="IRepository{T}"/>
    /// </summary>
    public class RepositoryOptions
    {
        /// <summary>
        /// The <see cref="ReadConcern"/> which should be honoured by the repository
        /// </summary>
        public ReadConcern ReadConcern { get; set; } = ReadConcern.Default;

        /// <summary>
        /// The <see cref="WriteConcern"/> which should be honoured by the repository
        /// </summary>
        public WriteConcern WriteConcern { get; set; } = WriteConcern.Unacknowledged;
    }
    
    
}