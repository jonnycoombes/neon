#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JCS.Neon.Glow.Statics;
using JCS.Neon.Glow.Statics.Reflection;
using MongoDB.Driver;
using Serilog;

#endregion

namespace JCS.Neon.Glow.Data.Repository.Mongo
{
    /// <summary>
    ///     Enumeration that denotes the kind of binding being performed against a <see cref="IMongoDatabase" />
    /// </summary>
    public enum BindingType
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
    public abstract class DbContext : IDbContext
    {
        /// <summary>
        ///     Static logger
        /// </summary>
        private static readonly ILogger _log = Log.ForContext<DbContext>();

        /// <summary>
        ///     A cache of currently bound collections
        /// </summary>
        private readonly Dictionary<Type, object> _boundCollections = new();

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
        protected DbContext(string hostName, string databaseName, string user, string password)
        {
            Logging.MethodCall(_log);
            Options = new DbContextOptionsBuilder()
                .Host(hostName)
                .Database(databaseName)
                .AuthenticationType(AuthenticationType.Basic)
                .User(user)
                .Password(password)
                .Build();
        }

        /// <summary>
        ///     Constructor that takes an instance of <see cref="DbContextOptions" />
        /// </summary>
        /// <param name="options">An instance of <see cref="DbContextOptions" /></param>
        protected DbContext(DbContextOptions options)
        {
            Logging.MethodCall(_log);
            Options = options;
        }

        /// <summary>
        ///     Constructor that takes an <see cref="Action" /> that can take a <see cref="DbContextOptionsBuilder" />
        ///     instance and configure the context options through the builder
        /// </summary>
        /// <param name="configureAction">
        ///     An <see cref="Action" /> which modifies an instance of
        ///     <see cref="DbContextOptionsBuilder" /> in order to arrive at a good configuration
        /// </param>
        protected DbContext(Action<DbContextOptionsBuilder> configureAction)
        {
            Logging.MethodCall(_log);
            var builder = new DbContextOptionsBuilder();
            configureAction(builder);
            Options = builder.Build();
        }

        /// <inheritdoc cref="IDbContext.Client" />
        public MongoClient Client => BindClient();

        /// <inheritdoc cref="IDbContext.Options" />
        public DbContextOptions Options { get; }

        /// <inheritdoc cref="IDbContext.Database" />
        public IMongoDatabase Database => BindDatabase();

        /// <inheritdoc cref="IDbContext.Collection{T}" />
        public IMongoCollection<T> Collection<T>(MongoCollectionSettings? settings, CancellationToken token)
        {
            return BindCollection<T>(settings);
        }

        /// <inheritdoc cref="IDbContext.Queryable{T}" />
        public IQueryable<T> Queryable<T>(MongoCollectionSettings? settings, CancellationToken token)
        {
            return BindCollection<T>(settings).AsQueryable();
        }

        /// <inheritdoc cref="IDbContext.DatabaseExists" />
        public bool DatabaseExists(string databaseName)
        {
            try
            {
                return Client
                    .ListDatabaseNames()
                    .ToList()
                    .Any(s => s == databaseName);
            }
            catch (Exception ex)
            {
                Logging.Error(_log, "Exception caught whilst attempting to interrogate underlying Mongo instance");
                throw Exceptions.LoggedException<DbContextException>(_log,
                    $"Timed out connecting to \"{databaseName}\"", ex);
            }
        }

        /// <inheritdoc cref="IDbContext.CollectionExists" />
        public bool CollectionExists(string collectionName)
        {
            try
            {
                return Database.ListCollectionNames()
                    .ToList()
                    .Any(s => s == collectionName);
            }
            catch (Exception ex)
            {
                Logging.Error(_log, "Exception caught whilst attempting to interrogate underlying Mongo instance");
                throw Exceptions.LoggedException<DbContextException>(_log,
                    $"Timed out whilst looking for collection \"{collectionName}\"", ex);
            }
        }

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
        ///     This member is called during database bind operations, and allows for <see cref="MongoDatabaseSettings" /> to be
        ///     specified or overridden.  The underlying bind call checks whether the database being bound already exists within the
        ///     underlying Mongo instance, or is being created dynamically.  If the database already exists, then a
        ///     <see cref="BindingType" /> with value <see cref="BindingType.Existing" /> is passed through as part of
        ///     the call.  Otherwise, <see cref="BindingType.Create" /> is passed.
        ///     <para>
        ///         Subclasses may override this function in order to tailor their settings.  By default, read and write concern
        ///         settings are
        ///         taken from the currently configured <see cref="DbContextOptions" />
        ///     </para>
        /// </summary>
        /// <param name="builder">
        ///     An instance of <see cref="DatabaseSettingsBuilder" /> which is used to construct the
        ///     database settings.
        /// </param>
        protected virtual void OnDatabaseBinding(BindingType bindingType, DatabaseSettingsBuilder builder)
        {
            Logging.MethodCall(_log);
            Logging.Verbose(_log, $"{bindingType} database binding event");
            builder
                .ReadConcern(Options.DatabaseReadConcern)
                .WriteConcern(Options.DatabaseWriteConcern);
        }

