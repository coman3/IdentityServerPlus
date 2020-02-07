using IdentityServerPlus.Plugin.Base.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace IdentityServerPlus.Plugin.ThemeProvider.Core
{
    public class CoreThemeProvider : IThemeProvider, IPluginProvider
    {
        public string Name => "Core Theme Provider";

        public string Description => "Provides a simple basic theme to the application that can be customized";

        public int Index => 0;

        public void ConfigureRazor(IMvcBuilder builder)
        {
            foreach (var ass in GetViewAssemblies())
            {
                builder.PartManager.ApplicationParts.Insert(0, new AssemblyPart(ass));
            }
            builder.AddControllersAsServices();
        }

        public Assembly[] GetViewAssemblies()
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