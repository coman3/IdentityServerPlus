﻿using IdentityServer.Models;
using IdentityServer4.Models;
using IdentityServerPlus.Plugin.Base.Interfaces;
using IdentityServerPlus.Plugin.Base.Models;
using IdentityServerPlus.Plugin.DatabaseProvider.MongoDB.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServerPlus.Plugin.DatabaseProvider.MongoDB
{
    class MongoDBDatabaseProvider : IPluginProvider, IIdentityProvider, IIdentityServerProvider
    {
        public string Name => "MongoDB Database Provider";

        public string Description => "Connect your identity server with MongoDB";

        public IdentityProviderType Type => IdentityProviderType.Database;

        public MongoDBDatabaseProvider()
        {
            MapClasses();
        }

        private void MapClasses()
        {
            // No idea why this is needed............... AutoMap Dosnt work on postloaded assemblies apperently.
            BsonClassMap.RegisterClassMap<ApplicationUser>(cm =>
            {
                cm.AutoMap();
                cm.MapProperty(x => x.AccessFailedCount);
            });

            BsonClassMap.RegisterClassMap<UserLoginInfo>(cm =>
            {
                cm.AutoMap();
                cm.SetIsRootClass(true);
                cm.AddKnownType(typeof(ApplicationProviderInfo));
                cm.MapProperty(x => x.LoginProvider);
                cm.MapProperty(x => x.ProviderDisplayName);
                cm.MapProperty(x => x.ProviderKey);
                cm.MapCreator(p => new UserLoginInfo(p.LoginProvider, p.ProviderKey, p.ProviderDisplayName));
            });
            BsonClassMap.RegisterClassMap<ApplicationProviderInfo>(cm =>
            {   
                cm.MapProperty(x => x.AccessToken);
                cm.MapProperty(x => x.AccessTokenExpiry);
                cm.MapProperty(x => x.IdToken);
                cm.MapProperty(x => x.IdTokenExpiry);
                cm.MapProperty(x => x.ProviderLinkedAt);
                cm.MapCreator(p => new ApplicationProviderInfo(p.LoginProvider, p.ProviderKey, p.ProviderDisplayName));
            });

            BsonClassMap.RegisterClassMap<ApplicationClaim>(cm =>
            {
                cm.MapProperty(x => x.OriginalIssuer);
                //cm.MapProperty(x => x.Properties);
                //cm.MapProperty(x => x.ValueType);
                cm.MapProperty(x => x.Value);
                cm.MapProperty(x => x.Issuer);
                cm.MapProperty(x => x.Type);
                //cm.MapCreator(p => new ApplicationUserClaim() { Issuer = p.Issuer, Type = p.Type});
            });
        }

        public IdentityBuilder Build(IdentityBuilder builder)
        {
            
            var client = new MongoClient();
            builder.Services.AddSingleton<IMongoDatabase>(client.GetDatabase("IdentityServer"));
            builder.Services.AddScoped<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>();
            builder.Services.AddScoped<IRoleStore<ApplicationRole>, RoleStore<ApplicationRole>>();

            return builder;
        }

        public IIdentityServerBuilder Build(IIdentityServerBuilder builder)
        {
            builder = builder
                .AddInMemoryApiResources(new List<ApiResource>())
                .AddInMemoryClients(new List<Client>())
                .AddInMemoryIdentityResources(new List<IdentityResource>())
                .AddInMemoryPersistedGrants();
            return builder;
        }
    }
}
