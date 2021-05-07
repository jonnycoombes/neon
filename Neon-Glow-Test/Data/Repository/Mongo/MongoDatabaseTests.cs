/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System;
using JCS.Neon.Glow.Data.Repository.Mongo;
using JCS.Neon.Glow.Test.Data.Repository.EFCore;
using MongoDB.Driver.Core.Configuration;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

#endregion

namespace JCS.Neon.Glow.Test.Data.Repository.Mongo
{
    /// <summary>
    ///     Series of connection-related unit tests.  Check various different configuration options, depending on the test
    ///     environment
    /// </summary>
    public class MongoDatabaseTests : MongoTestBase
    {
        
        /// <summary>
        /// Check whether we can create a context using all the defaults
        /// </summary>
        [Fact(DisplayName = "Required database is dynamically generated with options by the context")]
        [Trait("Category", "Data:Mongo")]
        public void CheckDatabaseCreation()
        {
            var context = new MongoTestContext(ConfigureContextOptions);
            var database = context.Database;
            Assert.True(false);
        }

        [Fact(DisplayName = "Required database is dynamically generated with the correct settings overridden by a context subclass")]
        [Trait("Category", "Data:Mongo")]
        public void CheckDatabaseSettings()
        {
            Assert.True(false);
        }

        public MongoDatabaseTests(ITestOutputHelper output) : base(output)
        {
        }
    }
}