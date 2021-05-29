/*

    Copyright 2013-2021 © JCS Software Limited

    Author: Jonny Coombes

    Contact: jcoombes@jcs-software.co.uk

    All rights reserved.

 */
 #region

using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using JCS.Neon.Glow.Types;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using File = JCS.Neon.Glow.Statics.IO.File;
using X509Certificate = JCS.Neon.Glow.Statics.Crypto.X509Certificate;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

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
        public TestBase(ITestOutputHelper output)
        {
            LoadConfiguration();
            ConfigureLogging(output);
        }

        /// <summary>
        ///     Configures the logging for tests, based on the current test configuration
        /// </summary>
        private void ConfigureLogging(ITestOutputHelper output)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.WithMachineName()
                .WriteTo.TestOutput(output)
                .CreateLogger();
            _log = Log.ForContext<TestBase>();
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
        /// Sleeps (suspends) the current thread of execution for a given number of seconds
        /// </summary>
        /// <param name="count">The amount to sleep the current thread in seconds.  Should be > 0</param>
        protected static void SleepCurrentThread(int count)
        {
            if (count > 0)
            {
                Thread.Sleep(count * 1000);
            }
        }
    }
}