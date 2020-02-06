using IdentityServer4.Models;
using IdentityServer4.Stores;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServerPlus.Plugin.Base.Stores
{
    public abstract class UserResourceStoreBase : IPersistedGrantStore, IUserConsentStore
    {
        public abstract Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId);
        public abstract Task<PersistedGrant> GetAsync(string key);
        public abstract Task<Consent> GetUserConsentAsync(string subjectId, string clientId);
        public abstract Task RemoveAllAsync(string subjectId, string clientId);
        public abstract Task RemoveAllAsync(string subjectId, string clientId, string type);
        public abstract Task RemoveAsync(string key);
        public abstract Task RemoveUserConsentAsync(string subjectId, string clientId);
        public abstract Task StoreAsync(PersistedGrant grant);
        public abstract Task StoreUserConsentAsync(Consent consent);
    }
}
