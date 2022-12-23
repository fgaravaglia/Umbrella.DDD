using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Umbrella.DDD.Abstractions;

namespace Umbrella.DDD.GCP
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the In memory implementation of Event Publisher
        /// </summary>
        /// <param name="services"></param>
        /// <param name="projectID"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddPubSubEventPublisher(this IServiceCollection services, string projectID)
        { 
            if(services == null)
                throw new ArgumentNullException(nameof(services));
            if (String.IsNullOrEmpty(projectID))
                throw new ArgumentNullException(nameof(projectID));

            services.AddSingleton<IEventPublisher>(x =>
            {
                var logger = x.GetRequiredService<ILogger>();
                return new EventPublisher(logger, projectID);
            });
        }

    }
}
