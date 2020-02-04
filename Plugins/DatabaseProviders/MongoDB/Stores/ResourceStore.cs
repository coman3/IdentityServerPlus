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
        private List<Resource> ResourceCache { get; } = new List<Resource>();
        private List<Client> ClientsCache { get; } = new List<Client>();
        private IMongoCollection<Resource> _resources { get; }
        private IMongoCollection<Client> _clients { get; }
        private ILogger _logger { get; }

        public ResourceStore(IMongoDatabase applicationDatabase, ILogger<ResourceStore> logger)
        {
            _resources = applicationDatabase.GetCollection<Resource>("resources");
            _clients = applicationDatabase.GetCollection<Client>("clients");
            _logger = logger;
        }
        private async Task GetAll()
        {
            if (ResourceCache.Count <= 0)
            {
                _logger.LogInformation("Fetching Resources from Database and caching");
                var resources = await _resources.AsQueryable().ToListAsync();
                ResourceCache.AddRange(resources);
            }
            if (ClientsCache.Count <= 0)
            {
                _logger.LogInformation("Fetching clients from Database and caching");
                var clients = await _clients.AsQueryable().ToListAsync();
                ClientsCache.AddRange(clients);
            }
        }

        public override async Task<ApiResource> FindApiResourceAsync(string name)
        {
            await GetAll();
            var resource = ResourceCache.OfType<ApiResource>().Where(x => x.Name == name).SingleOrDefault();
            return resource;
        }

        public override async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            await GetAll();
            var resources = ResourceCache.OfType<ApiResource>().Where(x => x.Scopes.Any(c => scopeNames.Contains(c.Name)));
            return resources;
        }

        public override async Task<Client> FindClientByIdAsync(string clientId)
        {
            await GetAll();
            var client = ClientsCache.OfType<Client>().Where(x => x.ClientId == clientId).SingleOrDefault();
            return client;
        }

        public override async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            await GetAll();
            var resources = ResourceCache.OfType<IdentityResource>().Where(x => scopeNames.Contains(x.Name));
            return resources;
        }

        public override Task<Resources> GetAllResourcesAsync()
        {
            return Task.FromResult(new Resources(ResourceCache.OfType<IdentityResource>(), ResourceCache.OfType<ApiResource>()));
        }
    }
}
