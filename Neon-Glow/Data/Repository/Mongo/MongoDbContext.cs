/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System;
using System.Linq;
using JCS.Neon.Glow.Statics;
using MongoDB.Driver;
using Serilog;

#endregion

namespace JCS.Neon.Glow.Data.Repository.Mongo
{
    /// <summary>
    ///     Enumeration that denotes the kind of binding being performed against a database
    /// </summary>
    public enum DatabaseBindingType
    {
        /// <summary>
        ///     The bind operation relates to a new database being created
        /// </summary>
        Create,

        /// <summary>
        ///     The bind operation relates to an existing database
        /// </summary>
        Existing
    }

    /// <summary>
    ///     Abstract base class for Mongo DB contexts.  Takes care of all the clever stuff relating to lifecycle, session
    ///     management etc...Derived classes can add automatic support for repository-style access to collections and related
    ///     functionality through generic virtual methods and properties.
    /// </summary>
    public abstract class MongoDbContext : IMongoDbContext
    {
        /// <summary>
        ///     Static logger
        /// </summary>
        private static readonly ILogger _log = Log.ForContext<MongoDbContext>();

        /// <summary>
        ///     The underlying <see cref="MongoClient" />
        /// </summary>
        private MongoClient? _client;

        /// <summary>
        ///     Simple constructor that allows a host and database to be specified, along with a username and password
        /// </summary>
        /// <param name="hostName">The mongo host name</param>
        /// <param name="databaseName">The database name</param>
        /// <param name="user">The user to be used for authentication</param>
        /// <param name="password">The password to be used for authentication</param>
        protected MongoDbContext(string hostName, string databaseName, string user, string password)
        {
            Logging.MethodCall(_log);
            Options = new MongoDbContextOptionsBuilder()
                .Host(hostName)
                .Database(databaseName)
                .AuthenticationType(MongoAuthenticationType.Basic)
                .User(user)
                .Password(password)
                .Build();
        }

        /// <summary>
        ///     Constructor that takes an instance of <see cref="MongoDbContextOptions" />
        /// </summary>
        /// <param name="options">An instance of <see cref="MongoDbContextOptions" /></param>
        protected MongoDbContext(MongoDbContextOptions options)
        {
            Logging.MethodCall(_log);
            Options = options;
        }

        /// <summary>
        ///     Constructor that takes an <see cref="Action" /> that can take a <see cref="MongoDbContextOptionsBuilder" />
        ///     instance and configure the context options through the builder
        /// </summary>
        /// <param name="configureAction">
        ///     An <see cref="Action" /> which modifies an instance of
        ///     <see cref="MongoDbContextOptionsBuilder" /> in order to arrive at a good configuration
        /// </param>
        protected MongoDbContext(Action<MongoDbContextOptionsBuilder> configureAction)
        {
            Logging.MethodCall(_log);
            var builder = new MongoDbContextOptionsBuilder();
            configureAction(builder);
            Options = builder.Build();
        }

        /// <inheritdoc cref="IMongoDbContext.Client" />
        public MongoClient Client => BindClient();

        /// <inheritdoc cref="IMongoDbContext.Options" />
        public MongoDbContextOptions Options { get; }

        /// <inheritdoc cref="IMongoDbContext.Database" />
        public IMongoDatabase Database => BindDatabase();

        /// <summary>
        ///     Checks whether we have a client, and if not builds one using the current <see cref="MongoClientSettings" /> object.
        /// </summary>
        /// <returns>An instance of <see cref="MongoClient" /></returns>
        private MongoClient BindClient()
        {
            lock (this)
            {
                return _client ??= new MongoClient(Options.BuildClientSettings());
            }
        }

        /// <summary>
        ///     Function which checks whether a given database currently exists
        /// </summary>
        /// <param name="databaseName">The name of the database to check for</param>
        /// <returns><code>true</code> if the database exists, <code>false</code> otherwise</returns>
        private bool DatabaseExists(string databaseName)
        {
            return Client
                .ListDatabaseNames()
                .ToList()
                .Any(s => s == databaseName);
        }

        /// <summary>
        ///     Function which checks whether a given collection currently exists
        /// </summary>
        /// <param name="collectionName">The name of the collection</param>
        /// <returns><code>true</code> if the database exists, <code>false</code> otherwise</returns>
        private bool CollectionExists(string collectionName)
        {
            return Database.ListCollectionNames()
                .ToList()
                .Any(s => s == collectionName);
        }

        /// <summary>
        /// </summary>
        /// <param name="builder"></param>
        protected virtual void OnDatabaseBinding(DatabaseBindingType bindingType, MongoDatabaseSettingsBuilder builder)
        {
            Logging.MethodCall(_log);
            switch (bindingType)
            {
                case DatabaseBindingType.Create:
                    Logging.Verbose(_log, "Binding to non-existent database");
                    break;
                case DatabaseBindingType.Existing:
                    Logging.Verbose(_log, "Binding to existing database");
                    break;
            }

            builder
                .ReadConcern(Options.DatabaseReadConcern)
                .WriteConcern(Options.DatabaseWriteConcern);
        }

        /// <summary>
        ///     Checks whether we currently have a database reference, and if not performs the necessary setup and bind operations
        ///     in order to get a valid reference
        /// </summary>
        /// <returns>A bound instance of <see cref="IMongoDatabase" /></returns>
        private IMongoDatabase BindDatabase()
        {
            Logging.MethodCall(_log);
            Assertions.Checked<MongoDbContextException>(Options.Database != null,
                "No database name has been specified");

            var builder = new MongoDatabaseSettingsBuilder();
            OnDatabaseBinding(!DatabaseExists(Options.Database!) ? DatabaseBindingType.Create : DatabaseBindingType.Existing, builder);
            return Client.GetDatabase(Options.Database, builder.Build());
        }
    }
}