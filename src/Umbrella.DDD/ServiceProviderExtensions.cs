using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Umbrella.DDD.Abstractions;
using Umbrella.DDD.Configuration;

namespace Umbrella.DDD
{
    /// <summary>
    /// Extensions to manage publisher Instance
    /// </summary>
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Redas the configuration and register proper messages on target queue inside IEventPublisher
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="config"></param>
        public static void UseQueuesToPublishMessages(this IServiceProvider provider, IConfiguration config)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            // read messagebus configuration
            UmbrellaMessageBusSettings messageBusSettings = new UmbrellaMessageBusSettings();
            config.GetSection("UmbrellaMessageBus").Bind(messageBusSettings);
            if (messageBusSettings.IsInMemory)
                return;

            // registers messages into proper queue
            var publisher = provider.GetRequiredService<IEventPublisher>();
            foreach (var queue in messageBusSettings.Queues)
            {
                publisher.UsingThisQueueFor(queue.EventType, queue.QueueName);
            }
        }
    }
}