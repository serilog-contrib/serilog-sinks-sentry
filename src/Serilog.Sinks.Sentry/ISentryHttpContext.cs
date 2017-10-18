using System.Collections.Generic;
using System.Security.Principal;

namespace Serilog.Sinks.Sentry
{
    public interface ISentryHttpContext
    {
        string RemoteIpAddress { get; }

        object GetRequestBody();

        IDictionary<string, string> RequestCookies { get; }

        IDictionary<string, string> RequestHeaders { get; }

        string RequestMethod { get; }

        string RequestPath { get; }

        string RequestQueryString { get; }

        IPrincipal User { get; }
    }
}
