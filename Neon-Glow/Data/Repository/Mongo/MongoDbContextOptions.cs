#region

using System.Collections.Generic;
using JCS.Neon.Glow.Statics.Reflection;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;

#endregion

namespace JCS.Neon.Glow.Data.Repository.Mongo
{
    /// <summary>
    ///     Class for containing Mongo DB context options
    /// </summary>
    public class MongoDbContextOptions
    {
        /// <summary>
        ///     The default application name value
        /// </summary>
        public static readonly string DefaultApplicationName = $"neon-glow-{Assemblies.GetApplicationAssemblyVersion()}";

        /// <summary>
        ///     The connection scheme, depends on whether you are connecting to a standalone instance/RS or an Atlas instance
        /// </summary>
        public ConnectionStringScheme ServerScheme { get; set; } = ConnectionStringScheme.MongoDB;

        /// <summary>
        ///     The server host name (defaults to localhost)
        /// </summary>
        public string ServerHost { get; set; } = "localhost";

        /// <summary>
        ///     The server port (defaults to 27017)
        /// </summary>
        public int ServerPort { get; set; } = 27017;

        /// <summary>
        ///     An optional application name
        /// </summary>
        public string ApplicationName { get; set; } = DefaultApplicationName;

        /// <summary>
        ///     Accesses a <see cref="MongoClientSettings" /> instance based on the current options
        /// </summary>
        public MongoClientSettings Settings => BuildSettings();

        /// <summary>
        ///     Accesses a <see cref="MongoServerAddress" /> instance based on the current options
        /// </summary>
        public MongoServerAddress ServerAddress => BuildServerAddress();

        /// <summary>
        ///     A list of <see cref="MongoServerAddress" /> instances.  If this list is non-empty, then it is used to construct
        /// </summary>
        public IEnumerable<MongoServerAddress> ServerAddresses => BuildServerAddresses();

        /// <summary>
        ///     Internal collection of
        /// </summary>
        private List<MongoServerAddress> _serverAddresses { get; } = new();

        /// <summary>
        ///     Builds a single <see cref="MongoServerAddress" /> based on the <see cref="ServerHost" /> and
        ///     <see cref="ServerPort" /> property values
        /// </summary>
        /// <returns>A new <see cref="MongoServerAddress" /> instance</returns>
        private MongoServerAddress BuildServerAddress()
        {
            return new(ServerHost, ServerPort);
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
        ///     Takes the relevant options and builds an instance of <see cref="MongoClientSettings" />
        /// </summary>
        /// <returns>A new <see cref="MongoClientSettings" /> instance</returns>
        private MongoClientSettings BuildSettings()
        {
            return new()
            {
                ApplicationName = ApplicationName,
                Scheme = ServerScheme
            };
        }
    }

    /// <summary>
    ///     Builder class for <see cref="MongoDbContextOptions" />
    /// </summary>
    public class MongoDbContextOptionsBuilder
    {
        /// <summary>
        ///     The actual <see cref="MongoDbContextOptions" /> instance
        /// </summary>
        private readonly MongoDbContextOptions Options = new();

        /// <summary>
        ///     Sets the <see cref="MongoDbContextOptions.ServerScheme" /> property
        /// </summary>
        /// <param name="scheme">A value from the <see cref="ConnectionStringScheme" /> enumeration</param>
        /// <returns>The current builder instance</returns>
        public MongoDbContextOptionsBuilder SetScheme(ConnectionStringScheme scheme)
        {
            Options.ServerScheme = scheme;
            return this;
        }

        /// <summary>
        ///     Sets the <see cref="MongoDbContextOptions.ServerHost" /> property.  The default is 'localhost'
        /// </summary>
        /// <param name="host">A host name (note that this isn't checked for validity</param>
        /// <returns>The current builder instance</returns>
        public MongoDbContextOptionsBuilder SetServerHost(string host)
        {
            Options.ServerHost = host;
            return this;
        }

        /// <summary>
        ///     Sets the <see cref="MongoDbContextOptions.ServerPort" /> property.  The default Mongo DB port is 27017.
        /// </summary>
        /// <param name="port">A port number</param>
        /// <returns>The current builder instance</returns>
        public MongoDbContextOptionsBuilder SetServerPort(int port)
        {
            Options.ServerPort = port;
            return this;
        }

        /// <summary>
        ///     Sets the <see cref="MongoDbContextOptions.ApplicationName" /> property
        /// </summary>
        /// <param name="name">A string containing the required application name</param>
        /// <returns>The current builder instance</returns>
        public MongoDbContextOptionsBuilder SetApplicationName(string name)
        {
            Options.ApplicationName = name;
            return this;
        }

        /// <summary>
        ///     Adds a server address to the current options instance
        /// </summary>
        /// <param name="host">The hostname of the server</param>
        /// <param name="port">The port number of the server</param>
        /// <returns>The current builder instance</returns>
        public MongoDbContextOptionsBuilder AddServerAddress(string host, int port)
        {
            Options.AddServerAddress(host, port);
            return this;
        }

        /// <summary>
        ///     Adds a server address to the current options instance, using the default port
        /// </summary>
        /// <param name="host">The hostname of the server</param>
        /// <returns>The current builder instance</returns>
        public MongoDbContextOptionsBuilder AddServerAddress(string host)
        {
            Options.AddServerAddress(host);
            return this;
        }
    }
}