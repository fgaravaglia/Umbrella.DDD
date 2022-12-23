using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.DDD.Abstractions;
using Umbrella.DDD.Startup;

namespace Umbrella.DDD.Tests.TestClasses
{
    internal class DependencyResolver : IDependencyResolver
    {
        public void AddEventHandlers(IServiceCollection services)
        {
            services.AddTransient<IMessageHandler<TestMessage>, TestMessageHandler>();
            services.AddTransient<IMessageHandler<TestMessage>, NewTestMessageHandler>();
        }

        public void AddSagas(IServiceCollection services)
        {

        }
    }
}