        /// <summary>
        ///     This is called during collection binding operations and allows for <see cref="MongoCollectionSettings" /> to be overridden.
        /// </summary>
        /// <param name="bindingType">
        ///     The binding type.  Will either be <see cref="BindingType.Create" /> or <see cref="BindingType.Existing" />
        /// </param>
        /// <param name="builder">An instance of <see cref="CollectionSettingsBuilder" /> which can be used to modify collection settings</param>
        /// <typeparam name="T">The type of the collection being bound</typeparam>
        protected virtual void OnCollectionBinding<T>(BindingType bindingType, CollectionSettingsBuilder builder)
        {
            Logging.MethodCall(_log);
            Logging.Verbose(_log, $"{bindingType} collection binding event");
            builder.ReadConcern(Options.CollectionReadConcern)
                .WriteConcern(Options.CollectionWriteConcern);
        }

        /// <summary>
        ///     Called during the creation of a new <see cref="IMongoCollection{TDocument}" />, allows for the chance to reflect on the
        ///     target type and create any necessary index structures etc...
        /// </summary>
        /// <param name="optionsBuilder">An instance of <see cref="CreateCollectionOptionsBuilder" /> to configure the collection</param>
        /// <param name="indexDefinitions">A list of <see cref="IndexKeysDefinition{TDocument}" />, used to create indexes</param>
        /// <typeparam name="T">The type to be stored within the collection</typeparam>
        protected virtual void OnCollectionCreating<T>(CreateCollectionOptionsBuilder optionsBuilder,
            List<IndexKeysDefinition<T>> indexDefinitions)
        {
            Logging.MethodCall(_log);
            
        }

        /// <summary>
        ///     Checks whether we currently have a database reference, and if not performs the necessary setup and bind operations
        ///     in order to get a valid reference
        /// </summary>
        /// <returns>A bound instance of <see cref="IMongoDatabase" /></returns>
        /// <exception cref="DbContextException">Thrown if the connection to the database fails (i.e. times out)</exception>
        private IMongoDatabase BindDatabase()
        {
            Logging.MethodCall(_log);
            Assertions.Checked<DbContextException>(Options.Database != null,
                "No database name has been specified");

            var builder = new DatabaseSettingsBuilder();
            OnDatabaseBinding(!DatabaseExists(Options.Database!) ? BindingType.Create : BindingType.Existing, builder);
            return Client.GetDatabase(Options.Database, builder.Build());
        }

        /// <summary>
        ///     Checks whether we currently have a binding into a collection for objects of type <see cref="T" />, and if not
        ///     performs any necessary bind operations
        /// </summary>
        /// <param name="settings">
        ///     An optional set of <see cref="MongoCollectionSettings" /> which may be passed in to influence
        ///     the bind
        /// </param>
        /// <param name="token">An optional <see cref="CancellationToken" /></param>
        /// <typeparam name="T">
        ///     The type of objects stored within the underlying <see cref="IMongoCollection{TDocument}" />
        /// </typeparam>
        /// <returns>A bound instance of <see cref="IMongoCollection{TDocument}" /></returns>
        private IMongoCollection<T> BindCollection<T>(MongoCollectionSettings? settings,
            CancellationToken token = default)
        {
            Logging.MethodCall(_log);
            Logging.Verbose(_log, $"Binding collection for type \"{typeof(T)}\"");

            var collectionType = typeof(T);
            if (_boundCollections.ContainsKey(collectionType))
            {
                Logging.Verbose(_log,
                    $"Collection for type \"{collectionType}\" already bound, returning existing instance");
                return (IMongoCollection<T>) _boundCollections[collectionType];
            }

            // derive the name for the collection
            Logging.Verbose(_log, $"No bound collection found for type \"{collectionType}\"");
            var collectionName = DeriveCollectionName<T>(Options.CollectionNamingConvention);
            Logging.Verbose(_log, $"Derived collection name is given as \"{collectionName}\"");

            // check if the collection exists already
            if (CollectionExists(collectionName))
            {
                Logging.Verbose(_log, $"Binding to existing collection \"{collectionName}\"");
                var settingsBuilder = new CollectionSettingsBuilder();
                OnCollectionBinding<T>(BindingType.Existing, settingsBuilder);
                return Database.GetCollection<T>(collectionName, settingsBuilder.Build());
            }

            // create a new collection and bind to that
            Logging.Verbose(_log, $"Creating a new collection for type \"{typeof(T).FullName}\"");
            var optionsBuilder = new CreateCollectionOptionsBuilder();
            var indexDefinitions = new List<IndexKeysDefinition<T>>();
            OnCollectionCreating(optionsBuilder, indexDefinitions);

            return null;
        }

        /// <summary>
        ///     Given a specific collection type, function which determines the name of the collection.  The name is either based
        ///     on an explicit value entered via attribution (using the <see cref="Attributes.Collection" />) custom attribute or
        ///     via a currently configured naming convention function
        /// </summary>
        /// <param name="namingConvention">The function used to derive the collection name if no valid attribution if present</param>
        /// <typeparam name="T">The type of the entities to be contained in the collection</typeparam>
        /// <returns>A name for a given collection</returns>
        private string DeriveCollectionName<T>(Func<string, string> namingConvention)
        {
            Logging.MethodCall(_log);
            var t = typeof(T);
            if (Attributes.HasCustomClassAttribute<Collection>(t))
            {
                var option = Attributes.GetCustomClassAttribute<Collection>(t);
                if (option.IsSome(out var attribute))
                {
                    return attribute.Name ?? namingConvention(t.Name);
                }

                return namingConvention(t.Name);
            }

            return namingConvention(t.Name);
        }
    }
}