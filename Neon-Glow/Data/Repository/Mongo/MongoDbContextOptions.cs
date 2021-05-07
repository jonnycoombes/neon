#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using JCS.Neon.Glow.Statics;
using JCS.Neon.Glow.Statics.Reflection;
using JCS.Neon.Glow.Types;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using Serilog;

#endregion

namespace JCS.Neon.Glow.Data.Repository.Mongo
{
    /// <summary>
    ///     Enumeration for denoting the type of authentication to use in order to connect to Mongo
    /// </summary>
    public enum MongoAuthenticationType
    {
        /// <summary>
        ///     Basic authentication, using SCRAM (Salted Challenge Response Authentication Mechanism).  Username and password
        ///     required.
        /// </summary>
        Basic,

        /// <summary>
        ///     Certificate based authentication.  A valid X509 certificate is required, with associated private key material
        /// </summary>
        X509Certificate
    }

    /// <summary>
    ///     Enumeration for denoting the type of channel to use in order to connect to Mongo
    /// </summary>
    public enum MongoChannelType
    {
        /// <summary>
        ///     No channel encryption (DO NOT USE IN PRODUCTION)
        /// </summary>
        PlainText,

        /// <summary>
        ///     Secure sockets utilising TLS 1.1 or higher
        /// </summary>
        Secure,

        /// <summary>
        ///     Secure sockets utilising TLS 1.1 or higher, but with no certificate revocation checking.  (Useful for debug).
        /// </summary>
        SecureNoRevocationChecks
    }

    /// <summary>
    ///     Class for containing Mongo DB context options
    /// </summary>
    public class MongoDbContextOptions
    {
        /// <summary>
        ///     The default port used to connect through to a Mongo DB instance
        /// </summary>
        public const int DefaultServerPort = 27017;

        /// <summary>
        ///     The default host name used to connect through to a Mongo DB instance
        /// </summary>
        public const string DefaultServerHost = "localhost";

        /// <summary>
        ///     Static logger for this class
        /// </summary>
        private static readonly ILogger _log = Log.ForContext<MongoDbContextOptions>();

        /// <summary>
        ///     The default application name value
        /// </summary>
        public static readonly string DefaultApplicationName =
            $"neon-glow-{Assemblies.GetApplicationAssemblyVersion()}";

        /// <summary>
        ///     The connection scheme, depends on whether you are connecting to a standalone instance/RS or an Atlas instance
        /// </summary>
        public ConnectionStringScheme ServerScheme { get; set; } = ConnectionStringScheme.MongoDB;

        /// <summary>
        ///     The server host name (defaults to localhost)
        /// </summary>
        public string Host { get; set; } = DefaultServerHost;

        /// <summary>
        ///     The server port (defaults to 27017)
        /// </summary>
        public int Port { get; set; } = DefaultServerPort;

        /// <summary>
        ///     An optional application name
        /// </summary>
        public string Application { get; set; } = DefaultApplicationName;

        /// <summary>
        ///     The database that the context will mount to
        /// </summary>
        public string? Database { get; set; }

        /// <summary>
        ///     An optional replica set name, which may be used to construct a valid connection string
        /// </summary>
        public string? ReplicaSet { get; set; }

        /// <summary>
        ///     The <see cref="MongoAuthenticationType" /> to use
        /// </summary>
        public MongoAuthenticationType AuthenticationType { get; set; } = MongoAuthenticationType.Basic;

        /// <summary>
        ///     The <see cref="MongoChannelType" /> to use
        /// </summary>
        public MongoChannelType ChannelType { get; set; } = MongoChannelType.PlainText;

        /// <summary>
        ///     Accesses a <see cref="MongoServerAddress" /> instance based on the current options
        /// </summary>
        public MongoServerAddress ServerAddress => BuildServerAddress();

        /// <summary>
        ///     Whether or not we allow self-signed certificates within the SSL layer
        /// </summary>
        public bool AllowSelfSignedCertificates { get; set; } = true;

