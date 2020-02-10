using IdentityServerPlus.Plugin.Base.Models;
using IdentityServerPlus.Plugin.Base.Structures;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace IdentityServerPlus.Plugin.ThemeProvider.Empty
{
    public class CoreThemeProviderPlugin : PluginBase
    {
        public override Guid Id => new Guid();

        public override DateTime LastUpdated => new DateTime(2020, 2, 7);

        private IConfiguration Configuration { get; }
        public CoreThemeProviderPlugin() : base("Empty Theme", "0.0.0.1")
        {
            Configuration = null;
        }


        public override IEnumerable<ProviderItem> GetProviderTypesAndArguments()
        {
            yield return new ProviderItem<CoreThemeProvider>();
        }
    }
}
