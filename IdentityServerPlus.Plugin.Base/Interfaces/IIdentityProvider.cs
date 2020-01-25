using IdentityServerPlus.Plugin.Base.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServerPlus.Plugin.Base.Interfaces
{
    public interface IIdentityProvider
    {
        string Name { get; }
        IdentityProviderType Type { get; }

        IdentityBuilder Build(IdentityBuilder builder);
    }
}