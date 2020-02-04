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
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously as

namespace IdentityServerPlus.Plugin.Base.Stores
{
    public abstract class UserStoreBase<TUser> :

        IUserLoginStore<TUser>,
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
        public abstract IQueryable<TUser> Users { get; }

        public abstract Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken);
        public abstract Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken);
        public abstract void Dispose();
        public abstract ValueTask DisposeAsync();
        public abstract Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken);
        public abstract Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken);

        #region Sets

        public virtual async Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            user.AccessFailedCount += 1;
            return user.AccessFailedCount;
        }
        public virtual async Task SetAuthenticatorKeyAsync(TUser user, string key, CancellationToken cancellationToken)
        {
            var authenticatorKey = user.Codes.SingleOrDefault(x => x.Type == ApplicationCodes.CodeType.AuthenticatorKey);
            if (authenticatorKey == null)
            {
                user.Codes.Add(new ApplicationCodes() { Code = key, Type = ApplicationCodes.CodeType.AuthenticatorKey, Expiry = DateTime.MaxValue, Created = DateTime.UtcNow });
            }
            else
            {
                authenticatorKey.Code = key;
                authenticatorKey.Created = DateTime.UtcNow;
            }
        }
        public virtual async Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
        }

        public virtual async Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;
        }

        public virtual async Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.LockoutEnabled = enabled;

        }

        public virtual async Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            user.LockoutEnd = lockoutEnd;
        }

        public virtual async Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.Email = normalizedEmail;
        }

        public virtual async Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.Username = normalizedName;
        }

        public virtual async Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
        }

        public virtual async Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            user.PhoneNumber = phoneNumber;
        }

        public virtual async Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.PhoneNumberConfirmed = confirmed;
        }

        public virtual async Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken)
        {
            user.SecurityStamp = stamp;
        }

        public virtual async Task SetTokenAsync(TUser user, string loginProvider, string name, string value, CancellationToken cancellationToken)
        {
            var provider = user.Providers.SingleOrDefault(x => x.LoginProvider == loginProvider);
            if (provider == null) return;
            if (name == "access_token")
            {
                provider.AccessToken = value;
                provider.AccessTokenExpiry = DateTime.MaxValue;
            }
            else if (name == "id_token")
            {
                provider.IdToken = value;
                provider.IdTokenExpiry = DateTime.MaxValue;
            }

        }

        public virtual async Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.TwoFactorEnabled = enabled;
        }

        public virtual async Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
        {
            user.Username = userName;
        }
        #endregion

        #region Adds
        public virtual async Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            var newClaims = claims.Select(c => new ApplicationClaim(c)).ToList();
            user.Claims.AddRange(newClaims);
        }

        public virtual async Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            var provider = new ApplicationProviderInfo(login) { ProviderLinkedAt = DateTime.UtcNow };
            user.Providers.Add(provider);
        }
        #endregion

        #region Gets
        public virtual async Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            return user.AccessFailedCount;
        }

        public virtual async Task<string> GetAuthenticatorKeyAsync(TUser user, CancellationToken cancellationToken)
        {
            return user.Codes.SingleOrDefault(x => x.Type == ApplicationCodes.CodeType.AuthenticatorKey)?.Code ?? null;
        }

        public virtual async Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken)
        {

            return user.Claims.Select(x => new Claim(x.Type, x.Value, x.ValueType, x.Issuer, x.OriginalIssuer)).ToList();
        }

        public virtual async Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            return user.Email;
        }

        public virtual async Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            return user.EmailConfirmed;
        }

        public virtual async Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            return user.LockoutEnabled;
        }

        public virtual async Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken)
        {
            return user.LockoutEnd;
        }

        public virtual async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken)
        {
            return user.Providers.Cast<UserLoginInfo>().ToList();
        }

        public virtual async Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            return user.Email;
        }

        public virtual async Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            return user.Username;
        }

        public virtual async Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
        {
            return user.PasswordHash;
        }

        public virtual async Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken)
        {
            return user.PhoneNumber;
        }

        public virtual async Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            return user.PhoneNumberConfirmed;
        }

        public virtual async Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken)
        {
            return user.SecurityStamp;
        }

        public virtual async Task<string> GetTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            var provider = user.Providers.SingleOrDefault(x => x.LoginProvider == loginProvider);
            if (provider == null) return null;
            if (name == "access_token")
            {
                return provider.AccessToken;
            }
            else if (name == "id_token")
            {
                return provider.IdToken;
            }
            return null;
        }

        public virtual async Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            return user.TwoFactorEnabled;
        }

        public virtual async Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
        {
            return user.Id.ToString();
        }

        public virtual async Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            return user.Username;
        }

        public virtual async Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
        {
            return !string.IsNullOrWhiteSpace(user.PasswordHash);
        }
        public virtual async Task<bool> RedeemCodeAsync(TUser user, string code, CancellationToken cancellationToken)
        {
            return user.Codes.Any(x => x.Type == ApplicationCodes.CodeType.VerificationCode && x.Code == code);
        }
        public virtual async Task<int> CountCodesAsync(TUser user, CancellationToken cancellationToken)
        {
            return user.Codes.Count(x => x.Type == ApplicationCodes.CodeType.VerificationCode);
        }

        #endregion

        #region Removes
        public virtual async Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            user.Claims.RemoveAll(x => claims.Any(c => c.Type == x.Type && c.Value == x.Value && c.Issuer == x.Issuer));
        }

        public virtual async Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            user.Providers.RemoveAll(x => x.LoginProvider == loginProvider && x.ProviderKey == providerKey);
        }

        public virtual async Task RemoveTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            var provider = user.Providers.SingleOrDefault(x => x.LoginProvider == loginProvider);
            if (provider == null) return;

            if (name == "access_token")
            {
                provider.AccessToken = null;
                provider.AccessTokenExpiry = DateTime.MinValue;
            }
            else if (name == "id_token")
            {
                provider.IdToken = null;
                provider.IdTokenExpiry = DateTime.MinValue;
            }

        }

        public virtual async Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            await RemoveClaimsAsync(user, new[] { claim }, cancellationToken);
            await AddClaimsAsync(user, new[] { claim }, cancellationToken);
        }

        public virtual async Task ReplaceCodesAsync(TUser user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
        {
            user.Codes.RemoveAll(x => recoveryCodes.Any(c => c == x.Code));
        }

        public virtual async Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            user.AccessFailedCount = 0;
        }

        #endregion

        public abstract Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken);
        public abstract Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken);
        public abstract Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken);
        public abstract Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken);

    }
}