        /// <summary>
        ///     A list of <see cref="MongoServerAddress" /> instances.  If this list is non-empty, then it is used to construct
        /// </summary>
        public IEnumerable<MongoServerAddress> ServerAddresses => BuildServerAddresses();

        /// <summary>
        ///     Internal collection of <see cref="MongoServerAddress" /> objects
        /// </summary>
        private List<MongoServerAddress> _serverAddresses { get; } = new();

        /// <summary>
        ///     An internal list of <see cref="X509Certificate" /> instances
        /// </summary>
        private List<X509Certificate> _clientCertificates { get; } = new();

        /// <summary>
        ///     Returns the currently configured list of possible client certificates
        /// </summary>
        public IEnumerable<X509Certificate> ClientCertificates => BuildClientCertificates();

        /// <summary>
        ///     An optional authentication database name
        /// </summary>
        public string? AuthenticationDatabase { get; set; }

        /// <summary>
        ///     An optional username to use for authentication.  If SCRAM authentication is chosen, this field needs to be non-null
        ///     and a password also needs to be provided.  If X509Certificate authentication is selected, then this username needs
        ///     to match the FQN present in the selected certificate.
        /// </summary>
        public string? User { get; set; }

        /// <summary>
        ///     An optional password to use for SCRAM authentication.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        ///     The default read concern to use for database binding operations
        /// </summary>
        public ReadConcern DatabaseReadConcern { get; set; } = ReadConcern.Default;

        /// <summary>
        ///     The default write concern to use for database binding operations
        /// </summary>
        public WriteConcern DatabaseWriteConcern { get; set; } = WriteConcern.Unacknowledged;

        /// <summary>
        ///     The default read concern to use for collection binding operations
        /// </summary>
        public ReadConcern CollectionReadConcern { get; set; } = ReadConcern.Default;

        /// <summary>
        ///     The default write concern to use for collection binding operations
        /// </summary>
        public WriteConcern CollectionWriteConcern { get; set; } = WriteConcern.Unacknowledged;

        /// <summary>
        ///     Returns the internal list of client <see cref="X509Certificate" /> instances to be used when SSL is selected as the
        ///     channel type
        /// </summary>
        /// <returns>An <see cref="IEnumerable{X509Certificate}" /></returns>
        private IEnumerable<X509Certificate> BuildClientCertificates()
        {
            return _clientCertificates;
        }

        /// <summary>
        ///     Builds a single <see cref="MongoServerAddress" /> based on the <see cref="Host" /> and
        ///     <see cref="Port" /> property values
        /// </summary>
        /// <returns>A new <see cref="MongoServerAddress" /> instance</returns>
        private MongoServerAddress BuildServerAddress()
        {
            return new(Host, Port);
        }

        /// <summary>
        ///     Returns the current list of <see cref="MongoServerAddress" /> instances configured for the context
        /// </summary>
        /// <returns></returns>
        private IEnumerable<MongoServerAddress> BuildServerAddresses()
        {
            return _serverAddresses;
        }

        /// <summary>
        ///     Adds a new <see cref="X509Certifcate" /> to the list of available certificates
        /// </summary>
        /// <param name="certificate">A valid <see cref="X509Certificate" /> instance</param>
        public void AddClientCertificate(X509Certificate certificate)
        {
            _clientCertificates.Add(certificate);
        }

        /// <summary>
        ///     Adds a new server address to the internal server address collection
        /// </summary>
        /// <param name="host">The server host name</param>
        /// <param name="port">The server port number</param>
        public void AddServerAddress(string host, int port)
        {
            _serverAddresses.Add(new MongoServerAddress(host, port));
        }

        /// <summary>
        ///     Adds a new server address to the internal server address collection using the default Mongo port of 27017
        /// </summary>
        /// <param name="host">The server host name</param>
        public void AddServerAddress(string host)
        {
            _serverAddresses.Add(new MongoServerAddress(host));
        }

