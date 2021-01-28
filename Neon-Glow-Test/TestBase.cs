#region

using System.IO;
using System.Security.Cryptography.X509Certificates;
using JCS.Neon.Glow.Types;
using JCS.Neon.Glow.Utilities.Cryptography;
using JCS.Neon.Glow.Utilities.General;
using Microsoft.Extensions.Configuration;
using Serilog;

#endregion

namespace JCS.Neon.Glow.Test
{
    public abstract class TestBase
    {
        /// <summary>
        ///     Static logger
        /// </summary>
        private static ILogger _log;

        /// <summary>
        ///     The current configuration for tests
        /// </summary>
        protected IConfiguration _configuration;

        /// <summary>
        ///     Default constructor
        /// </summary>
        protected TestBase()
        {
            LoadConfiguration();
            ConfigureLogging();
        }

        /// <summary>
        ///     Configures the logging for tests, based on the current test configuration
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
        ///     Stands up the <see cref="IConfiguration" /> instance to be used during testing
        /// </summary>
        private void LoadConfiguration()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Test.json", true, true)
                .AddEnvironmentVariables()
                .Build();
        }

        /// <summary>
        ///     Just loads a test certificate for use during tests
        /// </summary>
        /// <returns></returns>
        protected X509Certificate2 LoadTestCertificate(string passphrase = "test")
        {
            var sshOption = Files.GetHomeSubdirectoryPath(".config", "neon", "glow", "test.pfx");
            var result = sshOption.Fold(path =>
            {
                var cert = X509Certificates.ImportFromFile(path, () => passphrase);
                return cert;
            }, () => new X509Certificate2());
            return result;
        }
    }
}