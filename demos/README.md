# Sentry Serilog Demos
This project contains two demo projects that demonstrate how to use `Serilog` and Sentry together with dotnet core. The SentryConsole project demonstrates how to set up a basic console application with Serilog and Sentry, while the SentryWeb project demonstrates a basic MVC web app.

Make sure to add `.WriteTo.Sentry("Sentry DSN")` as part of your Serilog LoggerConfigruation.
