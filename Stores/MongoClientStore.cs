using System.Threading.Tasks;
using IdentityServer.Interfaces;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace IdentityServer.Stores
{
    public class MongoClientStore : IClientStore
    {
        protected IRepository _dbRepository;

        public MongoClientStore(IRepository repository)
        {
            _dbRepository = repository;
        }
        public Task<Client> FindClientByIdAsync(string clientId)
        {
            var client = _dbRepository.Single<Client>(x=> x.ClientId == clientId);
            return Task.FromResult(client);
        }
    }
}