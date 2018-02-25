using System;
using Serilog.Configuration;
using Serilog.Events;

namespace Serilog.Sinks.Sentry
{
    /// <summary>
    ///     Contains extension methods for Serilog configuration.
    /// </summary>
    public static class SentrySinkExtensions
    {
        /// <summary>
        ///     Add Sentry sink to the logger configuration.
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="dsn">The DSN.</param>
        /// <param name="release">The release.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="restrictedToMinimumLevel">The restricted to minimum level.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="tags">Comma separated list of properties to treat as tags in sentry.</param>
        /// <returns>The logger configuration.</returns>
        // ReSharper disable once StyleCop.SA1625
        public static LoggerConfiguration Sentry(
            this LoggerSinkConfiguration loggerConfiguration,
            string dsn,
            string release = null,
            string environment = null,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Error,
            IFormatProvider formatProvider = null,
            string tags = null)
        {
            return loggerConfiguration.Sink(
                new SentrySink(formatProvider, dsn, release, environment, tags),
                restrictedToMinimumLevel);
        }
    }
}
