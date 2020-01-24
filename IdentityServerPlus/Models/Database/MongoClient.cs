using IdentityServer4.Models;
using MongoDB.Bson;

namespace IdentityServer.Models {
    public class Client : IdentityServer4.Models.Client {
        public ObjectId Id { get; set; }

    }

    public class IdentityResource : IdentityServer4.Models.IdentityResource {
        public ObjectId Id { get; set; }

    }

    public class PersistedGrant : IdentityServer4.Models.PersistedGrant {
        public ObjectId Id { get; set; }

    }
}