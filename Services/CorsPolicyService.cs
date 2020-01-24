using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Interfaces;
using IdentityServer.Models;
using IdentityServer4.Services;

namespace IdentityServer.Services {
    public class CorsPolicyService : ICorsPolicyService {

        private IRepository _dbRepository;
        private List<string> AllowedOrigins { get; set; }
        public CorsPolicyService(IRepository repository) {
            _dbRepository = repository;
        }
        public Task<bool> IsOriginAllowedAsync(string origin) {
            if (AllowedOrigins == null) {
                var clients = _dbRepository.All<Client>();
                AllowedOrigins = new List<string>();
                foreach (var client in clients) {                    
                    AllowedOrigins.AddRange(client.AllowedCorsOrigins.Select(x => x.ToLower()));
                }
            }
            return Task.FromResult(AllowedOrigins.Contains(origin.ToLower()));
        }
    }
}