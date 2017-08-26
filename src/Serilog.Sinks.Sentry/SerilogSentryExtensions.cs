using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Internal;

namespace Serilog.Sinks.Sentry
{
    /// <summary>
    ///     Contains extensions methods for an application.
    /// </summary>
    public static class SentrySinkContextMiddlewareExtensions
    {
        /// <summary>
        ///     Adds Sentry context middleware to the app.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <returns>The application.</returns>
        public static IApplicationBuilder AddSentryContext(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            app.Use(
                next => context =>
                {
                    context.Request.EnableRewind();

                    return next(context);
                });

            return app.UseMiddleware<SentrySinkContextMiddleware>();
        }
    }
}
