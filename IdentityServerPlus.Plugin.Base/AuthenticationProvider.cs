using System;
using IdentityServerPlus.Plugin.Base.Interfaces;
using Microsoft.AspNetCore.Authentication;

namespace IdentityServerPlus.Plugin.Base {
    public abstract class AuthenticationProvider : IPluginProvider {
        public string ProviderName { get; set; }

        public string ProviderScheme { get; set; }

        protected AuthenticationProvider(string providerName, string providerScheme)
        {
            this.ProviderName = providerName;
            this.ProviderScheme = providerScheme;
        }

        public abstract AuthenticationBuilder Build(AuthenticationBuilder builder);

        public virtual ProviderType GetProviderType()
        {
            return ProviderType.AuthenticationProvider;
        }
    }
}