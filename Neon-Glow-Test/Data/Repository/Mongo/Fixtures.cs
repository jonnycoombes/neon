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
using MongoDB.Driver.Core.Configuration;
using Xunit.Abstractions;

#endregion

namespace JCS.Neon.Glow.Test.Data.Repository.Mongo
{

    /// <summary>
    ///     Class containing fixtures for general Mongo tests
    /// </summary>
    public class Fixtures
    {

        /// <summary>
        /// The test <see cref="IDbContext"/>
        /// </summary>
        public IDbContext DbContext { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Fixtures()
        {
            DbContext = new TestDbContext(ConfigureContextOptions("test"));
        }
        
        /// <summary>
        /// This builds the default context options for all local tests
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        protected Action<DbContextOptionsBuilder> ConfigureContextOptions(string databaseName)
        {
            return builder => builder.Host(DbContextOptions.DefaultServerHost)
                .Port(DbContextOptions.DefaultServerPort)
                .Scheme(ConnectionStringScheme.MongoDB)
                .Database(databaseName)
                .AuthenticationDatabase("admin")
                .Application("neon-glow")
                .User("root")
                .Password("root");
        }
    }
}