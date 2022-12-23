using System;
using System.Collections.Generic;
using System.Text;

namespace Umbrella.DDD.Configuration
{
    /// <summary>
    /// MessageBus settings
    /// </summary>
    public class UmbrellaMessageBusSettings
    {
        /// <summary>
        /// Name of publisher to use
        /// </summary>
        public string PublisherName { get; set; }

        public UmbrellaMessageBusSettings()
        {
            this.PublisherName = "";
        }
    }
}
