using IdentityServerPlus.Plugin.Base.Interfaces;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace IdentityServerPlus.Plugin.Base.Structures
{
    public abstract class ThemeProviderBase : IThemeProvider
    {
        public abstract int Index { get; }
        public abstract string Name { get; }
        public abstract string Description { get; }

        public virtual void ConfigureRazor(IMvcBuilder builder)
        {
            foreach (var ass in GetViewAssemblies())
            {
                builder.PartManager.ApplicationParts.Insert(0, new AssemblyPart(ass));
                builder.PartManager.ApplicationParts.Insert(0, new CompiledRazorAssemblyPart(ass));
            }
        }
        public abstract Assembly[] GetViewAssemblies();
    }
}
