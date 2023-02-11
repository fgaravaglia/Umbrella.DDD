using Serilog.Extensions.Hosting;
using Serilog;
using System;
using Microsoft.Extensions.Hosting;
using Serilog.Exceptions;
using Serilog.Enrichers.Span;

namespace Umbrella.DDD.WebApi.Infrastructure
{
    internal static class SerilogExtensions
    {
        /// <summary>
        /// Enable logging capabilities based on Serilog. 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config">IConfiguration provided by Application builder to reduce memoery consumption and avoid memory leak</param>
        /// <param name="environment"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ApplicationException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        internal static Serilog.ILogger AddLogging(this IServiceCollection services, IConfiguration config, string environment)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            if (String.IsNullOrEmpty(environment))
                throw new ArgumentNullException(nameof(environment));

            // initialize the settings from appSettings file, than customize it programamtically
            var serilogConfig = new LoggerConfiguration().ReadFrom.Configuration(config)
                            .WithDefaultEnrichments()
                            .WithDestructureMaximumLimits()
                            .WithDefaultProperties(environment);
        
            if (serilogConfig == null)
                throw new ApplicationException("Unicredit Logger settings creation failed");

            // create here the logger: if you create it into transient statement, it will be invoked more than once
            var serilog = serilogConfig.CreateLogger();
            // instance also MSFT logger factory, to use also the generic interface provided by MSFT
            services.AddSingleton<ILoggerFactory>(x =>
            {
                // inject MSFT component using Serilog
                var factory = LoggerFactory.Create(c => c.AddSerilog(serilog));
                // inject log provider if are there
                var providers = x.GetServices<ILoggerProvider>();
                if (providers != null && providers.Any())
                    providers.ToList().ForEach(p => factory.AddProvider(p));
                // return the singleton
                return factory;
            });

            //add the serilog factory
            services.AddTransient<Serilog.ILogger>(x =>
            {
                return serilogConfig.CreateLogger();
            });
            // instance also MSFT logger
            services.AddTransient<Microsoft.Extensions.Logging.ILogger>(x =>
            {
                var msftLogFactory = x.GetService<ILoggerFactory>();
                if (msftLogFactory is null)
                    throw new InvalidOperationException($"DI Exception: Unable to resolve Microsoft.Extensions.Logging.ILoggerFactory to configure Logging capability");
                return msftLogFactory.CreateLogger("Umbrella.MyHealth.Headache.WebApi");
            });

            // inject Diagnostic Context to log requests out of the box (used by Serilog.Extension.Hosting package)
            services.AddSingleton(x => new DiagnosticContext(serilog));
            // Consumed by user code by its interface (best practice)
            services.AddSingleton<IDiagnosticContext>(x => x.GetRequiredService<DiagnosticContext>());

            return serilog;
        }

        #region Private methods

        /// <summary>
        /// Creates the default configuration for Logging
        /// </summary>
        /// <remarks>
        /// the Umbrella Default includes:
        /// - enrichments
        /// - destructure limits
        /// - custom properties
        /// </remarks>
        /// <param name="config"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        static LoggerConfiguration CreateConfiguration(this IConfiguration config, string environment)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            if(String.IsNullOrEmpty(environment))
                throw new ArgumentNullException(nameof(environment));

            // initialize the settings from appSettings file, than customize it programamtically
            return new LoggerConfiguration().ReadFrom.Configuration(config)
                            .WithDefaultEnrichments()
                            .WithDestructureMaximumLimits()
                            .WithDefaultProperties(environment);
        }
        /// <summary>
        /// Sets the Enrichments
        /// </summary>
        /// <param name="serilogConfig"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static LoggerConfiguration WithDefaultEnrichments(this LoggerConfiguration serilogConfig)
        {
            if (serilogConfig == null)
                throw new ArgumentNullException(nameof(serilogConfig));

            // add feature programamtically - Enrich
            serilogConfig.Enrich.WithSpan(new SpanOptions { IncludeBaggage = true });
            serilogConfig.Enrich.WithEnvironmentName();         // added by Serilog.Enrichers.Context
            serilogConfig.Enrich.WithEnvironmentUserName();     // added by Serilog.Enrichers.Environment
            serilogConfig = EnvironmentLoggerConfigurationExtensions.WithMachineName(serilogConfig.Enrich);
            serilogConfig.Enrich.WithThreadId()                 // added by Serilog.Enrichers.Thread
                         .Enrich.WithClientIp()                 // added by Serilog.Enrichers.ClientInfo
                         .Enrich.WithClientAgent()              // added by Serilog.Enrichers.ClientInfo
                         .Enrich.WithExceptionDetails()         // added by Serilog.Exceptions
                         .Enrich.FromLogContext();

            return serilogConfig;
        }
        /// <summary>
        /// Sets UniCredit Destructure limits
        /// </summary>
        /// <param name="serilogConfig"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        static LoggerConfiguration WithDestructureMaximumLimits(this LoggerConfiguration serilogConfig)
        {
            if (serilogConfig == null)
                throw new ArgumentNullException(nameof(serilogConfig));

            return serilogConfig.Destructure.ToMaximumStringLength(100)
                                .Destructure.ToMaximumDepth(4)
                                .Destructure.ToMaximumCollectionCount(10);
        }
        /// <summary>
        /// Sets Defualt logging properties
        /// </summary>
        /// <param name="serilogConfig"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        static LoggerConfiguration WithDefaultProperties(this LoggerConfiguration serilogConfig, string environment)
        {
            if (serilogConfig == null)
                throw new ArgumentNullException(nameof(serilogConfig));
            if (String.IsNullOrEmpty(environment))
                throw new ArgumentNullException(nameof(environment));

            return serilogConfig.Enrich.WithProperty("LogType", "App")
                         .Enrich.WithProperty("Application", "Umbrella.DDD.WebApi")
                         .Enrich.WithProperty("EnvironmentDisplayName", environment);

        }

        #endregion
    }
}
