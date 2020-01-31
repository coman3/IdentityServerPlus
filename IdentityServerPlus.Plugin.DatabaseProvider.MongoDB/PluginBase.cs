using IdentityServerPlus.Plugin.Base.Models;
using IdentityServerPlus.Plugin.Base.Structures;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace IdentityServerPlus.Plugin.DatabaseProvider.MongoDB
{
    public class MongoDBDatabaseProviderPlugin : PluginBase
    {
        public MongoDBDatabaseProviderPlugin() : base("MongoDB Connector", "0.0.0.1")
        {

        }

        public override Guid Id => new Guid("320a226b-9b70-4883-bcb6-a457be08b7c0");

        public override DateTime LastUpdated => new DateTime(2020, 01, 30);

        public override IEnumerable<ProviderItem> GetProviderTypesAndArguments(IConfiguration configuration)
        {
            yield return new ProviderItem<MongoDBDatabaseProvider>();
        }
    }
}
