using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Umbrella.DDD.Configuration
{
    /// <summary>
    /// /Extensions to manage message bus configuration
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Reads the settings
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static UmbrellaMessageBusSettings GetMessageBusSettings(this IConfiguration config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            var settings = new UmbrellaMessageBusSettings();
            config.GetSection("UmbrellaMessageBus").Bind(settings);
            return settings;
        }
    }
}
