using IdentityServer4.Models;
using IdentityServer4.Stores;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServerPlus.Plugin.Base.Stores
{
    public abstract class UserResourceStoreBase : IPersistedGrantStore, IReferenceTokenStore, IRefreshTokenStore, IAuthorizationCodeStore, IDeviceFlowStore, IUserConsentStore
    {
        public abstract Task<DeviceCode> FindByDeviceCodeAsync(string deviceCode);
        public abstract Task<DeviceCode> FindByUserCodeAsync(string userCode);
        public abstract Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId);
        public abstract Task<PersistedGrant> GetAsync(string key);
        public abstract Task<AuthorizationCode> GetAuthorizationCodeAsync(string code);
        public abstract Task<Token> GetReferenceTokenAsync(string handle);
        public abstract Task<RefreshToken> GetRefreshTokenAsync(string refreshTokenHandle);
        public abstract Task<Consent> GetUserConsentAsync(string subjectId, string clientId);

        public abstract Task RemoveAllAsync(string subjectId, string clientId);
        public abstract Task RemoveAllAsync(string subjectId, string clientId, string type);
        public abstract Task RemoveAsync(string key);
        public abstract Task RemoveAuthorizationCodeAsync(string code);
        public abstract Task RemoveByDeviceCodeAsync(string deviceCode);
        public abstract Task RemoveReferenceTokenAsync(string handle);
        public abstract Task RemoveReferenceTokensAsync(string subjectId, string clientId);
        public abstract Task RemoveRefreshTokenAsync(string refreshTokenHandle);
        public abstract Task RemoveRefreshTokensAsync(string subjectId, string clientId);
        public abstract Task RemoveUserConsentAsync(string subjectId, string clientId);

        public abstract Task StoreAsync(PersistedGrant grant);
        public abstract Task<string> StoreAuthorizationCodeAsync(AuthorizationCode code);
        public abstract Task StoreDeviceAuthorizationAsync(string deviceCode, string userCode, DeviceCode data);
        public abstract Task<string> StoreReferenceTokenAsync(Token token);
        public abstract Task<string> StoreRefreshTokenAsync(RefreshToken refreshToken);
        public abstract Task StoreUserConsentAsync(Consent consent);
        public abstract Task UpdateByUserCodeAsync(string userCode, DeviceCode data);
        public abstract Task UpdateRefreshTokenAsync(string handle, RefreshToken refreshToken);
    }
}
