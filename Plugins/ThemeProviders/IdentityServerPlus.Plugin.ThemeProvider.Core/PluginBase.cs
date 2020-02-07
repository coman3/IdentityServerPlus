using IdentityServerPlus.Plugin.Base.Models;
using IdentityServerPlus.Plugin.Base.Structures;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace IdentityServerPlus.Plugin.ThemeProvider.Core
{
    public class CoreThemeProviderPlugin : PluginBase
    {
        public override Guid Id => new Guid("5b1c16fc-2b22-4820-8ccc-3b26b7a3e224");

        public override DateTime LastUpdated => new DateTime(2020, 2, 7);

        private IConfiguration Configuration { get; }
        public CoreThemeProviderPlugin() : base("Core Theme", "0.0.0.1")
        {
            Configuration = null;
        }


        public override IEnumerable<ProviderItem> GetProviderTypesAndArguments()
        {
            yield return new ProviderItem<CoreThemeProvider>();
        }
    }
}
