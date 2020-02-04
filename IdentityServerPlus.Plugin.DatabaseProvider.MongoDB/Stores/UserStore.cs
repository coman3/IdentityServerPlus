using IdentityServer.Models;
using IdentityServerPlus.Plugin.Base.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace IdentityServerPlus.Plugin.DatabaseProvider.MongoDB.Stores
{
    internal class UserStore<TUser> : UserStoreBase<TUser>
        where TUser : ApplicationUser
    {

        private IMongoCollection<TUser> _users { get; }
        private ILogger _logger { get; }
        private List<TUser> UserCache = new List<TUser>();

        public UserStore(IMongoDatabase applicationDatabase, ILogger<UserStore<TUser>> logger)
        {
            _users = applicationDatabase.GetCollection<TUser>("users");
            _logger = logger;
        }
        public override IQueryable<TUser> Users => _users.AsQueryable();

        public override async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                user.Id = Guid.NewGuid();
                await _users.InsertOneAsync(user);
                return IdentityResult.Success;
            }
            catch (MongoException ex)
            {
                return IdentityResult.Failed(new IdentityError() { Description = ex.Message, Code = "IUSCA_CME" });
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError() { Description = ex.Message, Code = "IUSCA_CE" });
            }
        }

        public override async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                await _users.DeleteOneAsync(x => x.Id == user.Id);
                return IdentityResult.Success;
            }
            catch (MongoException ex)
            {
                return IdentityResult.Failed(new IdentityError() { Description = ex.Message, Code = "IUSDA_DME" });
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError() { Description = ex.Message, Code = "IUSDA_DE" });
            }
        }

        public override void Dispose()
        {
            UserCache = null;
        }

        public override async ValueTask DisposeAsync()
        {
            await Task.Run(Dispose);
        }

        public override async Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            try
            {
                var userCached = UserCache.SingleOrDefault(x => x.Email == normalizedEmail);
                if (userCached != null) return userCached;

                var result = await _users.FindAsync(Builders<TUser>.Filter.Eq(x => x.Email, normalizedEmail));
                var user = await result.SingleOrDefaultAsync();
                UserCache.Add(user);
                return user;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Finding user failed!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Finding user resulted in generic error (Could be two with the same id!)!");
            }
            return null;
        }

        public override async Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            try
            {
                var userCached = UserCache.SingleOrDefault(x => x.Id == new Guid(userId));
                if (userCached != null) return userCached;

                var result = await _users.FindAsync(Builders<TUser>.Filter.Eq(x => x.Id, new Guid(userId)));
                var user = await result.SingleOrDefaultAsync();
                UserCache.Add(user);
                return user;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Finding user failed!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Finding user resulted in generic error (Could be two with the same id!)!");
            }
            return null;
        }

        public override async Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            try
            {
                var userCached = UserCache.SingleOrDefault(x=> x.Providers.Any(x => x.ProviderKey == providerKey && x.LoginProvider == loginProvider));
                if (userCached != null) return userCached;

                var result = await _users.FindAsync(Builders<TUser>.Filter.ElemMatch(x => x.Providers, c => c.LoginProvider == loginProvider && c.ProviderKey == providerKey));
                var user = await result.SingleOrDefaultAsync();
                UserCache.Add(user);
                return user;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Finding user failed!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Finding user resulted in generic error (Could be two with the same id!)!");
            }
            return null;
        }

        public override async Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            try
            {
                var userCached = UserCache.SingleOrDefault(x => x.Username == normalizedUserName);
                if (userCached != null) return userCached;
                var result = await _users.FindAsync(Builders<TUser>.Filter.Eq(x => x.Username, normalizedUserName));
                var user = await result.SingleOrDefaultAsync();
                UserCache.Add(user);
                return user;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Finding user failed!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Finding user resulted in generic error (Could be two with the same id!)!");
            }
            return null;
        }


        public override async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {

            try
            {
                var resultReplace = await _users.ReplaceOneAsync(Builders<TUser>.Filter.Eq(x => x.Id, user.Id), user);
                if (resultReplace.IsAcknowledged)
                {
                    return IdentityResult.Success;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update user: {0}", user.Id);
                return IdentityResult.Failed(new IdentityError() { Description = ex.Message });
            }
            return IdentityResult.Failed(new IdentityError() { Description = "Something went wrong while updating" });
        }

        public override async Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _users.FindAsync(Builders<TUser>.Filter.ElemMatch(x => x.Claims, c => c.Type == claim.Type && c.Value == claim.Value && c.ValueType == claim.ValueType));
                var users = await result.ToListAsync();
                UserCache.AddRange(users);
                return users;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Finding users failed!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Finding users resulted in generic error!");
            }
            return null;
        }
    }
}
