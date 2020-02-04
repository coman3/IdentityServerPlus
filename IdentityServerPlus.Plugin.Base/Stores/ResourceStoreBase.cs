using IdentityServer4.Models;
using IdentityServer4.Stores;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServerPlus.Plugin.Base.Stores
{
    /// <summary>
    /// ResourceStore for IdentityServer4. This includes gets for Clients, ApiResources and IdentityResources
    /// </summary>
    public abstract class ResourceStoreBase : IResourceStore, IClientStore
    {
        public abstract Task<ApiResource> FindApiResourceAsync(string name);
        public abstract Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames);
        public abstract Task<Client> FindClientByIdAsync(string clientId);
        public abstract Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames);
        public abstract Task<Resources> GetAllResourcesAsync();
    }
}
