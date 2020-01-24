// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using AspNetCore.Identity.MongoDbCore.Models;
using IdentityServer.Extentions;
using IdentityServer.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityServer {
    public class Startup {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment) {
            Environment = environment;
            var builder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional : true, reloadOnChange : true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional : true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services) {
            // uncomment, if you want to add an MVC-based UI
            services.AddControllersWithViews();
            services.AddOptions();
            var mongoDBConfiguration = Configuration.GetSection("MongoDB").Get<MongoDBConfiguration>();
            services.Configure<MongoDBConfiguration>(Configuration.GetSection("MongoDB"));
            services.AddSingleton<IConfiguration>(Configuration);

            // var builder = services.AddIdentityServer()
            //     .AddInMemoryIdentityResources(Config.Ids)
            //     .AddInMemoryApiResources(Config.Apis)
            //     .AddInMemoryClients(Config.Clients);

            services.AddAuthentication().AddOpenIdConnect("microsoft", options => {
                options.ClientId = "7d6d3978-f958-4759-887d-498b52ede1df";
                options.ClientSecret = "DSBjc:/?V-FBGAfY64t96gm@GG=qfcct";
                options.MetadataAddress = "https://login.microsoftonline.com/15864aef-67b6-4b0e-8bfe-cd61201a6837/v2.0/.well-known/openid-configuration";
                options.Authority = "https://login.microsoftonline.com/15864aef-67b6-4b0e-8bfe-cd61201a6837";
            });

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>(mongoDBConfiguration.ConnectionString, mongoDBConfiguration.Name)
                .AddSignInManager()
                .AddDefaultTokenProviders();

            var builder = services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddMongoRepository()
                .AddClients()
                .AddIdentityApiResources()
                .AddPersistedGrants();

            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();
        }

        public void Configure(IApplicationBuilder app) {
            if (Environment.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            // uncomment if you want to add MVC
            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();

            // uncomment, if you want to add MVC
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}