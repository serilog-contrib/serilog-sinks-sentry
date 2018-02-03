using System;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Sentry;
using SharpRaven;
using SharpRaven.Data;

namespace SentryConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // Insert Sentry DSN here
            var sentryDSN = "";

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.Sentry(sentryDSN)
                .Enrich.FromLogContext()
                // Serilog.Exceptions
                .Enrich.WithExceptionDetails()
                .CreateLogger();

            // Explicitly call our error logger
            Log.Error("Intentional error logged at {TimeStamp}", DateTime.Now.ToLongTimeString());

            // Serilog will not catch uncaught exceptions for console applications, they must be caught explicitly

            // This can be done with Serilog.Exceptions...
            try
            {
                ConvertToIntSecondTier();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }

            // ...or with the ravenClient itself
            var ravenClient = new RavenClient(sentryDSN);
            try
            {
                DivByZeroSecondTier();
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
            }

            Log.CloseAndFlush();

            Console.WriteLine("Finished");
        }

        static int DivByZeroSecondTier()
        {
            var i = DivByZero();
            return i;
        }

        static int DivByZero()
        {
            var i = 0;
            var j = 1 / i;
            return j;
        }

        static int ConvertToIntSecondTier()
        {
            var i = ConvertToInt();
            return i;
        }

        static int ConvertToInt()
        {
            var s = "hello world";
            return Convert.ToInt32(s);
        }
    }
}
