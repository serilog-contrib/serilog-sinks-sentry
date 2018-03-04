using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Internal;

namespace Serilog
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
        // ReSharper disable once StyleCop.SA1625
        public static IApplicationBuilder AddSentryContext(this IApplicationBuilder app)
        {
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
