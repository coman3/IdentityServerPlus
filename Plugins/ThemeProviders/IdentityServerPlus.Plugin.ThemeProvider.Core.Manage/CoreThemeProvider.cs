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

namespace IdentityServerPlus.Plugin.ThemeProvider.Core.Manage
{
    public class CoreThemeProvider : ThemeProviderBase
    {
        public override string Name => "Core Manage Theme Provider";

        public override string Description => "Provides the implementation of an account management portal ";

        public override int Index => 0;
    }
}