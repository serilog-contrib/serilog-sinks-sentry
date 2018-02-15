using System;
using System.Collections.Generic;
using System.Linq;

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
        private readonly IEnumerable<string> _tags = new string[0];

        /// <summary>
        ///     Initializes a new instance of the <see cref="SentrySink" /> class.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="dsn">The DSN.</param>
        /// <param name="release">The release.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="tags">Comma separated list of properties to treat as tags in sentry.</param>
        public SentrySink(IFormatProvider formatProvider, string dsn, string release, string environment, string tags)
        {
            _formatProvider = formatProvider;
            _dsn = dsn;
            _release = release;
            _environment = environment;
            if (!string.IsNullOrWhiteSpace(tags))
            {
                _tags = tags.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToArray();
            }
        }

        /// <inheritdoc />
        public void Emit(LogEvent logEvent)
        {
            var sentryEvent = new SentryEvent(logEvent.Exception)
            {
                Level = GetSentryLevel(logEvent),
                Message = logEvent.RenderMessage(_formatProvider),
                Extra = logEvent.Properties.Where(pair => _tags.All(t => t != pair.Key))
                    .ToDictionary(pair => pair.Key, pair => Render(pair.Value, _formatProvider)),
                Tags = logEvent.Properties.Where(pair => _tags.Any(t => t == pair.Key))
                    .ToDictionary(pair => pair.Key, pair => Render(pair.Value, _formatProvider))
            };

            IRavenClient ravenClient;
            if (logEvent.Properties.TryGetValue(SentrySinkConstants.HttpContextKey, out var logEventPropertyValue) &&
                logEventPropertyValue is ScalarValue scalarValue && scalarValue.Value is ISentryHttpContext httpContext)
            {
                ravenClient = new RavenClient(
                                  _dsn,
                                  sentryRequestFactory: new SentryRequestFactory(httpContext),
                                  sentryUserFactory: new SentryUserFactory(httpContext))
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

        private static string Render(LogEventPropertyValue logEventPropertyValue, IFormatProvider formatProvider)
        {
            if (logEventPropertyValue is ScalarValue scalarValue && scalarValue.Value is string)
            {
                // Remove quotes from the value
                return logEventPropertyValue.ToString("l", formatProvider);
            }

            return logEventPropertyValue?.ToString(null, formatProvider);
        }
    }
}
