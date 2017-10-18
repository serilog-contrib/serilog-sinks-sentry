using Serilog.Core;
using Serilog.Events;

namespace Serilog.Sinks.Sentry.AspCore
{
    /// <inheritdoc />
    public class HttpContextDestructingPolicy : IDestructuringPolicy
    {
        /// <inheritdoc />
        public bool TryDestructure(
            object value,
            ILogEventPropertyValueFactory propertyValueFactory,
            out LogEventPropertyValue result)
        {
            if (value is ISentryHttpContext)
            {
                result = new ScalarValue(value);

                return true;
            }

            result = null;

            return false;
        }
    }
}
