using Jwt.Proxy.ConfigSettings;
using Jwt.Proxy.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Jwt.Proxy.Website
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IProxyService, ProxyService>();
            services.Configure<ProxySettings>(Configuration.GetSection("ProxySettings"));
            services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Success case - proxy
            app.MapWhen(TokenIsValid, builder =>
            {
                var proxySettings = builder.ApplicationServices.GetService<IOptions<ProxySettings>>().Value;

                Console.WriteLine($"Proxying to {proxySettings.BackendScheme}://{proxySettings.BackendHost}:{proxySettings.BackendPort}");

                builder.RunProxy(new ProxyOptions()
                {
                    Host = proxySettings.BackendHost,
                    Port = proxySettings.BackendPort.ToString(),
                    Scheme = proxySettings.BackendScheme
                });
            });

            // Failure case - Redirect OR return http 403
            app.Use(async (c, n) =>
            {
                var proxySettings = c.RequestServices.GetService<IOptions<ProxySettings>>().Value;

                if (proxySettings.Mode == ProxyMode.Redirect)
                {
                    c.Response.Redirect(string.Format(proxySettings.RedirectUrl, c.Request.GetEncodedUrl()));
                }
                else if (proxySettings.Mode == ProxyMode.Http403)
                {
                    c.Response.StatusCode = 403;
                }
            });
        }

        bool TokenIsValid(HttpContext context)
        {
            var token = context.RequestServices.GetService<IJwtService>().GetTokenFromContext(context);
            var proxy = context.RequestServices.GetService<IProxyService>();

            return proxy.TokenHasRequiredProperties(token) && proxy.TokenSatisfiesUrlConstraints(token, context);
        }
    }
}
