using System;

using Serilog.Configuration;
using Serilog.Events;

using SharpRaven.Data;
using SharpRaven.Logging;

namespace Serilog
{
    /// <summary>
    ///     Contains extension methods for Serilog configuration.
    /// </summary>
    public static class SentrySinkExtensions
    {
        /// <summary>
        /// Add Sentry sink to the logger configuration.
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="dsn">The DSN.</param>
        /// <param name="release">The release.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="restrictedToMinimumLevel">The restricted to minimum level.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="tags">Comma separated list of properties to treat as tags in sentry.</param>
        /// <param name="jsonPacketFactory">The json packet factory.</param>
        /// <param name="sentryUserFactory">The sentry user factory.</param>
        /// <param name="sentryRequestFactory">The sentry request factory.</param>
        /// <param name="dataScrubber">An <see cref="IScrubber"/> implementation for cleaning up logs before sending to Sentry</param>
        /// <param name="logger">The name of the logger used by Sentry.</param>
        /// <returns>
        /// The logger configuration.
        /// </returns>
        // ReSharper disable once StyleCop.SA1625
        public static LoggerConfiguration Sentry(
            this LoggerSinkConfiguration loggerConfiguration,
            string dsn,
            string release = null,
            string environment = null,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Error,
            IFormatProvider formatProvider = null,
            string tags = null,
            IJsonPacketFactory jsonPacketFactory = null,
            ISentryUserFactory sentryUserFactory = null,
            ISentryRequestFactory sentryRequestFactory = null,
            IScrubber dataScrubber = null,
            string logger = null)
        {
            return loggerConfiguration.Sink(
                new SentrySink(
                    formatProvider,
                    dsn,
                    release,
                    environment,
                    tags,
                    jsonPacketFactory,
                    sentryUserFactory,
                    sentryRequestFactory,
                    dataScrubber,
                    logger),
                restrictedToMinimumLevel);
        }
    }
}
