using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.DDD.Abstractions;
using Umbrella.DDD.DependencyInjection;

namespace Umbrella.DDD.Tests.TestClasses
{
    internal class DependencyResolver : IMessageBusDependencyResolver
    {
        public IServiceCollection AddEventHandlers(IServiceCollection services)
        {
            services.AddTransient<IMessageHandler<TestMessage>>(x => new TestMessageHandler());
            services.AddTransient<IMessageHandler<TestMessage>>(x => new NewTestMessageHandler());
            return services;
        }

        public IServiceCollection AddSagas(IServiceCollection services)
        {
            services.AddTransient<ISaga, Saga1>();
            return services;
        }
    }
}
