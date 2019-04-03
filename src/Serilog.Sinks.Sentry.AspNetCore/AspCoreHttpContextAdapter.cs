using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;

using Microsoft.AspNetCore.Http;

using SharpRaven.Data;
using SharpRaven.Utilities;

namespace Serilog
{
    public class AspCoreHttpContextAdapter : ISentryHttpContext
    {
        private readonly HttpContext _httpContext;

        public AspCoreHttpContextAdapter(HttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        // ReSharper disable once ExceptionNotDocumented
        public string RemoteIpAddress => _httpContext.Connection.RemoteIpAddress.ToString();

        public IDictionary<string, string> RequestCookies =>
            _httpContext.Request.Cookies.ToDictionary(x => x.Key, x => x.Value);

        public IDictionary<string, string> RequestHeaders => _httpContext.Request.Headers.Where(h => h.Value.Any())
            .ToDictionary(x => x.Key, x => string.Join(" ", x.Value.ToArray()));

        public string RequestMethod => _httpContext.Request.Method;

        public string RequestPath => _httpContext.Request.Path;

        public string RequestQueryString => _httpContext.Request.QueryString.ToString();

        public IPrincipal User => _httpContext.User;

        public object GetRequestBody()
        {
            try
            {
                dynamic wrapper = new ExpandoObject();
                wrapper.Request = new DynamicHttpContextRequest(_httpContext);
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

                if (_context.Request.Body == null || !_context.Request.Body.CanSeek)
                {
                    return;
                }

                // ReSharper disable once ExceptionNotDocumented
                _context.Request.Body.Position = 0;
                using (var bodyReader = new StreamReader(_context.Request.Body, Encoding.UTF8, false, 1024, true))
                {
                    // ReSharper disable once ExceptionNotDocumented
                    var body = bodyReader.ReadToEnd();

                    _stream = new MemoryStream(Encoding.UTF8.GetBytes(body));
                }

                // Reset the request body stream position so the next middleware can read it
                context.Request.Body.Position = 0;
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
