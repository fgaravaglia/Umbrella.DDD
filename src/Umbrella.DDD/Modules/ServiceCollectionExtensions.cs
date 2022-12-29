using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Umbrella.DDD.Modules
{
    /// <summary>
    /// Extensions to set up component at applications tartup
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Uses the proivder to get application modules and register all services for DI
        /// </summary>
        /// <param name="services"></param>
        /// <param name="provider"></param>
        public static void AddApplicationModules(this IServiceCollection services, IApplicationModuleProvider provider)
        {
            if (provider is null)
                throw new ArgumentNullException(nameof(provider));

            foreach (var module in provider.GetModules())
            {
                module.AddServices(services);
            }
        }
        /// <summary>
        /// Uses the provider to get application modules and register all services for DI
        /// </summary>
        /// <param name="services"></param>
        /// <param name="providerFactory">lamda function to instance the provider of type <see href="IApplicationModuleProvider" ></see></param>
        public static void AddApplicationModules(this IServiceCollection services, Func<IServiceCollection, IApplicationModuleProvider> providerFactory)
        {
            if (providerFactory is null)
                throw new ArgumentNullException(nameof(providerFactory));

            providerFactory.Invoke(services).GetModules().ToList().ForEach(m => m.AddServices(services));
        }

    }
}
