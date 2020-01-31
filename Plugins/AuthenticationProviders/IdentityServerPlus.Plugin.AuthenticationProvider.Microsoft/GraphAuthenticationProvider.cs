using Microsoft.Graph;
using System.Net.Http;
using System.Threading.Tasks;

namespace IdentityServerPlus.Plugin.AuthenticationProvider.Microsoft
{
    class GraphAuthenticationProvider : IAuthenticationProvider
    {
        public GraphAuthenticationProvider(string token)
        {
            Token = token;
        }

        public string Token { get; set; }
        
        public Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
            return Task.CompletedTask;
        }
    }
}
