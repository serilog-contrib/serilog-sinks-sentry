using Microsoft.AspNetCore.Http;
using SharpRaven.Data;

namespace Serilog.Sinks.Sentry
{
    internal class AspNetCoreSentryUserFactory : ISentryUserFactory
    {
        private readonly HttpContext _httpContext;

        public AspNetCoreSentryUserFactory(HttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        public SentryUser Create()
        {
            var sentryUser =
                new SentryUser(_httpContext.User) { IpAddress = _httpContext.Connection.RemoteIpAddress.ToString() };

            return sentryUser;
        }
    }
}
