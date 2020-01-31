using IdentityServer.Models;
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
    internal class UserStore<TUser> : IUserLoginStore<TUser>,
        IUserStore<TUser>,
        IAsyncDisposable,
        IDisposable,
        IUserClaimStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IUserEmailStore<TUser>,
        IUserLockoutStore<TUser>,
        IUserPhoneNumberStore<TUser>,
        IQueryableUserStore<TUser>,
        IUserTwoFactorStore<TUser>,
        IUserAuthenticationTokenStore<TUser>,
        IUserAuthenticatorKeyStore<TUser>,
        IUserTwoFactorRecoveryCodeStore<TUser>
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
        IQueryable<TUser> IQueryableUserStore<TUser>.Users => _users.AsQueryable();


        async Task IUserClaimStore<TUser>.AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {   
            var newClaims = claims.Select(c => new ApplicationUserClaim(c)).ToList();
            user.Claims.AddRange(newClaims);
        }

        async Task IUserLoginStore<TUser>.AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            var provider = new ApplicationProviderInfo(login) { ProviderLinkedAt = DateTime.UtcNow };
            user.Providers.Add(provider);
        }

        async Task<int> IUserTwoFactorRecoveryCodeStore<TUser>.CountCodesAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        async Task<IdentityResult> IUserStore<TUser>.CreateAsync(TUser user, CancellationToken cancellationToken)
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

        async Task<IdentityResult> IUserStore<TUser>.DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            UserCache = null;
        }

        public async ValueTask DisposeAsync()
        {
            this.Dispose();
        }

        async Task<TUser> IUserEmailStore<TUser>.FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            try
            {
                if (UserCache.Any(x => x.Email == normalizedEmail))
                    return UserCache.SingleOrDefault(x => x.Email == normalizedEmail);
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

        async Task<TUser> IUserStore<TUser>.FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            try
            {
                if (UserCache.Any(x => x.Id == new Guid(userId)))
                    return UserCache.SingleOrDefault(x => x.Id == new Guid(userId));
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

        async Task<TUser> IUserLoginStore<TUser>.FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            try
            {
                if (UserCache.Any(x => x.Providers.Any(x => x.ProviderKey == providerKey && x.LoginProvider == loginProvider)))
                    return UserCache.SingleOrDefault(x => x.Providers.Any(x => x.ProviderKey == providerKey && x.LoginProvider == loginProvider));
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

        async Task<TUser> IUserStore<TUser>.FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            try
            {
                if (UserCache.Any(x => x.Username == normalizedUserName))
                    return UserCache.SingleOrDefault(x => x.Username == normalizedUserName);
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

        async Task<int> IUserLockoutStore<TUser>.GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            return user.AccessFailedCount;
        }

        async Task<string> IUserAuthenticatorKeyStore<TUser>.GetAuthenticatorKeyAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        async Task<IList<Claim>> IUserClaimStore<TUser>.GetClaimsAsync(TUser user, CancellationToken cancellationToken)
        {
            return user.Claims.Select(x => new Claim(x.Type, x.Value, x.ValueType, x.Issuer, x.OriginalIssuer)).ToList();
        }

        async Task<string> IUserEmailStore<TUser>.GetEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            return user.Email;
        }

        async Task<bool> IUserEmailStore<TUser>.GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            return user.EmailConfirmed;
        }

        async Task<bool> IUserLockoutStore<TUser>.GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            return user.LockoutEnabled;
        }

        async Task<DateTimeOffset?> IUserLockoutStore<TUser>.GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken)
        {
            return user.LockoutEnd;
        }

        async Task<IList<UserLoginInfo>> IUserLoginStore<TUser>.GetLoginsAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        async Task<string> IUserEmailStore<TUser>.GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            return user.Email;
        }

        async Task<string> IUserStore<TUser>.GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            return user.Username;
        }

        async Task<string> IUserPasswordStore<TUser>.GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
        {
            return user.PasswordHash;
        }

        async Task<string> IUserPhoneNumberStore<TUser>.GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken)
        {
            return user.PhoneNumber;
        }

        async Task<bool> IUserPhoneNumberStore<TUser>.GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            return user.PhoneNumberConfirmed;
        }

        async Task<string> IUserSecurityStampStore<TUser>.GetSecurityStampAsync(TUser user, CancellationToken cancellationToken)
        {
            return user.SecurityStamp;
        }

        async Task<string> IUserAuthenticationTokenStore<TUser>.GetTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        async Task<bool> IUserTwoFactorStore<TUser>.GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            return user.TwoFactorEnabled;
        }

        async Task<string> IUserStore<TUser>.GetUserIdAsync(TUser user, CancellationToken cancellationToken)
        {
            return user.Id.ToString();
        }

        async Task<string> IUserStore<TUser>.GetUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            return user.Username;
        }

        async Task<IList<TUser>> IUserClaimStore<TUser>.GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        async Task<bool> IUserPasswordStore<TUser>.HasPasswordAsync(TUser user, CancellationToken cancellationToken)
        {
            return !string.IsNullOrWhiteSpace(user.PasswordHash);
        }

        async Task<int> IUserLockoutStore<TUser>.IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            user.AccessFailedCount += 1;
            return user.AccessFailedCount;
        }

        async Task<bool> IUserTwoFactorRecoveryCodeStore<TUser>.RedeemCodeAsync(TUser user, string code, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        async Task IUserClaimStore<TUser>.RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        async Task IUserLoginStore<TUser>.RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        async Task IUserAuthenticationTokenStore<TUser>.RemoveTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        async Task IUserClaimStore<TUser>.ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        async Task IUserTwoFactorRecoveryCodeStore<TUser>.ReplaceCodesAsync(TUser user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        async Task IUserLockoutStore<TUser>.ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            user.AccessFailedCount = 0;
        }

        async Task IUserAuthenticatorKeyStore<TUser>.SetAuthenticatorKeyAsync(TUser user, string key, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        async Task IUserEmailStore<TUser>.SetEmailAsync(TUser user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;

        }

        async Task IUserEmailStore<TUser>.SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;

        }

        async Task IUserLockoutStore<TUser>.SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.LockoutEnabled = enabled;


        }

        async Task IUserLockoutStore<TUser>.SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            user.LockoutEnd = lockoutEnd;

        }

        async Task IUserEmailStore<TUser>.SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.Email = normalizedEmail;

        }

        async Task IUserStore<TUser>.SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.Username = normalizedName;

        }

        async Task IUserPasswordStore<TUser>.SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;

        }

        async Task IUserPhoneNumberStore<TUser>.SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            user.PhoneNumber = phoneNumber;

        }

        async Task IUserPhoneNumberStore<TUser>.SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.PhoneNumberConfirmed = confirmed;

        }

        async Task IUserSecurityStampStore<TUser>.SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken)
        {
            user.SecurityStamp = stamp;

        }

        async Task IUserAuthenticationTokenStore<TUser>.SetTokenAsync(TUser user, string loginProvider, string name, string value, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        async Task IUserTwoFactorStore<TUser>.SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {

            user.TwoFactorEnabled = enabled;

        }

        async Task IUserStore<TUser>.SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
        {

            user.Username = userName;

        }

        async Task<IdentityResult> IUserStore<TUser>.UpdateAsync(TUser user, CancellationToken cancellationToken)
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
    }
}
