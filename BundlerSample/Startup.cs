﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Bundler.Transformers;

namespace Bundler
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
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseBundles(options =>
            {
                options.Enabled = true;// !env.IsDevelopment();

                options.AddJs("/all.js", new[] { "js/site.js", "js/b.js" })
                       .Run(config => {
                           return config.Content.Replace("hat", "svin");
                       }).CacheKey += "hat";
                //options.Transforms.Add(new JavaScriptMinifier("/all.js").Include("js/site.js", "js/b.js"));

                options.AddCss("/all.css", "css/site.css", "lib/bootstrap/dist/css/bootstrap.css");
                //options.Transforms.Add(new CssMinifier("/all.css").Include("css/site.css", "/lib/bootstrap/dist/css/bootstrap.css"));
            });

            app.MinifyJavaScript();
            app.MinifyCss();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
