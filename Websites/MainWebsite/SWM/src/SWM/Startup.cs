using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SWM.Models;
using Microsoft.AspNetCore.Mvc;
using SWM.Services;
using Microsoft.AspNetCore.Identity;
using SWM.Models.Repositories;
using System;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Http;

namespace SWM
{
    public class Startup
    {
        private IHostingEnvironment _env;
        private IConfigurationRoot _config;

        public Startup(IHostingEnvironment env)
        {
            _env = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(_env.ContentRootPath)
                .AddJsonFile("config.json")
                .AddEnvironmentVariables();

            _config = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IMailService, MailService>();
            services.AddSingleton(_config);
            services.AddIdentity<SwmUser, UserRoleManager>(config =>
            {
                config.Cookies.ApplicationCookie.LoginPath = "/SignIn";
                config.Cookies.ApplicationCookie.AccessDeniedPath = "/AccessDenied";
                config.Cookies.ApplicationCookie.ExpireTimeSpan = TimeSpan.FromDays(1);
                config.Cookies.ApplicationCookie.CookieName = ".SWM";
                config.Cookies.ApplicationCookie.SlidingExpiration = false;
                config.Password.RequireDigit = false;
                config.Password.RequiredLength = 8;
                config.Password.RequireLowercase = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireUppercase = false;
            }).AddEntityFrameworkStores<SwmContext>();
            services.AddDbContext<SwmContext>();
            services.AddTransient<SwmContextSeedData>();
            services.AddScoped<ISwmRepository, SwmRepository>();
            services.AddLogging();
            services.AddMvc(config =>
            {
                //if (_env.IsEnvironment("Production"))
                //    config.Filters.Add(new RequireHttpsAttribute());
                
            });
            services.Configure<RazorViewEngineOptions>(options => {
                options.ViewLocationExpanders.Add(new ViewLocationExpander());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory factory, SwmContextSeedData seeder)
        {
            if (env.IsEnvironment("Development"))
            {
                app.UseDeveloperExceptionPage();
                factory.AddDebug(LogLevel.Information);
            }
            else
                factory.AddDebug(LogLevel.Error);

            app.UseStaticFiles();
            app.UseIdentity();

            //do not use error pages when invalid api is called instead return 404 status code.
            Func<HttpContext, bool> isApiRequest = (HttpContext context) => context.Request.Path.ToString().Contains("api");
            app.UseWhen(context => !isApiRequest(context), appBuilder =>
            {
                appBuilder.UseStatusCodePagesWithRedirects("~/Error/{0}");
            });

            app.UseMvc(config =>
            {
                config.MapRoute(
                    name: "PublicRoute",
                    template: "Public/{action}",
                    defaults: new { controller = "Public", action = "Index" }
                );

                config.MapRoute(
                    name: "UserRoute",
                    template: "{action}",
                    defaults: new { controller = "User", action = "Dashboard" }
                );
            });
            seeder.EnsureSeedData().Wait();
        }
    }
}
