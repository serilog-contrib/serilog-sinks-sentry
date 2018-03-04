using SharpRaven.Data;

namespace Serilog
{
    internal class SentryUserFactory : ISentryUserFactory
    {
        private readonly ISentryHttpContext _httpContext;

        public SentryUserFactory(ISentryHttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        public SentryUser Create()
        {
            var sentryUser = new SentryUser(_httpContext.User) { IpAddress = _httpContext.RemoteIpAddress };

            return sentryUser;
        }
    }
}
