/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
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
    public abstract class MongoDbContext
    {
        /// <summary>
        ///     Static logger
        /// </summary>
        private static readonly ILogger _log = Log.ForContext<MongoDbContext>();

        /// <summary>
        ///     The <see cref="MongoClient" /> instance, available to subclasses
        /// </summary>
        protected MongoClient Client => BuildClient();

        /// <summary>
        ///     The <see cref="MongoClientSettings" /> instance, available to subclasses
        /// </summary>
        protected MongoClientSettings ClientSettings => BuildClientSettings();

        /// <summary>
        ///     Returns the <see cref="MongoDbContextOptions" /> currently in use
        /// </summary>
        protected MongoDbContextOptions DbContextOptions => BuildDbContextOptions();

        /// <summary>
        ///     The underlying <see cref="MongoClient" />
        /// </summary>
        private MongoClient? _client;

        /// <summary>
        ///     The <see cref="MongoClientSettings" /> to use within this context
        /// </summary>
        private MongoClientSettings? _clientSettings;

        /// <summary>
        ///     The <see cref="MongoDbContextOptions" /> instance for this context
        /// </summary>
        private MongoDbContextOptions? _options;


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
            _options = new MongoDbContextOptionsBuilder()
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
            _options = options;
        }

        /// <summary>
        ///     Constructor that takes an <see cref="Action" /> that can take a <see cref="MongoDbContextOptionsBuilder" />
        ///     instance and configure the context options through the builder
        /// </summary>
        /// <param name="configureAction">An <see cref="Action"/> which modifies an instance of <see cref="MongoDbContextOptionsBuilder"/> in order to arrive at a good configuration</param>
        protected MongoDbContext(Action<MongoDbContextOptionsBuilder> configureAction)
        {
            Logging.MethodCall(_log);
            var builder = new MongoDbContextOptionsBuilder();
            configureAction(builder);
            _options = builder.Build();
        }

        /// <summary>
        ///     Checks whether we have a client, and if not builds one using the current <see cref="MongoClientSettings" /> object
        /// </summary>
        /// <returns>An instance of <see cref="MongoClient" /></returns>
        private MongoClient BuildClient()
        {
            lock (this)
            {
                return _client ??= new MongoClient(ClientSettings);
            }
        }

        /// <summary>
        ///     Checks whether we have a current instance of <see cref="MongoDbContextOptions" />, and if not (shouldn't happen)
        ///     just creates one based on the default provided by <see cref="MongoDbContextOptionsBuilder" />
        /// </summary>
        /// <returns>An instance of <see cref="MongoDbContextOptions" /></returns>
        private MongoDbContextOptions BuildDbContextOptions()
        {
            lock (this)
            {
                return _options ??= new MongoDbContextOptionsBuilder().Build();
            }
        }

        /// <summary>
        ///     Takes the currently configured <see cref="MongoDbContextOptions" /> instance and uses them to create an instance of
        ///     <see cref="MongoClientSettings" /> which can then be used to access Mongo
        /// </summary>
        private MongoClientSettings BuildClientSettings()
        {
            Logging.MethodCall(_log);

            lock (this)
            {
                if (_clientSettings is not null)
                {
                    return _clientSettings;
                }

                Assertions.Checked<MongoDbContextException>(_options != null, "No context options have been set");

                Logging.Verbose(_log, $"Building new client settings object from options: {_options}");
                if (!ValidateContextOptions())
                {
                    throw Exceptions.LoggedException<MongoDbContextException>(_log,
                        "Failed to validate context options - check log entries");
                }

                _clientSettings ??= new MongoClientSettings();

                Logging.Verbose(_log, $"Setting client authentication type to {_options?.AuthenticationType}");
                _clientSettings.Credential = _options?.AuthenticationType switch
                {
                    MongoAuthenticationType.Basic => MongoCredential.CreateCredential(_options.AuthenticationDatabase, _options.User,
                        _options.Password),
                    MongoAuthenticationType.X509Certificate => _options.User != null
                        ? MongoCredential.CreateMongoX509Credential(_options.User)
                        : MongoCredential.CreateMongoX509Credential(),
                    _ => _clientSettings.Credential
                };

                Logging.Verbose(_log, $"Setting the channel type to {_options?.ChannelType}");
                if (_options?.ChannelType is MongoChannelType.Secure or MongoChannelType.SecureNoRevocationChecks)
                {
                    _clientSettings.UseTls = true;
                    _clientSettings.SslSettings.ClientCertificates = _options?.ClientCertificates;
                    _clientSettings.SslSettings.CheckCertificateRevocation =
                        _options?.ChannelType != MongoChannelType.SecureNoRevocationChecks;
                    _clientSettings.AllowInsecureTls = _options?.AllowSelfSignedCertificates ?? true;
                }

                if (_options?.ReplicaSet is null)
                {
                    return _clientSettings;
                }

                Logging.Verbose(_log, $"Setting the replica set name is {_options.ReplicaSet}");
                _clientSettings.ReplicaSetName = _options.ReplicaSet;

                return _clientSettings;
            }
        }

        /// <summary>
        ///     Checks/validates the currently configured set of <see cref="MongoDbContextOptions" />
        /// </summary>
        /// <returns><code>true</code> if the options are OK, <code>false</code> otherwise, logging the details</returns>
        private bool ValidateContextOptions()
        {
            Logging.MethodCall(_log);

            if (_options?.Database == null)
            {
                Logging.Error(_log, "A database must be specified within the context options");
                return false;
            }

            if (_options?.AuthenticationDatabase == null)
            {
                Logging.Warning(_log, "No explicit authentication database specified, will used the configured URI database instead");
            }

            switch (_options?.AuthenticationType)
            {
                case MongoAuthenticationType.X509Certificate when _options?.AuthenticationCertificate == null:
                    Logging.Error(_log, "Certificate authentication selected, but no certificate provided");
                    return false;
                case MongoAuthenticationType.Basic when _options?.User == null || _options.Password == null:
                    Logging.Warning(_log, "Basic authentication selected, but missing username or password");
                    return false;
                default:
                    return true;
            }
        }
    }
}