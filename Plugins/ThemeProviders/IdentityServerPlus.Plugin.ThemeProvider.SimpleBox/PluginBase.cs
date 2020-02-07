using IdentityServerPlus.Plugin.Base.Models;
using IdentityServerPlus.Plugin.Base.Structures;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace IdentityServerPlus.Plugin.ThemeProvider.SimpleBox
{
    public class SimpleBoxThemeProviderPlugin : PluginBase
    {
        public override Guid Id => new Guid("5b1c16fc-2b22-4820-8ccc-3b26b7a3e224");

        public override DateTime LastUpdated => new DateTime(2020, 2, 7);

        private IConfiguration Configuration { get; }
        public SimpleBoxThemeProviderPlugin() : base("SimpleBox Theme", "0.0.0.1")
        {
            Configuration = null;
        }


        public override IEnumerable<ProviderItem> GetProviderTypesAndArguments()
        {
            yield return new ProviderItem<SimpleBoxThemeProvider>();
        }
    }
}
