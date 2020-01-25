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
using IdentityServerPlus.Plugin.Base.Services;
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
            PluginManager.Configure(Configuration);
            PluginManager.CollectAll();


            services.AddMvc().AddControllersAsServices();
            services.AddOptions();

            var mongoDBConfiguration = Configuration.GetSection("MongoDB").Get<MongoDBConfiguration>();
            services.Configure<MongoDBConfiguration>(Configuration.GetSection("MongoDB"));
            services.AddSingleton<IConfiguration>(Configuration);



            services.AddCors();
            services.AddSingleton<ICorsPolicyService, CorsPolicyService>(); // Move to MongoDB Plugin


            var authentication = services.AddAuthentication();
            // Call IAuthenticationProvider
            PluginManager.BuildAuthentication(authentication);


            var identity = services.AddIdentity<ApplicationUser, ApplicationRole>() // Build a general model for application user that can be implemented into any database schema
                .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>(mongoDBConfiguration.ConnectionString, mongoDBConfiguration.Name) // Move to MongoDB Plugin
                .AddSignInManager()
                .AddDefaultTokenProviders();
            // Call IIdentityProvider
            

            var identityServer = services.AddIdentityServer()
                .AddDeveloperSigningCredential() // Move to Signing Credential Plugin
                .AddMongoRepository() // Move to MongoDB Plugin
                .AddClients() // Move to MongoDB Plugin
                .AddIdentityApiResources() // Move to MongoDB Plugin
                .AddPersistedGrants(); // Move to MongoDB Plugin
            // Call IIdentityServerProvider

            //Call IServiceConfigurationProvider Services (to override any existing services if needed)
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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

            // Call IAppConfigurationProvider
        }
    }
}