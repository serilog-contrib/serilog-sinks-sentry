using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Sinks.Sentry
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
            if (value is HttpContext)
            {
                result = new ScalarValue(value);

                return true;
            }

            result = null;

            return false;
        }
    }
}
