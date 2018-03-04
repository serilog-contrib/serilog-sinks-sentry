using System.Collections.Generic;

using SharpRaven.Data;

namespace Serilog
{
    public class SentryRequestFactory : ISentryRequestFactory
    {
        public SentryRequestFactory(ISentryHttpContext sentryHttpContext)
        {
            SentryHttpContext = sentryHttpContext;
        }

        private ISentryHttpContext SentryHttpContext { get; }

        public ISentryRequest Create()
        {
            var request = new SentryRequest
                              {
                                  Url = SentryHttpContext.RequestPath,
                                  Method = SentryHttpContext.RequestMethod,
                                  Environment = new Dictionary<string, string>(),
                                  Headers = SentryHttpContext.RequestHeaders,
                                  Cookies = SentryHttpContext.RequestCookies,
                                  Data = SentryHttpContext.GetRequestBody(),
                                  QueryString = SentryHttpContext.RequestQueryString
                              };

            return request;
        }
    }
}
