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
    ///     Options class for instances of <see cref="IRepository{T}" />
    /// </summary>
    public class RepositoryOptions
    {
        /// <summary>
        ///     Determines the behaviour of deletions
        /// </summary>
        public enum DeletionBehaviourOption
        {
            /// <summary>
            ///     Soft deletion.  Default behaviour is to update the <see cref="RepositoryObject.Deleted" /> field
            /// </summary>
            Soft,

            /// <summary>
            ///     Hard deletion.  The entire object is removed.
            /// </summary>
            Hard
        }

        /// <summary>
        ///     Determines the behaviour of read operations
        /// </summary>
        public enum ReadBehaviourOption
        {
            /// <summary>
            ///     Read operations ignore deleted items
            /// </summary>
            IgnoreDeleted,

            /// <summary>
            ///     Read operations include deleted items
            /// </summary>
            IncludeDeleted
        }

        /// <summary>
        ///     The <see cref="ReadConcern" /> which should be honoured by the repository
        /// </summary>
        public ReadConcern ReadConcern { get; set; } = ReadConcern.Default;

        /// <summary>
        ///     The <see cref="WriteConcern" /> which should be honoured by the repository
        /// </summary>
        public WriteConcern WriteConcern { get; set; } = WriteConcern.Unacknowledged;

        /// <summary>
        ///     Determines how the repository handles deletions
        /// </summary>
        public DeletionBehaviourOption DeletionBehaviour { get; set; } = DeletionBehaviourOption.Soft;

        /// <summary>
        ///     Determines how the repository handles reads
        /// </summary>
        public ReadBehaviourOption ReadBehaviour { get; set; } = ReadBehaviourOption.IgnoreDeleted;
    }
}