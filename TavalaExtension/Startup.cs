using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DirectScale.Disco.Extension.Middleware;
using TavalaExtension.Hooks.Order;
using TavalaExtension.Repositories;
using TavalaExtension.Services;
using System;

namespace TavalaExtension
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            CurrentEnvironment = env;
        }

        public IConfiguration Configuration { get; }
        private IWebHostEnvironment CurrentEnvironment { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            string environmentURL = Environment.GetEnvironmentVariable("DirectScaleServiceUrl");

            // services.AddResponseCaching();
            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.WithOrigins(environmentURL, environmentURL.Replace("corpadmin", "clientextension"))
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowAnyOrigin());
            });
            services.AddRazorPages();
            services.AddMvc();
            services.AddDirectScale(options =>
            {
                options.AddHook<SubmitOrderHook>();
                //options.AddCustomPage(Menu.AssociateDetail, "Hello Associate", "ViewAdministration", "/CustomPage/SecuredHelloWorld");
                //options.AddHook<CreateAutoshipHook>();
                //options.AddMerchant<StripeMoneyIn>();
                //options.AddEventHandler("OrderCreated", "/api/webhooks/Order/CreateOrder");
                services.AddControllers();
            });

            //Repositories
            services.AddSingleton<IOrdersRepository, OrdersRepository>();

            //Services
            services.AddSingleton<IOrdersService, OrdersService>();

            services.AddControllersWithViews();
            //Configurations
            //services.Configure<configSetting>(Configuration.GetSection("configSetting"));
            services.AddMvc(option => option.EnableEndpointRouting = false);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var environmentUrl = Environment.GetEnvironmentVariable("DirectScaleServiceUrl");
            if (environmentUrl != null)
            {
                var serverUrl = environmentUrl.Replace("https://vidafy.corpadmin.", "");
                var appendUrl = @" http://"+ serverUrl + " " + "https://" + serverUrl + " " + "http://*." + serverUrl + " " + "https://*." + serverUrl;

                var csPolicy = "frame-ancestors https://vaa.corpadmin.directscale.com https://vaa.corpadmin.directscalestage.com https://639e-117-247-182-219.in.ngrok.io https://localhost:44308" + appendUrl + ";";
                app.UseRequestLocalization();

                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseExceptionHandler("/Home/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                //Configure Cors
                app.UseRouting();

                app.UseCors("CorsPolicy");
                app.UseHttpsRedirection();

                app.UseStaticFiles(new StaticFileOptions
                {
                    OnPrepareResponse = ctx =>
                    {
                        ctx.Context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                    }
                });

                app.UseStaticFiles();
                app.UseAuthorization();

                //DS
                app.UseDirectScale();
                app.Use(async (context, next) =>
                {
                    context.Response.Headers.Add("Content-Security-Policy", csPolicy);
                    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                    await next();
                });
            }


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            app.UseMvc();
        }
    }
}
