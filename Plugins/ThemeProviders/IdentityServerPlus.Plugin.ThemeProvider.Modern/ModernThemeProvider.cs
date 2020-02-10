using IdentityServerPlus.Plugin.Base.Interfaces;
using IdentityServerPlus.Plugin.Base.Models;
using IdentityServerPlus.Plugin.Base.Structures;
using IdentityServerPlus.Plugin.ThemeProvider.Modern.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace IdentityServerPlus.Plugin.ThemeProvider.Modern
{
    public class ModernThemeProvider : ThemeProviderBase, IIdentityProvider
    {
        public override string Name => "Modern Theme Provider";

        public override string Description => "Provides a simple modern theme that can be customized via config";

        public override int Index => 2;

        public IdentityProviderType Type => IdentityProviderType.Other;

        public IdentityBuilder Build(IdentityBuilder builder)
        {
            return builder.AddErrorDescriber<CustomIdentityErrorDescriber>();
        }
    }
}