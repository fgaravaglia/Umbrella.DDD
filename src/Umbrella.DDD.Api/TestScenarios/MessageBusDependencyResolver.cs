using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.DDD;
using Umbrella.DDD.Abstractions;
using Umbrella.DDD.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Umbrella.DDD.Api.TestScenarios
{
    public class MessageBusDependencyResolver : IMessageBusDependencyResolver
    {
        /// <summary>
        /// Default COnstr
        /// </summary>
        public MessageBusDependencyResolver()
        {

        }
        /// <summary>
        /// Adds domains ervices
        /// </summary>
        /// <param name="services"></param>
        public IServiceCollection AddEventHandlers(IServiceCollection services)
        {
            services.AddScoped<IMessageHandler>(x =>
            {
                var logger = x.GetRequiredService<ILogger>();
                return new TestEventOccurredHandler(logger);
            });
            return services;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public IServiceCollection AddSagas(IServiceCollection services)
        {
            return services;
        }
    }
}