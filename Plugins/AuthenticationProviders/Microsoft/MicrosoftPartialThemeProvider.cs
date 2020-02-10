using IdentityServerPlus.Plugin.Base.Interfaces;
using IdentityServerPlus.Plugin.Base.Structures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace IdentityServerPlus.Plugin.AuthenticationProvider.Microsoft
{
    public class MicrosoftPartialThemeProvider : ThemeProviderBase
    {
        public override string Name => "Microsoft Partial Theme Provider";

        public override string Description => "Provides microsoft themed partials for external login";

        public override int Index => -1;

    }
}