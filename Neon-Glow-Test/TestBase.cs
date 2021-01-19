using System.IO;
using Microsoft.Extensions.Configuration;
using Serilog;


namespace JCS.Neon.Glow.Test
{
    public abstract class TestBase
    {
        /// <summary>
        /// The current configuration for tests
        /// </summary>
        protected IConfiguration _configuration;

        /// <summary>
        /// Static logger
        /// </summary>
        private static ILogger _log;

        /// <summary>
        /// Default constructor
        /// </summary>
        protected TestBase()
        {
            LoadConfiguration();
            ConfigureLogging();
        }

        /// <summary>
        /// Configures the logging for tests, based on the current test configuration
        /// </summary>
        private void ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(_configuration)
                .Enrich.WithMachineName()
                .CreateLogger();
            _log = Log.ForContext(typeof(TestBase));
        }

        /// <summary>
        /// Stands up the <see cref="IConfiguration"/> instance to be used during testing
        /// </summary>
        private void LoadConfiguration()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Test.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}