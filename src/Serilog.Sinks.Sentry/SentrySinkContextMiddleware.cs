using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Serilog.Sinks.Sentry
{
    internal class SentrySinkContextMiddleware
    {
        private readonly RequestDelegate _next;

        public SentrySinkContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            using (LogContext.PushProperty(Constants.HttpContextKey, context, true))
            {
                try
                {
                    await _next(context)
                        .ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Log.Logger.Error(e, $"Connection id \"\"{context.TraceIdentifier}\"\": An unhandled exception was thrown by the application.");
                    e.SetCaptured();

                    throw;
                }
            }
        }
    }
}
