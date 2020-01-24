// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using AspNetCore.Identity.MongoDbCore.Models;
using IdentityServer.Extentions;
using IdentityServer.Models;
using IdentityServer.Services;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServerPlus.Models;
using IdentityServerPlus.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
            PluginManager.Configure(Configuration.GetSection("PluginManager").Get<PluginManagerConfiguration>());
            PluginManager.CollectAll(services.BuildServiceProvider());

            services.AddControllersWithViews();
            services.AddOptions();

            var mongoDBConfiguration = Configuration.GetSection("MongoDB").Get<MongoDBConfiguration>();
            services.Configure<MongoDBConfiguration>(Configuration.GetSection("MongoDB"));
            services.AddSingleton<IConfiguration>(Configuration);

            var authentication = services.AddAuthentication();
            PluginManager.BuildAuthentication(authentication);
            

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>(mongoDBConfiguration.ConnectionString, mongoDBConfiguration.Name)
                .AddSignInManager()
                .AddDefaultTokenProviders();
            services.AddCors();
            services.AddSingleton<ICorsPolicyService, CorsPolicyService>();
            var builder = services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddMongoRepository()
                .AddClients()
                .AddIdentityApiResources()
                .AddPersistedGrants();

            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // uncomment if you want to add MVC
            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();
            app.UseCors();

            // uncomment, if you want to add MVC
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}