/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JCS.Neon.Glow.Statics;
using JCS.Neon.Glow.Statics.Reflection;
using JCS.Neon.Glow.Types;
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
            Logging.MethodCall(_log);
            return BindCollection<T>(settings, token);
        }

        /// <inheritdoc cref="IDbContext.Queryable{T}" />
        public IQueryable<T> Queryable<T>(MongoCollectionSettings? settings, CancellationToken token)
        {
            Logging.MethodCall(_log);
            return BindCollection<T>(settings, token).AsQueryable();
        }

        /// <inheritdoc cref="IDbContext.DatabaseExists" />
        public bool DatabaseExists(string databaseName)
        {
            Logging.MethodCall(_log);
            try
            {
                return Client
                    .ListDatabaseNames()
                    .ToList()
                    .Any(s => s == databaseName);
            }
            catch (Exception ex)
            {
                Logging.Error(_log, $"Mongo Exception: \"{ex.Message}\"");
                throw Exceptions.LoggedException<DbContextException>(_log,
                    $"Timed out connecting to \"{databaseName}\"", ex);
            }
        }

        /// <inheritdoc cref="IDbContext.CollectionExists" />
        public bool CollectionExists(string collectionName)
        {
            Logging.MethodCall(_log);
            try
            {
                return Database.ListCollectionNames()
                    .ToList()
                    .Any(s => s == collectionName);
            }
            catch (Exception ex)
            {
                Logging.Error(_log, $"Mongo Exception: \"{ex.Message}\"");
                throw Exceptions.LoggedException<DbContextException>(_log,
                    $"Timed out whilst looking for collection \"{collectionName}\"", ex);
            }
        }

        /// <summary>
        ///     Checks whether we already have a specific bound collection
        /// </summary>
        /// <typeparam name="T">The type of entities to be stored within the collection</typeparam>
        /// <returns>True or false</returns>
        public bool HaveBoundCollection<T>()
        {
            Logging.MethodCall(_log);
            lock (_boundCollections)
            {
                return _boundCollections.ContainsKey(typeof(T));
            }
        }

        /// <summary>
        ///     Adds a bound collection to the cache of bound collections
        /// </summary>
        /// <param name="collection">The collection to add</param>
        /// <typeparam name="T">The type of the entities stored within the collection</typeparam>
        public void AddBoundCollection<T>(IMongoCollection<T> collection)
        {
            Logging.MethodCall(_log);
            lock (_boundCollections)
            {
                if (!HaveBoundCollection<T>())
                {
                    _boundCollections[typeof(T)] = collection;
                }
            }
        }

        /// <summary>
        ///     Retrieves a bound collection instance
        /// </summary>
        /// <typeparam name="T">The type of the entities stored within the collection</typeparam>
        /// <returns></returns>
        public Option<IMongoCollection<T>> GetBoundCollection<T>()
        {
            Logging.MethodCall(_log);
            lock (_boundCollections)
            {
                if (HaveBoundCollection<T>())
                {
                    return Option<IMongoCollection<T>>.Some((IMongoCollection<T>) _boundCollections[typeof(T)]);
                }

                return Option<IMongoCollection<T>>.None;
            }
        }

        /// <summary>
        ///     Checks whether we have a client, and if not builds one using the current <see cref="MongoClientSettings" /> object.
        /// </summary>
        /// <returns>An instance of <see cref="MongoClient" /></returns>
        private MongoClient BindClient()
        {
            Logging.MethodCall(_log);
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
        /// <param name="bindingType">The type of binding being performed</param>
        /// <param name="builder">
        ///     An instance of <see cref="DatabaseSettingsBuilder" /> which is used to construct the
        ///     database settings.
        /// </param>
        protected virtual void OnDatabaseBinding(BindingType bindingType, ref DatabaseSettingsBuilder builder)
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
        /// <typeparam name="T">The type to be stored within the collection</typeparam>
        protected virtual void OnCollectionCreating<T>(ref CreateCollectionOptionsBuilder optionsBuilder,
            CancellationToken cancellationToken = default)
        {
            Logging.MethodCall(_log);
            var runtimeType = typeof(T);
            Logging.Verbose(_log, $"Creating collection for entity type of \"{runtimeType}\"");
            if (Attributes.GetCustomAttribute<Collection>(AttributeTargets.Class, runtimeType).IsSome(out var collectionAttribute))
            {
                Logging.Verbose(_log, "Found a collection attribute, using this to set options");
                optionsBuilder
                    .ValidationAction(collectionAttribute.ValidationAction)
                    .ValidationLevel(collectionAttribute.ValidationLevel)
                    .Capped(collectionAttribute.Capped)
                    .MaxDocuments(collectionAttribute.MaxDocuments)
                    .MaxSize(collectionAttribute.MaxSize)
                    .UsePowerOf2Sizes(collectionAttribute.PowerOf2Sizes);
            }
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
            OnDatabaseBinding(!DatabaseExists(Options.Database!) ? BindingType.Create : BindingType.Existing, ref builder);
            return Client.GetDatabase(Options.Database, builder.Build());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="DbContextException"></exception>
        private async void NewCollection<T>(string collectionName, CreateCollectionOptions options, CancellationToken cancellationToken)
        {
            Logging.MethodCall(_log);
            try
            {
                await Database.CreateCollectionAsync(collectionName, options, cancellationToken);
                var collection = Database.GetCollection<T>(collectionName);
            }
            catch (Exception ex)
            {
                Logging.Error(_log, $"Mongo Exception: \"{ex.Message}\"");
                throw Exceptions.LoggedException<DbContextException>(_log, "An exception was caught creating a new collection");
            }
        }

        /// <summary>
        ///     Checks whether we currently have a binding into a collection for objects of type <see cref="T" />, and if not
        ///     performs any necessary bind operations
        /// </summary>
        /// <param name="settings">
        ///     An optional set of <see cref="MongoCollectionSettings" /> which may be passed in to influence the bind
        /// </param>
        /// <param name="cancellationToken">An optional <see cref="CancellationToken" /></param>
        /// <typeparam name="T">
        ///     The type of objects stored within the underlying <see cref="IMongoCollection{TDocument}" />
        /// </typeparam>
        /// <returns>A bound instance of <see cref="IMongoCollection{TDocument}" /></returns>
        private IMongoCollection<T> BindCollection<T>(MongoCollectionSettings? settings,
            CancellationToken cancellationToken = default)
        {
            Logging.MethodCall(_log);
            Logging.Verbose(_log, $"Binding collection for type \"{typeof(T)}\"");

            var collectionType = typeof(T);
            if (GetBoundCollection<T>().IsSome(out var boundCollection))
            {
                Logging.Verbose(_log,
                    $"Collection for type \"{collectionType}\" already bound, returning existing instance");
                return boundCollection;
            }

            // derive the name for the collection
            Logging.Verbose(_log, $"No bound collection found for type \"{collectionType}\"");
            var collectionName = DeriveCollectionName<T>(Options.CollectionNamingConvention);
            Logging.Verbose(_log, $"Derived collection name is given as \"{collectionName}\"");

            // check to see if we need to create a new collection 
            if (!CollectionExists(collectionName))
            {
                // create a new collection and bind to that
                Logging.Verbose(_log, $"Creating a new collection \"{collectionName}\" for type \"{collectionType.FullName}\"");
                var optionsBuilder = new CreateCollectionOptionsBuilder();
                OnCollectionCreating<T>(ref optionsBuilder, cancellationToken);
                NewCollection<T>(collectionName, optionsBuilder.Build(), cancellationToken);
            }

            Logging.Verbose(_log, $"Binding to collection \"{collectionName}\" for type \"{collectionType.FullName}\"");
            var settingsBuilder = new CollectionSettingsBuilder(settings);
            OnCollectionBinding<T>(BindingType.Existing, settingsBuilder);
            var collection = Database.GetCollection<T>(collectionName, settingsBuilder.Build());
            AddBoundCollection(collection);
            return collection;
        }

        /// <summary>
        ///     Given a specific collection type, function which determines the name of the collection.  The name is either based
        ///     on an explicit value entered via attribution (using the <see cref="Attributes.Collection" />) custom attribute or
        ///     via a currently configured naming convention function
        /// </summary>
        /// <param name="namingConvention">The function used to derive the collection name if no valid attribution if present</param>
        /// <typeparam name="T">The type of the entities to be contained in the collection</typeparam>
        /// <returns>A name for a given collection</returns>
        private static string DeriveCollectionName<T>(Func<string, string> namingConvention)
        {
            Logging.MethodCall(_log);
            var t = typeof(T);
            var option = Attributes.GetCustomAttribute<Collection>(AttributeTargets.Class, t);
            if (option.IsSome(out var attribute))
            {
                return attribute.Name ?? namingConvention(t.Name);
            }

            return namingConvention(t.Name);
        }
    }
}