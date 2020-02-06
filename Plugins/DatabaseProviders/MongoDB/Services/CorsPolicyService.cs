using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Models;
using IdentityServer4.Services;

namespace IdentityServerPlus.Plugin.DatabaseProvider.MongoDB.Services
{
    public class CorsPolicyService : ICorsPolicyService
    {
        async Task<bool> ICorsPolicyService.IsOriginAllowedAsync(string origin)
        {
            return true;
        }
    }
}