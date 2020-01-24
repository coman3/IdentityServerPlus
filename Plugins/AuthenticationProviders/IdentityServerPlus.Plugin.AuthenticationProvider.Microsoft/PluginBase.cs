using IdentityServerPlus.Plugin.Base;
using IdentityServerPlus.Plugin.Base.Interfaces;
using System;

namespace IdentityServerPlus.Plugin.AuthenticationProvider.Microsoft
{
    public class MicrosoftAuthenticationProviderPlugin : PluginBase
    {
        public override Guid Id => new Guid("34e2d95a-13c3-4c69-bed9-0013ef30cef4");

        public override DateTime LastUpdated => new DateTime(2020, 1, 24);

        public MicrosoftAuthenticationProviderPlugin() : base("Microsoft Authentication Provider", "0.0.0.1", "MicrosoftAuthentication")
        {

        }

        public override IPluginProvider[] BuildProviders()
        {
            return new[]
            {
                new MicrosoftAuthenticationProvider(),
            };
        }
    }
}
