using IdentityServer.Interfaces;
using IdentityServer.Repositories;
using IdentityServer.Stores;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Extentions {
    public static class IdentityServerBuilderExtensions {
        public static IIdentityServerBuilder AddMongoRepository(this IIdentityServerBuilder builder) {
            builder.Services.AddTransient<IRepository, MongoRepository>();

            return builder;
        }

        /// <summary>
        /// Configure ClientId / Secrets
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configurationOption"></param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddClients(this IIdentityServerBuilder builder) {
            builder.Services.AddTransient<IClientStore, MongoClientStore>();

            return builder;
        }

        /// <summary>
        /// Configure API  &  Resources
        /// Note: Api's have also to be configured for clients as part of allowed scope for a given clientID 
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddIdentityApiResources(this IIdentityServerBuilder builder) {
            builder.Services.AddTransient<IResourceStore, MongoResourceStore>();

            return builder;
        }

        /// <summary>
        /// Configure Grants
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddPersistedGrants(this IIdentityServerBuilder builder) {
            builder.Services.AddSingleton<IPersistedGrantStore, MongoPersistedGrantStore>();

            return builder;
        }

    }
}