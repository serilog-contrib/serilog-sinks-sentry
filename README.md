# Serilog.Sinks.Sentry

## Note

> This is an unofficial library based on [the old Raven SDK](https://github.com/getsentry/raven-csharp/), the only use case for this is legacy projects (.NET Framework 4.5 to 4.6.0)  
> For .NET Framework 4.6.1, .NET Core 2.0, Mono 5.4 or higher, [**please use the new SDK**](https://github.com/getsentry/sentry-dotnet) and [**the Serilog sink**](https://www.nuget.org/packages/Sentry.Serilog).

[![Build status](https://ci.appveyor.com/api/projects/status/3rtn2dsk5ln6qaup?svg=true)](https://ci.appveyor.com/project/olsh/serilog-sinks-sentry)
[![Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=serilog-sinks-sentry&metric=alert_status)](https://sonarcloud.io/dashboard?id=serilog-sinks-sentry)

|   | Package |
| ------------- | ------------- |
| Serilog.Sinks.Sentry  | [![NuGet](https://img.shields.io/nuget/v/Serilog.Sinks.Sentry.svg)](https://www.nuget.org/packages/Serilog.Sinks.Sentry/)  |
| Serilog.Sinks.Sentry.AspNetCore  | [![NuGet](https://img.shields.io/nuget/v/Serilog.Sinks.Sentry.AspNetCore.svg)](https://www.nuget.org/packages/Serilog.Sinks.Sentry.AspNetCore/)  |

A Sentry sink for Serilog.

## Installation

The library is available as a [Nuget package](https://www.nuget.org/packages/Serilog.Sinks.Sentry/).
```
Install-Package Serilog.Sinks.Sentry
```

## Demos

You can find demo .NET Core apps [here](demos/).

## Get started

### Adding Sentry sink

```csharp
var log = new LoggerConfiguration()
    .WriteTo.Sentry("Sentry DSN")
    .Enrich.FromLogContext()
    .CreateLogger();

// By default, only messages with level errors and higher are captured
log.Error("This error goes to Sentry.");
```

### Data scrubbing

Some of the logged content might contain sensitive data and should therefore not be sent to Sentry. When setting up the Sentry Sink it is possible to provide a custom `IScrubber` implementation which will be passed the serialized data that is about to be sent to Sentry for scrubbing / cleaning.

Adding a scrubber would look like this:

```csharp
var log = new LoggerConfiguration()
    .WriteTo.Sentry("Sentry DSN", dataScrubber: new MyDataScrubber())
    .Enrich.FromLogContext()
    .CreateLogger();
```

`MyDataScrubber` has to implement the interface `SharpRaven.Logging.IScrubber`. Check the [Web Demo Startup.cs for further details](demos/SentryWeb/Startup.cs) and the [example implementation of a scrubber](demos/SentryWeb/Scrubbing/CustomLogScrubber.cs) .

### Capturing HttpContext (ASP.NET Core)

In order to capture a user, request body and headers, some additional steps are required.

Install the [additional sink](https://www.nuget.org/packages/Serilog.Sinks.Sentry.AspNetCore/) for ASP.NET Core
```
Install-Package Serilog.Sinks.Sentry.AspNetCore
```

Specify custom HttpContext destructing policy
```csharp
var log = new LoggerConfiguration()
    .WriteTo.Sentry("Sentry DSN")
    .Enrich.FromLogContext()

    // Add this two lines to the logger configuration
    .Destructure.With<HttpContextDestructingPolicy>()
    .Filter.ByExcluding(e => e.Exception?.CheckIfCaptured() == true)

    .CreateLogger();
```

Add Sentry context middleware in Startup.cs
````csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    // Add this line
    app.AddSentryContext();

    // Other stuff
}
````
