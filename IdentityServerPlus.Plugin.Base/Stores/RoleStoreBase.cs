using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// Disable await warning as most methods return a single property without needing await calls.
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace IdentityServerPlus.Plugin.Base.Stores
{

    public abstract class RoleStoreBase<TRole> :
        IQueryableRoleStore<TRole>,
        IRoleStore<TRole>,
        IDisposable,
        IAsyncDisposable,
        IRoleClaimStore<TRole>
        where TRole : ApplicationRole
    {
        public abstract IQueryable<TRole> Roles { get; }

        public abstract Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken);

        public abstract Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken);

        public virtual async Task AddClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default)
        {
            role.Claims.Add(new ApplicationClaim(claim));
        }

        public void Dispose()
        {
            
        }

        public async ValueTask DisposeAsync()
        {
            await Task.Run(this.Dispose);
        }

        public abstract Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken);

        public abstract Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken);

        public async Task<IList<Claim>> GetClaimsAsync(TRole role, CancellationToken cancellationToken = default)
        {
            return role.Claims.Select(x => new Claim(x.Type, x.Value, x.ValueType, x.Issuer, x.OriginalIssuer)).ToList();
        }

        public async Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            return role.Id;
        }

        public async Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
        {
            return role.Id;
        }

        public async Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            return role.Name;
        }

        public async Task RemoveClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default)
        {
            role.Claims.RemoveAll(x => x.Type == claim.Type && x.Value == claim.Value && x.Issuer == claim.Issuer);
        }

        public async Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
        {
            role.Id = normalizedName.Trim().Replace(' ', '.').ToLower();
        }


        public async Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
        }

        public abstract Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken);
    }
}
