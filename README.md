# Repository Content
Library to implement a light-weight Framework for Domain Driven Design

[![Build Status](https://garaproject.visualstudio.com/UmbrellaFramework/_apis/build/status/Umbrella.Infrastructure.Cache?branchName=main)](https://garaproject.visualstudio.com/UmbrellaFramework/_build/latest?definitionId=76&branchName=main)


To install it, use proper command:

```bat
dotnet add package Umbrella.DDD
dotnet add package Umbrella.DDD.GCP
```

[![Nuget](https://img.shields.io/nuget/v/Umbrella.Infrastructure.Cache.svg?style=plastic)](https://www.nuget.org/packages/Umbrella.DDD/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Umbrella.Infrastructure.Cache.svg)](https://www.nuget.org/packages/Umbrella.DDD/)

For more details about download, see [NuGet Web Site](https://www.nuget.org/packages/Umbrella.DDD/)

# Configuration
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

# Usage
for the usage of the library, please refer to extension methods.

<b>Standard usage</b>

```c#

services.AddInMemoryEventPublisher();
services.AddEventHanders(Environment.CurrentDirectory, "MyTestAssembly.dll");
services.AddMessageBus();

```

Based on the snippet, some important notice:
- _AddInMemoryEventPublisher_: it should be avoided for PROD environments
- _AddEventHanders_: it scans the assembly to inject dynamically IMEssageHandler component 
- instruction _AddMessageBus_ should be states after all DI injections

<b>Publishing message on Google Pub Sub</b>

```c#

services.AddPubSubEventPublisher("umbrella-proj-id");
services.AddEventHanders(Environment.CurrentDirectory, "MyTestAssembly.dll");
services.AddMessageBus();

```

Using PubSub publisher, you are decoupling the publish action to handler action.
the message will be handled asyncronously.