        /// <summary>
        ///     Overridden formatting member, just dumps the current options object as a JSON string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Password is not null)
            {
                return $"{JsonSerializer.Serialize(this)}".Replace(Password, "XXXXXXXXX");
            }

            return $"{JsonSerializer.Serialize(this)}";
        }

        /// <summary>
        ///     Takes a validation function, and will then attempt to convert the current <see cref="MongoDbContextOptions" />
        ///     instance to an instance of <see cref="MongoClientSettings" />
        /// </summary>
        /// <param name="optionsValidationFunc">
        ///     A function that will be passed the current instance of <see cref="MongoDbContextOptions" /> and should return true
        ///     or false
        /// </param>
        /// <returns>A new instance of <see cref="MongoClientSettings" /></returns>
        /// <exception cref="MongoDbContextException">Thrown if validation of the current options fails</exception>
        public MongoClientSettings BuildClientSettings(Func<MongoDbContextOptions, bool>? optionsValidationFunc = null)
        {
            Logging.MethodCall(_log);
            Logging.Verbose(_log, $"Build a new client settings based on {this}");
            var clientSettings = new MongoClientSettings();
            optionsValidationFunc ??= DefaultOptionsValidationFunction;
            if (optionsValidationFunc(this))
            {
                if (ReplicaSet is not null)
                {
                    Logging.Verbose(_log, $"Setting replica set to {ReplicaSet}");
                    clientSettings.ReplicaSetName = ReplicaSet;
                }

                if (AuthenticationType == MongoAuthenticationType.Basic)
                {
                    Logging.Verbose(_log, "Configuring for basic authentication");
                    clientSettings.Credential =
                        MongoCredential.CreateCredential(AuthenticationDatabase, User, Password);
                }
                else
                {
                    Logging.Verbose(_log, "Configuring for X509 authentication");
                    clientSettings.Credential = User is not null
                        ? MongoCredential.CreateMongoX509Credential(User)
                        : MongoCredential.CreateMongoX509Credential();
                }

                if (ChannelType is MongoChannelType.Secure or MongoChannelType.SecureNoRevocationChecks)
                {
                    Logging.Verbose(_log, "Using TLS over-the-wire for Mongo connection(s)");
                    clientSettings.UseTls = true;
                    clientSettings.SslSettings.ClientCertificates = ClientCertificates;
                    clientSettings.SslSettings.CheckCertificateRevocation =
                        ChannelType is not MongoChannelType.SecureNoRevocationChecks;
                    if (AllowSelfSignedCertificates)
                    {
                        Logging.Warning(_log,
                            "Allowing the use of self-signed certificates. Not recommanded for production systems");
                    }

                    clientSettings.AllowInsecureTls = AllowSelfSignedCertificates;
                }
                else
                {
                    Logging.Warning(_log,
                        "Running with non-TLS connection through to Mongo.  Not recommended for production systems");
                }
            }
            else
            {
                throw Exceptions.LoggedException<MongoDbContextException>(_log,
                    $"An invalid set of options have been specified: {this}");
            }

            return clientSettings;
        }

