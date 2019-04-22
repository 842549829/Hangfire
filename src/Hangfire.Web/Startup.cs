using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Console;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.HttpJob;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Holder.ERP.Job.Web
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
            var connectionString = Configuration.GetConnectionString("Hangfire");
            services.AddHangfire(x => x.UseSqlServerStorage(connectionString));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.Configure<HangfireOption>(Configuration.GetSection("Hangfire"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IGlobalConfiguration hangfire,IOptions<HangfireOption> hangfireOption)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            hangfire.UseConsole();
            hangfire.UseHangfireHttpJob();

            var filter = new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
            {
                // Require secure connection for dashboard
                RequireSsl = false,

                SslRedirect = false,
                // Case sensitive login checking
                LoginCaseSensitive = true,
                // Users
                Users = hangfireOption.Value.Users.Select(d => new BasicAuthAuthorizationUser
                {
                    Login = d.UserName,
                    PasswordClear = d.Password
                })
            });

            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                ServerName = "ERP background job",
            });
            app.UseHangfireDashboard("", new DashboardOptions
            {
                Authorization = new[] { filter }
            });
            app.UseMvc(routes =>
            { 
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
