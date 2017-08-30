using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;
using SharpRaven;
using SharpRaven.Data;

namespace Serilog.Sinks.Sentry
{
    /// <inheritdoc />
    public class SentrySink : ILogEventSink
    {
        private readonly string _dsn;
        private readonly string _environment;
        private readonly IFormatProvider _formatProvider;
        private readonly string _release;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SentrySink" /> class.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="dsn">The DSN.</param>
        /// <param name="release">The release.</param>
        /// <param name="environment">The environment.</param>
        public SentrySink(IFormatProvider formatProvider, string dsn, string release, string environment)
        {
            _formatProvider = formatProvider;
            _dsn = dsn;
            _release = release;
            _environment = environment;
        }

        /// <inheritdoc />
        public void Emit(LogEvent logEvent)
        {
            var sentryEvent = new SentryEvent(logEvent.Exception)
            {
                Level = GetSentryLevel(logEvent),
                Message = logEvent.RenderMessage(_formatProvider),
                Extra = logEvent.Properties.ToDictionary(pair => pair.Key, pair => pair.Value?.ToString())
            };

            IRavenClient ravenClient;
            if (logEvent.Properties.TryGetValue(Constants.HttpContextKey, out var logEventPropertyValue) &&
                logEventPropertyValue is ScalarValue scalarValue && scalarValue.Value is HttpContext httpContext)
            {
                ravenClient = new RavenClient(
                                  _dsn,
                                  sentryRequestFactory: new AspNetCoreSentryRequestFactory(httpContext),
                                  sentryUserFactory: new AspNetCoreSentryUserFactory(httpContext))
                {
                    Release = _release,
                    Environment = _environment
                };
            }
            else
            {
                ravenClient = new RavenClient(_dsn) { Release = _release, Environment = _environment };
            }

            ravenClient.Capture(sentryEvent);
        }

        private static ErrorLevel GetSentryLevel(LogEvent logEvent)
        {
            switch (logEvent.Level)
            {
                case LogEventLevel.Verbose:
                case LogEventLevel.Debug:
                    return ErrorLevel.Debug;
                case LogEventLevel.Information:
                    return ErrorLevel.Info;
                case LogEventLevel.Warning:
                    return ErrorLevel.Warning;
                case LogEventLevel.Error:
                    return ErrorLevel.Error;
                case LogEventLevel.Fatal:
                    return ErrorLevel.Fatal;
                default:
                    return ErrorLevel.Error;
            }
        }
    }
}
