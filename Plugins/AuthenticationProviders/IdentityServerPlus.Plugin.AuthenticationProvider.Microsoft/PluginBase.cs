using IdentityServerPlus.Plugin.Base;
using IdentityServerPlus.Plugin.Base.Interfaces;
using IdentityServerPlus.Plugin.Base.Models;
using IdentityServerPlus.Plugin.Base.Structures;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace IdentityServerPlus.Plugin.AuthenticationProvider.Microsoft
{
    public class MicrosoftAuthenticationProviderPlugin : PluginBase
    {
        public override Guid Id => new Guid("34e2d95a-13c3-4c69-bed9-0013ef30cef4");

        public override DateTime LastUpdated => new DateTime(2020, 1, 24);

        public MicrosoftAuthenticationProviderPlugin() : base("Microsoft Authentication Provider", "0.0.0.1")
        {

        }

        public override IEnumerable<ProviderItem> GetProviderTypesAndArguments(IConfiguration configuration)
        {
            yield return new ProviderItem<MicrosoftAuthenticationProvider>();
        }

        //public override IEnumerable<PluginConfigurationItem> GetPluginConfigurations()
        //{
        //    yield return new PluginConfigurationItem<MicrosoftAuthenticationOptions>();
        //}
    }
}
