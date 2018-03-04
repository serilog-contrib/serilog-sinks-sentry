using System.Collections.Generic;
using System.Security.Principal;

namespace Serilog
{
    public interface ISentryHttpContext
    {
        string RemoteIpAddress { get; }

        IDictionary<string, string> RequestCookies { get; }

        IDictionary<string, string> RequestHeaders { get; }

        string RequestMethod { get; }

        string RequestPath { get; }

        string RequestQueryString { get; }

        IPrincipal User { get; }

        object GetRequestBody();
    }
}
