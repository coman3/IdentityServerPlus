using System;
using System.Threading.Tasks;
using IdentityServer.Models;
using IdentityServerPlus.Plugin.Base.Interfaces;
using Microsoft.AspNetCore.Authentication;

namespace IdentityServerPlus.Plugin.Base.Structures
{
    public abstract class AuthenticationProvider : IPluginProvider, IAuthenticationProvider
    {
        public string FriendlyName { get; set; }

        public string Scheme { get; set; }

        public string Name => FriendlyName + " login provider";

        public abstract string Description { get; }

        protected AuthenticationProvider(string friendlyName, string scheme)
        {
            this.FriendlyName = friendlyName;
            this.Scheme = scheme;
        }

        public abstract AuthenticationBuilder Build(AuthenticationBuilder builder);
        public abstract string GetProviderId(AuthenticateResult authResult);
        public abstract ApplicationUser ProvisionUser(AuthenticateResult result);
        public virtual Task UpdateUserAsync(ApplicationUser user, AuthenticateResult result)
        {
            return Task.CompletedTask;
        }
    }
}