using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using JCS.Neon.Glow.Types;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;

namespace JCS.Neon.Glow.Data.Repository.Mongo
{
    /// <summary>
    ///     Builder class for <see cref="DbContextOptions" />
    /// </summary>
    public class DbContextOptionsBuilder : IBuilder<DbContextOptions>
    {
        /// <summary>
        ///     The actual <see cref="DbContextOptions" /> instance
        /// </summary>
        private readonly DbContextOptions _options = new();

        /// <summary>
        ///     Builds a <see cref="DbContextOptions" /> instance
        /// </summary>
        /// <returns>A fresh, mint-scented <see cref="DbContextOptions" /> instance</returns>
        public DbContextOptions Build()
        {
            return _options;
        }

        /// <summary>
        ///     Sets the <see cref="DbContextOptions.ServerScheme" /> property
        /// </summary>
        /// <param name="scheme">A value from the <see cref="ConnectionStringScheme" /> enumeration</param>
        /// <returns>The current builder instance</returns>
        public DbContextOptionsBuilder Scheme(ConnectionStringScheme scheme)
        {
            _options.ServerScheme = scheme;
            return this;
        }

        /// <summary>
        ///     Sets the <see cref="DbContextOptions.Host" /> property.  The default is 'localhost'
        /// </summary>
        /// <param name="host">A host name (note that this isn't checked for validity</param>
        /// <returns>The current builder instance</returns>
        public DbContextOptionsBuilder Host(string host)
        {
            _options.Host = host;
            return this;
        }

        /// <summary>
        ///     Sets the <see cref="DbContextOptions.Port" /> property.  The default Mongo DB port is 27017.
        /// </summary>
        /// <param name="port">A port number</param>
        /// <returns>The current builder instance</returns>
        public DbContextOptionsBuilder Port(int port)
        {
            _options.Port = port;
            return this;
        }

        /// <summary>
        ///     Sets the <see cref="DbContextOptions.Database" /> property.
        /// </summary>
        /// <param name="databaseName">The name of the database to mount.  Will be created if it doesn't already exist</param>
        /// <returns>The current builder instance</returns>
        public DbContextOptionsBuilder Database(string databaseName)
        {
            _options.Database = databaseName;
            return this;
        }

        /// <summary>
        ///     Sets the <see cref="DbContextOptions.Application" /> property
        /// </summary>
        /// <param name="name">A string containing the required application name</param>
        /// <returns>The current builder instance</returns>
        public DbContextOptionsBuilder Application(string name)
        {
            _options.Application = name;
            return this;
        }

        /// <summary>
        ///     Adds a server address to the current options instance
        /// </summary>
        /// <param name="host">The hostname of the server</param>
        /// <param name="port">The port number of the server</param>
        /// <returns>The current builder instance</returns>
        public DbContextOptionsBuilder ServerAddress(string host, int port)
        {
            _options.AddServerAddress(host, port);
            return this;
        }

        /// <summary>
        ///     Adds a server address to the current options instance, using the default port
        /// </summary>
        /// <param name="host">The hostname of the server</param>
        /// <returns>The current builder instance</returns>
        public DbContextOptionsBuilder ServerAddress(string host)
        {
            _options.AddServerAddress(host);
            return this;
        }

        /// <summary>
        ///     Sets the authentication type to use
        /// </summary>
        /// <param name="authenticationType">A value from the <see cref="Mongo.AuthenticationType" /> enumeration</param>
        /// <returns>The current builder instance</returns>
        public DbContextOptionsBuilder AuthenticationType(AuthenticationType authenticationType)
        {
            _options.AuthenticationType = authenticationType;
            return this;
        }

        /// <summary>
        ///     Sets the channel type to use.
        /// </summary>
        /// <param name="channelType">A valid value from the <see cref="Mongo.ChannelType" /> enumeration</param>
        /// <returns>The current builder instance</returns>
        public DbContextOptionsBuilder ChannelType(ChannelType channelType)
        {
            _options.ChannelType = channelType;
            return this;
        }

