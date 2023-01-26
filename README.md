# Repository Content
Library to implement a light-weight Framework for Domain Driven Design

[![Build Status](https://garaproject.visualstudio.com/UmbrellaFramework/_apis/build/status/Umbrella.DDD?branchName=main)](https://garaproject.visualstudio.com/UmbrellaFramework/_build/latest?definitionId=80&branchName=main)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Umbrella.DDD&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Umbrella.DDD)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=Umbrella.DDD&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=Umbrella.DDD)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=Umbrella.DDD&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=Umbrella.DDD)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=Umbrella.DDD&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=Umbrella.DDD)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=Umbrella.DDD&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=Umbrella.DDD)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=Umbrella.DDD&metric=bugs)](https://sonarcloud.io/summary/new_code?id=Umbrella.DDD)


To install it, use proper command:

```bat
dotnet add package Umbrella.DDD
dotnet add package Umbrella.DDD.GCP
```

Umbrella.DDD:
[![Nuget](https://img.shields.io/nuget/v/Umbrella.DDD.svg?style=plastic)](https://www.nuget.org/packages/Umbrella.DDD/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Umbrella.DDD.svg)](https://www.nuget.org/packages/Umbrella.DDD/)

Umbrella.DDD.GCP:
[![Nuget](https://img.shields.io/nuget/v/Umbrella.DDD.GCP.svg?style=plastic)](https://www.nuget.org/packages/Umbrella.DDD.GCP/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Umbrella.DDD.GCP.svg)](https://www.nuget.org/packages/Umbrella.DDD.GCP/)


For more details about download, see [NuGet Web Site](https://www.nuget.org/packages/Umbrella.DDD/)

## Configuration

to configure properly the application, it's necessary to add the section to ppSettings.json file as the example below:

```json
{
 "UmbrellaMessageBus": {
    "PublisherName": "InMemory"
  },
}
```

where we have:

- PublisherName: name of the provider for the publisher component. Valid names: [InMemory,PubSub]

## Usage

for the usage of the library, please refer to extension methods.

<b>Standard usage</b>

```c#

services.AddInMemoryEventPublisher();
services.AddEventHanders(new MyMessageBusDependencyResolver());
services.AddMessageBus();

```

Based on the snippet, some important notice:
- _AddInMemoryEventPublisher_: it should be avoided for PROD environments
- _AddEventHanders_: it scans the assembly to inject dynamically IMessageHandler and ISaga components
- instruction _AddMessageBus_ should be states after all DI injections

<b>Publishing message on Google Pub Sub</b>

```c#

services.AddPubSubEventPublisher("umbrella-proj-id");
services.AddEventHanders(new MyMessageBusDependencyResolver());
services.AddMessageBus();

```

Using PubSub publisher, you are decoupling the publish action to handler action.
the message will be handled asyncronously.

## Application Modules
to simplify the aggregation of assemblies into MOdules (or generically speaking _Domains_), _IApplicationModuleProvider_ is provided.
implementig such interface per Domain, you can easily setup your dependencies across domain assemly, infrastructure, ecc.
Here you find a simple snippet:

```c#

services.AddApplicationModules(new MyApplicationModuleProvider())

```

## How to add an Event Handler

To implement a new event handler, you have to follow the snippet below:

```c#
   /// <summary>
    /// Handlers to manage episode creations
    /// </summary>
    internal class TestEventHandler : MessageHandler<TestEventOccurred>
    {
        /// <summary>
        /// Default COnstr
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="repo"></param>
        public TestEventHandler(ILogger logger) : base(logger)
        {
        }
        /// <summary>
        /// handels the msg
        /// </summary>
        /// <param name="message"></param>
        public override void HandleThisMessage(TestEventOccurred message)
        {
            this._Logger.LogInformation("Handling message {message}", message);

            . . . .

            this._Logger.LogInformation("Message succesfully handled");
        }
    }
```

then register the handler using a custom extension linke the one below:
```c#
        /// <summary>
        /// Adds domains ervices
        /// </summary>
        /// <param name="services"></param>
        public static void AddMyDomainEventHandlers(this IServiceCollection services)
        {
            services.AddScoped<IMessageHandler TestEventHandler>();
            // add here other domain event handlers
            // . . .
        }
```

Please notice that you handlers must to be registered aas IMessageHandler.
