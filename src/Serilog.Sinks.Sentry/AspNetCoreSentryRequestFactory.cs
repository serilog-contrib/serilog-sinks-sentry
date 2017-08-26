using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using SharpRaven.Data;
using SharpRaven.Utilities;

namespace Serilog.Sinks.Sentry
{
    internal class AspNetCoreSentryRequestFactory : ISentryRequestFactory
    {
        public AspNetCoreSentryRequestFactory(HttpContext httpContext)
        {
            HttpContext = httpContext;
        }

        private HttpContext HttpContext { get; }

        public ISentryRequest Create()
        {
            var request = new SentryRequest
            {
                Url = HttpContext.Request.Path.ToString(),
                Method = HttpContext.Request.Method,
                Environment = new Dictionary<string, string>(),
                Headers = Convert(HttpContext.Request.Headers),
                Cookies = HttpContext.Request.Cookies.ToDictionary(
                    x => x.Key,
                    x => x.Value),
                Data = BodyConvert(),
                QueryString = HttpContext.Request.QueryString.ToString()
            };

            return request;
        }

        private static IDictionary<string, string> Convert(IEnumerable<KeyValuePair<string, StringValues>> collection)
        {
            return collection.ToDictionary(x => x.Key, x => string.Join(" ", x.Value.ToArray()));
        }

        private object BodyConvert()
        {
            try
            {
                dynamic wrapper = new ExpandoObject();
                wrapper.Request = new DynamicHttpContextRequest(HttpContext);
                return HttpRequestBodyConverter.Convert(wrapper);
            }
            catch (Exception exception)
            {
                SystemUtil.WriteError(exception);
            }

            return null;
        }

        public class DynamicHttpContextRequest
        {
            private readonly HttpContext _context;

            private readonly MemoryStream _stream;

            public DynamicHttpContextRequest(HttpContext context)
            {
                _context = context;

                if (_context.Request.Body == null)
                {
                    return;
                }

                _context.Request.Body.Position = 0;
                using (var bodyReader = new StreamReader(_context.Request.Body))
                {
                    var body = bodyReader.ReadToEnd();

                    _stream = new MemoryStream(Encoding.UTF8.GetBytes(body));
                }
            }

            public string ContentType => _context.Request.ContentType;

            public NameValueCollection Form
            {
                get
                {
                    var nameValueCollection = new NameValueCollection();
                    foreach (var pair in _context.Request.Form)
                    {
                        nameValueCollection.Add(pair.Key, pair.Value);
                    }

                    return nameValueCollection;
                }
            }

            public Stream InputStream => _stream;
        }
    }
}
