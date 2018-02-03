# Sentry Serilog Demos
This project contains two demo projects that demonstrate how to use `Serilog` and Sentry together with dotnet core. The SentryConsole project demonstrates how to set up a basic console application with Serilog and Sentry, while the SentryWeb project demonstrates a basic MVC web app.

Currently `RavenSharp` targets .NET 4.6, but can be built against .NET Standard. Until it does, all dependencies must be explicitly declared for NuGet, including the following packages:
* `Newtonsoft.Json`
* `Serilog`
* `Serilog.Sinks.Sentry`
* `Serilog.Sinks.AspNetCore`
* `SharpRaven`

Once all of these packages are set up, make sure to add `.WriteTo.Sentry("Sentry DSN")` as part of your Serilog LoggerConfigruation.

This would not be possible without the help of [olsh](https://github.com/olsh) and his project, [serilog-sinks-sentry](https://github.com/olsh/serilog-sinks-sentry)