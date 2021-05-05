#region

using System;
using JCS.Neon.Glow.Statics;
using MongoDB.Driver;
using Serilog;

#endregion

namespace JCS.Neon.Glow.Data.Repository.Mongo
{
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
        ///     The currently bound instance of <see cref="IMongoDatabase" />
        /// </summary>
        private IMongoDatabase? _database;

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
                .SetHost(hostName)
                .SetDatabase(databaseName)
                .SetAuthenticationType(MongoAuthenticationType.Basic)
                .SetUser(user)
                .SetPassword(password)
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

        /// <inheritdoc cref="IMongoDbContext.Client"/>
        public MongoClient Client => ResolveClient();

        /// <inheritdoc cref="IMongoDbContext.Options"/>
        public MongoDbContextOptions Options { get; }
        
        /// <inheritdoc cref="IMongoDbContext.Database"/>
        public IMongoDatabase Database => ResolveDatabase();

        /// <summary>
        ///     Checks whether we have a client, and if not builds one using the current <see cref="MongoClientSettings" /> object.
        /// </summary>
        /// <returns>An instance of <see cref="MongoClient" /></returns>
        private MongoClient ResolveClient()
        {
            lock (this)
            {
                return _client ??= new MongoClient(Options.BuildClientSettings());
            }
        }

        /// <summary>
        ///     Checks whether we currently have a database reference, and if not performs the necessary setup and binding
        ///     operations in order
        ///     to get a valid reference
        /// </summary>
        /// <returns>A bound instance of <see cref="IMongoDatabase" /></returns>
        private IMongoDatabase ResolveDatabase()
        {
            return null;
        }
    }
}