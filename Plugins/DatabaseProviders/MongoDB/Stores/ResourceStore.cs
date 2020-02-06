using IdentityServer4.Models;
using IdentityServerPlus.Plugin.Base.Stores;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServerPlus.Plugin.DatabaseProvider.MongoDB.Stores
{
    public class ResourceStore : ResourceStoreBase
    {
        private IMongoCollection<Resource> _resources { get; }
        private IMongoCollection<Client> _clients { get; }
        private ILogger _logger { get; }

        public ResourceStore(IMongoDatabase applicationDatabase, ILogger<ResourceStore> logger)
        {
            _resources = applicationDatabase.GetCollection<Resource>("resources");
            _clients = applicationDatabase.GetCollection<Client>("clients");
            _logger = logger;
        }
        public override async Task<ApiResource> FindApiResourceAsync(string name)
        {
            var query = await _resources.FindAsync<ApiResource>(Builders<Resource>.Filter.OfType<ApiResource>(Builders<ApiResource>.Filter.Eq(x => x.Name, name)));
            return await query.SingleOrDefaultAsync();
        }

        public override async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var query = await _resources.FindAsync<ApiResource>(Builders<Resource>.Filter.OfType<ApiResource>(Builders<ApiResource>.Filter.ElemMatch(x => x.Scopes, Builders<Scope>.Filter.In(x => x.Name, scopeNames))));
            return await query.ToListAsync();
        }

        public override async Task<Client> FindClientByIdAsync(string clientId)
        {
            var query = await _clients.FindAsync<Client>(Builders<Client>.Filter.Eq(x=> x.ClientId, clientId));
            return await query.SingleOrDefaultAsync();
        }

        public override async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var query = await _resources.FindAsync<IdentityResource>(Builders<Resource>.Filter.OfType<IdentityResource>(Builders<IdentityResource>.Filter.In(x => x.Name, scopeNames)));
            return await query.ToListAsync();
        }

        public override async Task<Resources> GetAllResourcesAsync()
        {
            var queryApiResouces = await _resources.FindAsync<ApiResource>(Builders<Resource>.Filter.OfType<ApiResource>());
            var queryIdentityResouces = await _resources.FindAsync<IdentityResource>(Builders<Resource>.Filter.OfType<IdentityResource>());
            return new Resources()
            {
                ApiResources = await queryApiResouces.ToListAsync(),
                IdentityResources= await queryIdentityResouces.ToListAsync()
            };
        }
    }
}
