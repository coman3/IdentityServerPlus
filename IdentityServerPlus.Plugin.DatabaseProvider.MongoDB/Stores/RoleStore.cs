using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityServerPlus.Plugin.DatabaseProvider.MongoDB.Stores
{
    class RoleStore<TRole> : IQueryableRoleStore<TRole>, 
        IRoleStore<TRole>, 
        IDisposable, 
        IAsyncDisposable, 
        IRoleClaimStore<TRole>
        where TRole : ApplicationRole
    {
        IQueryable<TRole> IQueryableRoleStore<TRole>.Roles => throw new NotImplementedException();

        async Task IRoleClaimStore<TRole>.AddClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        async Task<IdentityResult> IRoleStore<TRole>.CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        async Task<IdentityResult> IRoleStore<TRole>.DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        void IDisposable.Dispose()
        {
            
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            
        }

        async Task<TRole> IRoleStore<TRole>.FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        async Task<TRole> IRoleStore<TRole>.FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        async Task<IList<Claim>> IRoleClaimStore<TRole>.GetClaimsAsync(TRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        async Task<string> IRoleStore<TRole>.GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        async Task<string> IRoleStore<TRole>.GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        async Task<string> IRoleStore<TRole>.GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        async Task IRoleClaimStore<TRole>.RemoveClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        async Task IRoleStore<TRole>.SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        async Task IRoleStore<TRole>.SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        async Task<IdentityResult> IRoleStore<TRole>.UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
