using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Net.Http.Headers;

namespace signal_2019_dotnetcore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var twilioSection = Configuration.GetSection("Twilio");
            var accountSid = twilioSection.GetValue<string>("AccountSid");
            var authToken = twilioSection.GetValue<string>("AuthToken");
            var authyApiKey = twilioSection.GetValue<string>("AuthyApiKey");

            if(string.IsNullOrEmpty(accountSid))
              throw new ArgumentNullException(nameof(accountSid));

            if(string.IsNullOrEmpty(authToken))
                throw new ArgumentNullException(nameof(authToken));

            if(string.IsNullOrEmpty(authyApiKey))
                throw new ArgumentNullException(nameof(authyApiKey));

            var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes(accountSid + ":" + authToken));

            services.AddHttpClient("lookup", c =>
            {
                c.BaseAddress = new Uri("https://lookups.twilio.com");
                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            services.AddHttpClient("verify", c =>
            {
                c.BaseAddress = new Uri("https://verify.twilio.com");
                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            services.AddHttpClient("authy", c =>
            {
                c.BaseAddress = new Uri("https://api.authy.com");
                c.DefaultRequestHeaders.Add("X-Authy-API-Key", authyApiKey);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            
            services.Configure<TwilioOptions>(twilioSection);
            
            services.AddSingleton<IUserRepository, UserRepository>();
            
            services.AddSession(options => {
                options.Cookie.Name = "twilio.demo";
                options.Cookie.IsEssential = true;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseStaticFiles()
                .UseSession()
                .UseMvc(routes =>
                {
                    routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
                });
        }
    }
}
