using IdentityServer.Models;
using IdentityServer4.Models;
using IdentityServerPlus.Plugin.Base.Stores;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServerPlus.Plugin.DatabaseProvider.MongoDB.Stores
{
    class UserResourceStore : UserResourceStoreBase
    {

        private IMongoCollection<Consent> _consents { get; }
        private IMongoCollection<PersistedGrant> _grants { get; }
        //private IMongoCollection<Token> _tokens { get; }

        private ILogger _logger { get; }

        public UserResourceStore(IMongoDatabase applicationDatabase, ILogger<UserResourceStore> logger)
        {
            //TODO: Why the shit does Idendtity server provide a "sql" friendly api in the first place............. 
            //      Replace this 'PersistedGrant' grant shit with a proper, filterable altertative that does not store a serialized json string in my database.
            _grants = applicationDatabase.GetCollection<PersistedGrant>("grants");
            _consents = applicationDatabase.GetCollection<Consent>("consents");
            _logger = logger;
        }

        public async override Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            return await _grants.AsQueryable().Where(x => x.SubjectId == subjectId).ToListAsync();
        }

        public async override Task<PersistedGrant> GetAsync(string key)
        {
            return await _grants.AsQueryable().Where(x => x.Key == key).SingleOrDefaultAsync();
        }

      
        public async override Task<Consent> GetUserConsentAsync(string subjectId, string clientId)
        {
            return await _consents.AsQueryable().Where(x => x.SubjectId == subjectId && x.ClientId == clientId).SingleOrDefaultAsync();
        }

        public async override Task RemoveAllAsync(string subjectId, string clientId)
        {
            await _grants.DeleteManyAsync(
                Builders<PersistedGrant>.Filter.And(
                    Builders<PersistedGrant>.Filter.Eq(x => x.SubjectId, subjectId),
                    Builders<PersistedGrant>.Filter.Eq(x => x.ClientId, clientId)
            ));
        }

        public async override Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            await _grants.DeleteManyAsync(
                Builders<PersistedGrant>.Filter.And(
                    Builders<PersistedGrant>.Filter.Eq(x => x.SubjectId, subjectId),
                    Builders<PersistedGrant>.Filter.Eq(x => x.ClientId, clientId),
                    Builders<PersistedGrant>.Filter.Eq(x => x.Type, type)
            ));
        }

        public async override Task RemoveAsync(string key)
        {
            await _grants.DeleteOneAsync(Builders<PersistedGrant>.Filter.Eq(x => x.Key, key));
        }

     

        public async override Task RemoveUserConsentAsync(string subjectId, string clientId)
        {
            await _consents.DeleteManyAsync(
                Builders<Consent>.Filter.And(
                    Builders<Consent>.Filter.Eq(x => x.SubjectId, subjectId),
                    Builders<Consent>.Filter.Eq(x => x.ClientId, clientId)
            ));
        }

        public async override Task StoreAsync(PersistedGrant grant)
        {
            await _grants.InsertOneAsync(grant);
        }

        public async override Task StoreUserConsentAsync(Consent consent)
        {
            await _consents.InsertOneAsync(consent);
        }

    }
}
