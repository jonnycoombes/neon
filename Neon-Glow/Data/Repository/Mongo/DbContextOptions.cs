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
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Serialization;
using JCS.Neon.Glow.Statics;
using JCS.Neon.Glow.Statics.Reflection;
using JCS.Neon.Glow.Types.Extensions;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using Serilog;

#endregion

namespace JCS.Neon.Glow.Data.Repository.Mongo
{
    /// <summary>
    ///     Enumeration for denoting the type of authentication to use in order to connect to Mongo
    /// </summary>
    public enum AuthenticationType
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
    public enum ChannelType
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
    public class DbContextOptions
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
        private static readonly ILogger _log = Log.ForContext<DbContextOptions>();

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
        ///     The <see cref="Mongo.AuthenticationType" /> to use
        /// </summary>
        public AuthenticationType AuthenticationType { get; set; } = AuthenticationType.Basic;

        /// <summary>
        ///     The <see cref="Mongo.ChannelType" /> to use
        /// </summary>
        public ChannelType ChannelType { get; set; } = ChannelType.PlainText;

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
        ///     A function that can take a collection type name and then apply any naming conventions based on this.  This will be
        ///     used during collection bind operations
        /// </summary>
        [JsonIgnore]
        public Func<string, string> CollectionNamingConvention { get; set; } = s => s.ToCamelCase();

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
        ///     Takes a validation function, and will then attempt to convert the current <see cref="DbContextOptions" />
        ///     instance to an instance of <see cref="MongoClientSettings" />
        /// </summary>
        /// <param name="optionsValidationFunc">
        ///     A function that will be passed the current instance of <see cref="DbContextOptions" /> and should return true
        ///     or false
        /// </param>
        /// <returns>A new instance of <see cref="MongoClientSettings" /></returns>
        /// <exception cref="DbContextException">Thrown if validation of the current options fails</exception>
        public MongoClientSettings BuildClientSettings(Func<DbContextOptions, bool>? optionsValidationFunc = null)
        {
            Logging.MethodCall(_log);
            Logging.Verbose(_log, $"Build a new client settings based on {this.ToString()}");
            var clientSettings = new MongoClientSettings();
            optionsValidationFunc ??= DefaultOptionsValidationFunction;
            if (optionsValidationFunc(this))
            {
                if (ReplicaSet is not null)
                {
                    Logging.Verbose(_log, $"Setting replica set to {ReplicaSet}");
                    clientSettings.ReplicaSetName = ReplicaSet;
                }

                if (AuthenticationType == AuthenticationType.Basic)
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

                if (ChannelType is ChannelType.Secure or ChannelType.SecureNoRevocationChecks)
                {
                    Logging.Verbose(_log, "Using TLS over-the-wire for Mongo connection(s)");
                    clientSettings.UseTls = true;
                    clientSettings.SslSettings.ClientCertificates = ClientCertificates;
                    clientSettings.SslSettings.CheckCertificateRevocation =
                        ChannelType is not ChannelType.SecureNoRevocationChecks;
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
                throw Exceptions.LoggedException<DbContextException>(_log,
                    $"An invalid set of options have been specified: {this}");
            }

            return clientSettings;
        }

        /// <summary>
        ///     Static function used in order to validate the current <see cref="DbContextOptions" />
        /// </summary>
        /// <param name="options">The <see cref="DbContextOptions" /> instance to be validated</param>
        /// <returns><code>true</code> or <code>false</code> depending on whether the options look to be valid</returns>
        private static bool DefaultOptionsValidationFunction(DbContextOptions options)
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
                case AuthenticationType.Basic:
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
}