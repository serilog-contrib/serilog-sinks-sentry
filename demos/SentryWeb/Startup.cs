using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SentryWeb.Scrubbing;
using Serilog;
using SharpRaven.Data;

namespace SentryWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                // Insert Sentry DSN here
                .WriteTo.Sentry(
                    "", 
                    dataScrubber: new CustomLogScrubber()
                )
                .Enrich.FromLogContext()
                // Add Http Context for Sentry
                .Destructure.With<HttpContextDestructingPolicy>()
                .Filter.ByExcluding(e => e.Exception?.CheckIfCaptured() == true)
                .CreateLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            // Add Http Context for Sentry
            app.AddSentryContext();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