        /// <summary>
        ///     Static function used in order to validate the current <see cref="MongoDbContextOptions" />
        /// </summary>
        /// <param name="options">The <see cref="MongoDbContextOptions" /> instance to be validated</param>
        /// <returns><code>true</code> or <code>false</code> depending on whether the options look to be valid</returns>
        private static bool DefaultOptionsValidationFunction(MongoDbContextOptions options)
        {
            Logging.MethodCall(_log);
            if (options.Database is null)
            {
                Logging.Error(_log, "A valid database hasn't been specified");
                return false;
            }

            if (options.AuthenticationDatabase is null)
            {
                Logging.Warning(_log, "No explicit authentication database specified");
            }

            switch (options.AuthenticationType)
            {
                case MongoAuthenticationType.Basic:
                    if (options.User is null || options.Password is null)
                    {
                        Logging.Error(_log, "Null usernames or passwords are not allowed");
                        return false;
                    }

                    break;
                default:
                    if (!options.ClientCertificates.Any())
                    {
                        Logging.Warning(_log, "x509 authentication selected, but no client certificates supplied");
                    }

                    break;
            }

            return true;
        }
    }

    /// <summary>
    ///     Builder class for <see cref="MongoDbContextOptions" />
    /// </summary>
    public class MongoDbContextOptionsBuilder : IBuilder<MongoDbContextOptions>
    {
        /// <summary>
        ///     The actual <see cref="MongoDbContextOptions" /> instance
        /// </summary>
        private readonly MongoDbContextOptions _options = new();

        /// <summary>
        ///     Builds a <see cref="MongoDbContextOptions" /> instance
        /// </summary>
        /// <returns>A fresh, mint-scented <see cref="MongoDbContextOptions" /> instance</returns>
        public MongoDbContextOptions Build()
        {
            return _options;
        }

        /// <summary>
        ///     Sets the <see cref="MongoDbContextOptions.ServerScheme" /> property
        /// </summary>
        /// <param name="scheme">A value from the <see cref="ConnectionStringScheme" /> enumeration</param>
        /// <returns>The current builder instance</returns>
        public MongoDbContextOptionsBuilder Scheme(ConnectionStringScheme scheme)
        {
            _options.ServerScheme = scheme;
            return this;
        }

        /// <summary>
        ///     Sets the <see cref="MongoDbContextOptions.Host" /> property.  The default is 'localhost'
        /// </summary>
        /// <param name="host">A host name (note that this isn't checked for validity</param>
        /// <returns>The current builder instance</returns>
        public MongoDbContextOptionsBuilder Host(string host)
        {
            _options.Host = host;
            return this;
        }

        /// <summary>
        ///     Sets the <see cref="MongoDbContextOptions.Port" /> property.  The default Mongo DB port is 27017.
        /// </summary>
        /// <param name="port">A port number</param>
        /// <returns>The current builder instance</returns>
        public MongoDbContextOptionsBuilder Port(int port)
        {
            _options.Port = port;
            return this;
        }

        /// <summary>
        ///     Sets the <see cref="MongoDbContextOptions.Database" /> property.
        /// </summary>
        /// <param name="databaseName">The name of the database to mount.  Will be created if it doesn't already exist</param>
        /// <returns>The current builder instance</returns>
        public MongoDbContextOptionsBuilder Database(string databaseName)
        {
            _options.Database = databaseName;
            return this;
        }

        /// <summary>
        ///     Sets the <see cref="MongoDbContextOptions.Application" /> property
        /// </summary>
        /// <param name="name">A string containing the required application name</param>
        /// <returns>The current builder instance</returns>
        public MongoDbContextOptionsBuilder Application(string name)
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
        public MongoDbContextOptionsBuilder ServerAddress(string host, int port)
        {
            _options.AddServerAddress(host, port);
            return this;
        }

        /// <summary>
        ///     Adds a server address to the current options instance, using the default port
        /// </summary>
        /// <param name="host">The hostname of the server</param>
        /// <returns>The current builder instance</returns>
        public MongoDbContextOptionsBuilder ServerAddress(string host)
        {
            _options.AddServerAddress(host);
            return this;
        }

        /// <summary>
        ///     Sets the authentication type to use
        /// </summary>
        /// <param name="authenticationType">A value from the <see cref="MongoAuthenticationType" /> enumeration</param>
        /// <returns>The current builder instance</returns>
        public MongoDbContextOptionsBuilder AuthenticationType(MongoAuthenticationType authenticationType)
        {
            _options.AuthenticationType = authenticationType;
            return this;
        }

        /// <summary>
        ///     Sets the channel type to use.
        /// </summary>
        /// <param name="channelType">A valid value from the <see cref="MongoChannelType" /> enumeration</param>
        /// <returns>The current builder instance</returns>
        public MongoDbContextOptionsBuilder ChannelType(MongoChannelType channelType)
        {
            _options.ChannelType = channelType;
            return this;
        }

        /// <summary>
        ///     Sets the username to be used during authentication
        /// </summary>
        /// <param name="username">A user name</param>
        /// <returns>The current builder instance</returns>
        public MongoDbContextOptionsBuilder User(string username)
        {
            _options.User = username;
            return this;
        }

        /// <summary>
        ///     A password to be used during SCRAM authentication
        /// </summary>
        /// <param name="password">A password</param>
        /// <returns>The current builder instance</returns>
        public MongoDbContextOptionsBuilder Password(string password)
        {
            _options.Password = password;
            return this;
        }

        /// <summary>
        ///     Sets the database to use for authentication.  The default is 'admin'
        /// </summary>
        /// <param name="authenticationDatabase">A database name to use for authentication</param>
        /// <returns>The current builder instance</returns>
        public MongoDbContextOptionsBuilder AuthenticationDatabase(string authenticationDatabase)
        {
            _options.AuthenticationDatabase = authenticationDatabase;
            return this;
        }

        /// <summary>
        ///     Adds a certificate which can be used in order to establish a SSL/TLS tunnel to the Mongo server
        /// </summary>
        /// <param name="certificate">A <see cref="X509Certificate" /></param>
        /// <returns>The current builder instance</returns>
        public MongoDbContextOptionsBuilder ClientCertificate(X509Certificate certificate)
        {
            _options.AddClientCertificate(certificate);
            return this;
        }

        /// <summary>
        ///     Adds a sequence of client certificates to the options
        /// </summary>
        /// <param name="certificates">An series of <see cref="X509Certificate" /> instances to add</param>
        /// <returns>The current builder instance</returns>
        public MongoDbContextOptionsBuilder ClientCertificates(IEnumerable<X509Certificate> certificates)
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
        /// <returns>The current <see cref="MongoDbContextOptionsBuilder" /> instance</returns>
        public MongoDbContextOptionsBuilder AllowSelfSignedCertificates(bool allow)
        {
            _options.AllowSelfSignedCertificates = allow;
            return this;
        }

        /// <summary>
        ///     Sets the replica set name
        /// </summary>
        /// <param name="replicaSetName">The name of a replica set</param>
        /// <returns>The current builder instance</returns>
        public MongoDbContextOptionsBuilder ReplicaSet(string replicaSetName)
        {
            _options.ReplicaSet = replicaSetName;
            return this;
        }

        /// <summary>
        ///     The default <see cref="ReadConcern" /> to use for database binding operations
        /// </summary>
        /// <param name="readConcern">A member of <see cref="ReadConcern" /></param>
        /// <returns>The current builder instance</returns>
        public MongoDbContextOptionsBuilder DatabaseReadConcern(ReadConcern readConcern)
        {
            _options.DatabaseReadConcern = readConcern;
            return this;
        }

        /// <summary>
        ///     The default <see cref="WriteConcern" /> to use for database binding operations
        /// </summary>
        /// <param name="writeConcern">A member of <see cref="WriteConcern" /></param>
        /// <returns>The current builder instance</returns>
        public MongoDbContextOptionsBuilder DatabaseWriteConcern(WriteConcern writeConcern)
        {
            _options.DatabaseWriteConcern = writeConcern;
            return this;
        }

        /// <summary>
        ///     The default <see cref="ReadConcern" /> to use for collection binding operations
        /// </summary>
        /// <param name="readConcern">A member of <see cref="ReadConcern" /></param>
        /// <returns>The current builder instance</returns>
        public MongoDbContextOptionsBuilder CollectionReadConcern(ReadConcern readConcern)
        {
            _options.CollectionReadConcern = readConcern;
            return this;
        }

        /// <summary>
        ///     The default <see cref="WriteConcern" /> to use for collection binding operations
        /// </summary>
        /// <param name="writeConcern">A member of <see cref="WriteConcern" /></param>
        /// <returns>The current builder instance</returns>
        public MongoDbContextOptionsBuilder CollectionWriteConcern(WriteConcern writeConcern)
        {
            _options.CollectionWriteConcern = writeConcern;
            return this;
        }
    }
}