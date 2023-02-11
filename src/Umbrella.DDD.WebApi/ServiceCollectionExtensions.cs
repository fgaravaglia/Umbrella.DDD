using System;
using System.Diagnostics;
using Serilog;
using Serilog.Configuration;
using Umbrella.DDD.WebApi.Infrastructure;
using Umbrella.DDD.WebApi.TestScenarios;
using Umbrella.DDD;
using Microsoft.Extensions.Configuration;

namespace Umbrella.DDD.WebApi
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServices(this IServiceCollection services, IConfiguration config, Serilog.ILogger bootstrapLogger)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            if (bootstrapLogger == null)
                throw new ArgumentNullException(nameof(bootstrapLogger));

            bootstrapLogger.Information("Services Registration started...");
            // Add services to the container.
            string environmentName = config.GetSection("Environment:DisplayName").Value;
            bootstrapLogger.Information("Target Environment: " + environmentName);

            // set up Serilog as logger
            Log.Logger = services.AddLogging(config, environmentName);
            //builder.Host.UseSerilog();

            // setup message bus
            bootstrapLogger.Information("setting Message Bus from {currentFolder}", Environment.CurrentDirectory);
            // setup the publisher
            string publisher = config.GetSection("UmbrellaMessageBus:Publisher").Value.ToString();
            if (publisher.Equals("PubSub", StringComparison.InvariantCultureIgnoreCase))
                throw new NotImplementedException();
            else
                services.AddInMemoryEventPublisher();
            // and set specific domain event handlers
            var messageBusDependencyResolver = new MessageBusDependencyResolver();
            messageBusDependencyResolver.AddEventHandlers(services);
            messageBusDependencyResolver.AddSagas(services);
            // add the message bus
            services.AddAddMessageBus();
            bootstrapLogger.Information("Message Bus settings completed....");

            // register controlelrs into DI
            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            bootstrapLogger.Information("Services Registration Completed!");
        }
    }
}