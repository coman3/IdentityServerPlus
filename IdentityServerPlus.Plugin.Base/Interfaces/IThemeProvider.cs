using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace IdentityServerPlus.Plugin.Base.Interfaces
{
    public interface IThemeProvider : IPluginProvider
    {
        /// <summary>
        /// Specifies the order in which this ThemeProvider will be loaded (higher first, lower later)
        /// </summary>
        int Index { get; }

        void ConfigureRazor(IMvcBuilder builder);

        Assembly[] GetViewAssemblies();
    }
}