        /// <summary>
        ///     Sets the username to be used during authentication
        /// </summary>
        /// <param name="username">A user name</param>
        /// <returns>The current builder instance</returns>
        public DbContextOptionsBuilder User(string username)
        {
            _options.User = username;
            return this;
        }

        /// <summary>
        ///     A password to be used during SCRAM authentication
        /// </summary>
        /// <param name="password">A password</param>
        /// <returns>The current builder instance</returns>
        public DbContextOptionsBuilder Password(string password)
        {
            _options.Password = password;
            return this;
        }

        /// <summary>
        ///     Sets the database to use for authentication.  The default is 'admin'
        /// </summary>
        /// <param name="authenticationDatabase">A database name to use for authentication</param>
        /// <returns>The current builder instance</returns>
        public DbContextOptionsBuilder AuthenticationDatabase(string authenticationDatabase)
        {
            _options.AuthenticationDatabase = authenticationDatabase;
            return this;
        }

        /// <summary>
        ///     Adds a certificate which can be used in order to establish a SSL/TLS tunnel to the Mongo server
        /// </summary>
        /// <param name="certificate">A <see cref="X509Certificate" /></param>
        /// <returns>The current builder instance</returns>
        public DbContextOptionsBuilder ClientCertificate(X509Certificate certificate)
        {
            _options.AddClientCertificate(certificate);
            return this;
        }

        /// <summary>
        ///     Adds a sequence of client certificates to the options
        /// </summary>
        /// <param name="certificates">An series of <see cref="X509Certificate" /> instances to add</param>
        /// <returns>The current builder instance</returns>
        public DbContextOptionsBuilder ClientCertificates(IEnumerable<X509Certificate> certificates)
        {
            foreach (var certificate in certificates)
            {
                _options.AddClientCertificate(certificate);
            }

            return this;
        }

        /// <summary>
        ///     Whether or not self-signed certificates are allowed within the SSL layer
        /// </summary>
        /// <param name="allow">A boolean</param>
        /// <returns>The current <see cref="DbContextOptionsBuilder" /> instance</returns>
        public DbContextOptionsBuilder AllowSelfSignedCertificates(bool allow)
        {
            _options.AllowSelfSignedCertificates = allow;
            return this;
        }

        /// <summary>
        ///     Sets the replica set name
        /// </summary>
        /// <param name="replicaSetName">The name of a replica set</param>
        /// <returns>The current builder instance</returns>
        public DbContextOptionsBuilder ReplicaSet(string replicaSetName)
        {
            _options.ReplicaSet = replicaSetName;
            return this;
        }

        /// <summary>
        ///     The default <see cref="ReadConcern" /> to use for database binding operations
        /// </summary>
        /// <param name="readConcern">A member of <see cref="ReadConcern" /></param>
        /// <returns>The current builder instance</returns>
        public DbContextOptionsBuilder DatabaseReadConcern(ReadConcern readConcern)
        {
            _options.DatabaseReadConcern = readConcern;
            return this;
        }

        /// <summary>
        ///     The default <see cref="WriteConcern" /> to use for database binding operations
        /// </summary>
        /// <param name="writeConcern">A member of <see cref="WriteConcern" /></param>
        /// <returns>The current builder instance</returns>
        public DbContextOptionsBuilder DatabaseWriteConcern(WriteConcern writeConcern)
        {
            _options.DatabaseWriteConcern = writeConcern;
            return this;
        }

        /// <summary>
        ///     The default <see cref="ReadConcern" /> to use for collection binding operations
        /// </summary>
        /// <param name="readConcern">A member of <see cref="ReadConcern" /></param>
        /// <returns>The current builder instance</returns>
        public DbContextOptionsBuilder CollectionReadConcern(ReadConcern readConcern)
        {
            _options.CollectionReadConcern = readConcern;
            return this;
        }

        /// <summary>
        ///     The default <see cref="WriteConcern" /> to use for collection binding operations
        /// </summary>
        /// <param name="writeConcern">A member of <see cref="WriteConcern" /></param>
        /// <returns>The current builder instance</returns>
        public DbContextOptionsBuilder CollectionWriteConcern(WriteConcern writeConcern)
        {
            _options.CollectionWriteConcern = writeConcern;
            return this;
        }
    }
}