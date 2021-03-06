﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StockPriceApi.Hubs;
using StockPriceApi.Listener;

namespace StockPriceApi
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
            services.AddSingleton<StockPriceListener>();
            services.AddSignalR();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
            Console.WriteLine(env.EnvironmentName);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            ((StockPriceListener)app.ApplicationServices.GetService(typeof(StockPriceListener))).Start();
            applicationLifetime.ApplicationStopping.Register(() =>
            {
                Console.WriteLine("Handle Shutdown");
                ((StockPriceListener)app.ApplicationServices.GetService(typeof(StockPriceListener))).Stop();
            });

            app.UseCors(builder =>
            {
                builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .SetIsOriginAllowed((host) => true) // workaround as AllowAnyOrigin does not work
                    .AllowCredentials();
            });
            app.UseSignalR(options =>
            {
                options.MapHub<StockPriceHub>("/prices");
            });
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
