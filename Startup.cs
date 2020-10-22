using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;

namespace aspnetcore_oidc
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication(options => {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect(options => {
                options.ClientId = Configuration["Criipto:ClientId"];  
                options.ClientSecret = Configuration["Criipto:ClientSecret"];
                options.Authority = $"https://{Configuration["Criipto:Domain"]}/";
                options.ResponseType = "code";
                options.GetClaimsFromUserInfoEndpoint = true;
                options.TokenValidationParameters.ValidIssuers = new [] {
                    options.Authority
                };
                options.TokenValidationParameters.ClockSkew = TimeSpan.FromMinutes(5);
                if(options.SecurityTokenValidator is JwtSecurityTokenHandler jwtHandler)
                {
                    jwtHandler.MapInboundClaims = false;
                    jwtHandler.InboundClaimTypeMap.Clear();
                    jwtHandler.OutboundClaimTypeMap.Clear();
                }

                // The next to settings must match the Callback URLs in Criipto Verify
                options.CallbackPath = new PathString("/callback");
                options.SignedOutCallbackPath = new PathString("/signout");

                // Hook up an event handler to set the acr_value of the authorize request
                // In a real world implementation this is probably a bit more flexible
                options.Events = new OpenIdConnectEvents() {
                    OnRedirectToIdentityProvider = context => {
                        context.ProtocolMessage.AcrValues = context.Request.Query["loginmethod"];
                        return Task.FromResult(0);
                    }
                };
            });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                IdentityModelEventSource.ShowPII = true;
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            

            app.UseEndpoints(routes =>
            {
                routes.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
