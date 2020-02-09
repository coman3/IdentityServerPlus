using IdentityServerPlus.Plugin.Base.Interfaces;
using IdentityServerPlus.Plugin.Base.Structures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace IdentityServerPlus.Plugin.ThemeProvider.Empty
{
    public class CoreThemeProvider : ThemeProviderBase
    {
        public override string Name => "Empty Theme Provider";

        public override string Description => "Provides a simple basic theme to the application that can be customized";

        public override int Index => 10;
    }
}