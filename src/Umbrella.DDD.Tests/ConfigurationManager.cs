using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Umbrella.DDD.Tests
{
    internal class ConfigurationManager
    {
        /// <summary>
        /// Initializes the configuration from the given appsettings.json file
        /// </summary>
        /// <param name="appSettingsFile"></param>
        /// <returns></returns>
        public IConfiguration InitConfigurationFromFile(string appSettingsFile = "appsettings.test.json")
        {
            if (String.IsNullOrEmpty(appSettingsFile))
                throw new ArgumentNullException(nameof(appSettingsFile));

            var config = new ConfigurationBuilder()
                                .AddJsonFile(appSettingsFile)
                                .AddEnvironmentVariables()
                                .Build();
            return config;
        }
    }
}