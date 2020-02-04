using IdentityServer.Models;
using IdentityServerPlus.Plugin.Base.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityServerPlus.Plugin.DatabaseProvider.MongoDB.Stores
{
    class RoleStore<TRole> : RoleStoreBase<TRole>
        where TRole : ApplicationRole
    {
        private IMongoCollection<TRole> _roles { get; }
        private ILogger _logger { get; }

        private List<TRole> RoleCache = new List<TRole>();

        public override IQueryable<TRole> Roles => _roles.AsQueryable();

        public RoleStore(IMongoDatabase applicationDatabase, ILogger<RoleStore<TRole>> logger)
        {
            _roles = applicationDatabase.GetCollection<TRole>("roles");
            _logger = logger;
        }

        public override async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                await _roles.InsertOneAsync(role);
                return IdentityResult.Success;
            }
            catch (MongoException ex)
            {
                return IdentityResult.Failed(new IdentityError() { Description = ex.Message, Code = "IRSCA_CME" });
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError() { Description = ex.Message, Code = "IRSCA_CE" });
            }
        }

        public override async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                await _roles.DeleteOneAsync(x => x.Id == role.Id);
                return IdentityResult.Success;
            }
            catch (MongoException ex)
            {
                return IdentityResult.Failed(new IdentityError() { Description = ex.Message, Code = "IRSDA_DME" });
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError() { Description = ex.Message, Code = "IRSDA_DE" });
            }
        }

        public override async Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            try
            {
                var role = RoleCache.SingleOrDefault(x => x.Id == roleId);
                if (role != null)
                    return role;

                var result = await _roles.FindAsync(Builders<TRole>.Filter.Eq(x => x.Id, roleId));
                var roleDb = await result.SingleOrDefaultAsync();
                if (roleDb != null)
                    RoleCache.Add(roleDb);
                return role;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Finding role failed!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Finding role resulted in generic error (Could be two with the same id!)!");
            }
            return null;
        }

        public override Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            return FindByIdAsync(normalizedRoleName, cancellationToken);
        }

        public override async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            try
            {
                var resultReplace = await _roles.ReplaceOneAsync(Builders<TRole>.Filter.Eq(x => x.Id, role.Id), role);
                if (resultReplace.IsAcknowledged)
                {
                    return IdentityResult.Success;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update role: {0}", role.Id);
                return IdentityResult.Failed(new IdentityError() { Description = ex.Message });
            }
            return IdentityResult.Failed(new IdentityError() { Description = "Something went wrong while updating" });

        }
    }
}
