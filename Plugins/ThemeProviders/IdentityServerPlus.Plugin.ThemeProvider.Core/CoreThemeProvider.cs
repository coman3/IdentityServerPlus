﻿using IdentityServerPlus.Plugin.Base.Interfaces;
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

        public override string Description => "Provides the implementation of most authentication routes";

        public override int Index => 0;

    }
}