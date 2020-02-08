using IdentityServerPlus.Plugin.Base.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServerPlus.Plugin.Base.Interfaces
{
    public interface IIdentityServerProvider : IPluginProvider
    {
        IdentityProviderType Type { get; }

        IIdentityServerBuilder Build(IIdentityServerBuilder builder);
    }
}