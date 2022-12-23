﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Umbrella.DDD.Abstractions;
using Umbrella.DDD.Startup;

namespace Umbrella.DDD
{
    /// <summary>
    /// Shared extensions
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the In memory implementation of Event Publisher
        /// </summary>
        /// <param name="services"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddInMemoryEventPublisher(this IServiceCollection services)
        { 
            if(services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IEventPublisher>(x => new InMemoryPublisher());
        }

        /// <summary>
        /// Adds the message bus to DI
        /// </summary>
        /// <remarks>Since message bus requires the Service provider, please add it as latest services injected into DI</remarks>
        /// <param name="services"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddAddMessageBus(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IMessageBus>(x =>
            {
                var logger = x.GetRequiredService<ILogger>();
                var publisher = x.GetRequiredService<IEventPublisher>();
                return new MessageBus(logger, publisher, x);
            });
        }

        /// <summary>
        /// Scans the assembly and instances all classes derivating from <see cref="IDependencyResolver"/> to load <see cref="ISaga"/> stored in such assembly.
        /// </summary>
        /// <remarks>It assumes that the dependency resolver concrete class has a no-parameter constructor</remarks>
        /// <param name="services"></param>
        /// <param name="assemblyFolder"></param>
        /// <param name="assemblyName"></param>
        /// <param name="addMessagehandlers">true to inject IMessagehandlers</param>
        /// <param name="addLongRunningProcesses">true to inject sagas</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public static void AddEventHandlersAndLRP(this IServiceCollection services, string assemblyFolder, string assemblyName,
                                                bool addMessagehandlers = true, bool addLongRunningProcesses = true)

        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (string.IsNullOrEmpty(assemblyFolder))
                throw new ArgumentNullException(nameof(assemblyFolder));
            if (string.IsNullOrEmpty(assemblyName))
                throw new ArgumentNullException(nameof(assemblyName));

            // get assembly
            var assemblyPath = Path.Combine(assemblyFolder, assemblyName);
            var targetAssembly = Assembly.Load(assemblyPath);
            if (targetAssembly == null)
                throw new NullReferenceException($"Unable to load {assemblyName}");

            services.AddEventHandlersAndLRP(targetAssembly, addMessagehandlers: false, addLongRunningProcesses: true);
        }
        /// <summary>
        /// Scans the assembly and instances all classes derivating from <see cref="IDependencyResolver"/> to load <see cref="ISaga"/> and <see cref="IMessageHandler{T}"/>  stored in such assembly.
        /// </summary>
        /// <remarks>It assumes that the dependency resolver concrete class has a no-parameter constructor</remarks>
        /// <param name="services"></param>
        /// <param name="targetAssembly"></param>
        /// <param name="addMessagehandlers"></param>
        /// <param name="addLongRunningProcesses"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddEventHandlersAndLRP(this IServiceCollection services, Assembly targetAssembly, bool addMessagehandlers = true, bool addLongRunningProcesses = true)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (targetAssembly == null)
                throw new ArgumentNullException(nameof(targetAssembly));

            // extract all Ihandler<> 
            var resolvers = targetAssembly.GetTypes().Where(x =>
            {
                return x.GetInterfaces().Any(i => i.IsAssignableFrom(typeof(IMessageBusDependencyResolver)));
            }).ToList();
            // instance all types and fill services
            foreach (var resolverType in resolvers)
            {
                IMessageBusDependencyResolver res = (IMessageBusDependencyResolver)Activator.CreateInstance(resolverType);
                if(addMessagehandlers)
                    res.AddEventHandlers(services);
                if(addLongRunningProcesses)
                    res.AddSagas(services);
            }
        }
    }
}
