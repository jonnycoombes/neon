/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System;
using JCS.Neon.Glow.Data.Repository.Mongo;

#endregion

namespace JCS.Neon.Glow.Test.Data.Repository.Mongo
{
    /// <summary>
    ///     Context used during tests
    /// </summary>
    public class MongoTestContext : MongoDbContext
    {
        /// <summary>
        ///     Constructor taking a pre-cooked set of options
        /// </summary>
        /// <param name="options">Valid <see cref="MongoDbContextOptions" /> instance</param>
        public MongoTestContext(MongoDbContextOptions options) : base(options)
        {
        }

        /// <summary>
        ///     Constructor taking an action to configure the options
        /// </summary>
        /// <param name="configureAction">An <see cref="Action" /> that will be called in order to build the options</param>
        public MongoTestContext(Action<MongoDbContextOptionsBuilder> configureAction) : base(configureAction)
        {
        }
    }
}