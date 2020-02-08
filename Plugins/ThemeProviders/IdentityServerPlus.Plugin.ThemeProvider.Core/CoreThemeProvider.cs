using IdentityServerPlus.Plugin.Base.Interfaces;
using IdentityServerPlus.Plugin.Base.Structures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace IdentityServerPlus.Plugin.ThemeProvider.Core
{
    public class CoreThemeProvider : ThemeProviderBase
    {
        public override string Name => "Core Theme Provider";

        public override string Description => "Provides a simple basic theme to the application that can be customized";

        public override int Index => 0;

        public override Assembly[] GetViewAssemblies()
        {
            var currentAssembly = Assembly.GetAssembly(typeof(CoreThemeProvider));
            return new[]
            {
                Assembly.LoadFrom(Path.Combine(new FileInfo(currentAssembly.Location).DirectoryName, "IdentityServerPlus.Plugin.ThemeProvider.Core.Views.dll")),
                currentAssembly
        };
        }
    }
}