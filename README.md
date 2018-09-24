# Serilog.Sinks.Sentry

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
