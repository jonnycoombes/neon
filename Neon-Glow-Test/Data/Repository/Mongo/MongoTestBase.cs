/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System;
using JCS.Neon.Glow.Data.Repository.Mongo;
using MongoDB.Driver;

#endregion

namespace JCS.Neon.Glow.Test.Data.Repository.Mongo
{
    /// <summary>
    ///     Abstract base class for Mongo-related test suites and cases
    /// </summary>
    public abstract class MongoTestBase : TestBase, IDisposable
    {

        /// <summary>
        /// Used to delegate the build of a <see cref="MongoDbContextOptions"/> instance
        /// </summary>
        /// <param name="configAction">An action which configures the current options</param>
        /// <returns>A freshly minted instance of <see cref="MongoDbContextOptions"/> instance</returns>
        protected MongoDbContextOptions BuildOptions(Action<MongoDbContextOptionsBuilder> configAction)
        {
            var builder = new MongoDbContextOptionsBuilder();
            configAction(builder);
            return builder.Build();
        }
        
        /// <summary>
        ///     Nothing here to currently dispose...
        /// </summary>
        public void Dispose()
        {
        }
    }
}