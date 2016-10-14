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
                config.User.RequireUniqueEmail = true;
                config.Cookies.ApplicationCookie.LoginPath = "/SignIn";
                config.Password.RequireDigit = false;
                config.Password.RequiredLength = 8;
                config.Password.RequireLowercase = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireUppercase = false;
            }
            ).AddEntityFrameworkStores<SwmContext>();

            services.AddDbContext<SwmContext>();
            services.AddTransient<SwmContextSeedData>();
            services.AddScoped<ISwmRepository, SwmRepository>();
            services.AddLogging();
            services.AddMvc(config =>
            {
                if (_env.IsEnvironment("Production"))
                    config.Filters.Add(new RequireHttpsAttribute());
                
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
            app.UseMvc(config =>
            {
                config.MapRoute(
                    name: "Home",
                    template: "SignIn",
                    defaults: new { controller = "Home", action = "Index" }
                );

                config.MapRoute(
                    name: "ContactRoute",
                    template: "Contact",
                    defaults: new { controller = "Home", action = "Contact" }
                );

                config.MapRoute(
                    name: "PasswordResetRoute",
                    template: "PasswordReset",
                    defaults: new { controller = "Home", action = "PasswordReset" }
                );

                config.MapRoute(
                    name: "SignOut",
                    template: "SignOut",
                    defaults: new { controller = "User", action = "SignOut" }
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
