using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Interfaces;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace IdentityServer.Stores
{
    public class MongoPersistedGrantStore : IPersistedGrantStore
    {
        protected IRepository _dbRepository;

        public MongoPersistedGrantStore(IRepository repository)
        {
            _dbRepository = repository;
        }

        public Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            var result = _dbRepository.Where<IdentityServer.Models.PersistedGrant>(i => i.SubjectId == subjectId);
            return Task.FromResult(result.AsEnumerable().Cast<PersistedGrant>());
        }

        public Task<PersistedGrant> GetAsync(string key)
        {
            var result = _dbRepository.Single<IdentityServer.Models.PersistedGrant>(i => i.Key == key);
            return Task.FromResult((PersistedGrant) result);
        }

        public Task RemoveAllAsync(string subjectId, string clientId)
        {
            _dbRepository.Delete<IdentityServer.Models.PersistedGrant>(i => i.SubjectId == subjectId && i.ClientId == clientId);
            return Task.FromResult(0);
        }

        public Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            _dbRepository.Delete<IdentityServer.Models.PersistedGrant>(i => i.SubjectId == subjectId && i.ClientId == clientId && i.Type == type);
            return Task.FromResult(0);
        }

        public Task RemoveAsync(string key)
        {
            _dbRepository.Delete<IdentityServer.Models.PersistedGrant>(i => i.Key == key);
            return Task.FromResult(0);
        }

        public Task StoreAsync(PersistedGrant grant)
        {
            _dbRepository.Add<PersistedGrant>(grant);
            return Task.FromResult(0);
        }
    }
}