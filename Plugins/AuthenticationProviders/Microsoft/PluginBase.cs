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
        private IConfiguration Configuration { get; }

        public MicrosoftAuthenticationProviderPlugin(IConfiguration configuration) : base("Microsoft Authentication Provider", "0.0.0.1")
        {
            Configuration = configuration;
        }

        public override IEnumerable<ProviderItem> GetProviderTypesAndArguments()
        {
            yield return new ProviderItem<MicrosoftAuthenticationProvider>(Configuration);
        }
    }
}
