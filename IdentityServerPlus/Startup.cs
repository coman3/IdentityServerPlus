// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using IdentityServer.Models;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServerPlus.Models;
using IdentityServerPlus.Plugin.Base.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;

namespace IdentityServer
{
    public class Startup
    {

        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }
        public PluginManager PluginManager { get; }

        public Startup(IWebHostEnvironment environment, PluginManager pluginManager)
        {
            Environment = environment;
            var builder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
            PluginManager = pluginManager;

        }

        public void ConfigureServices(IServiceCollection services)
        {
            PluginManager.Configure(Configuration);
            PluginManager.CollectAll();
            services.AddSingleton(PluginManager);

            var mvcBuilder = services.AddMvc().AddControllersAsServices();
            mvcBuilder = PluginManager.BuildThemeProviders(mvcBuilder);


            services.AddOptions();
            services.AddSingleton<IConfiguration>(Configuration);

            services.AddCors();

            var authentication = services.AddAuthentication();
            authentication = PluginManager.BuildAuthentication(authentication);

            

            var identity = services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.User.RequireUniqueEmail = true;
                })
                .AddSignInManager()
                .AddDefaultTokenProviders();
            // Call IIdentityProvider
            identity = PluginManager.BuildIdentity(identity);

            var identityServer = services.AddIdentityServer()
                .AddDeveloperSigningCredential(); // Move to Signing Credential Plugin

            identityServer = PluginManager.BuildIdentityServer(identityServer);
            PluginManager.BuildServices(services);
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                IdentityModelEventSource.ShowPII = true;
            }


            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();
            app.UseCors();


            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });

            PluginManager.BuildAppConfiguration(app);
            PluginManager.Finalize();
        }
    }
}