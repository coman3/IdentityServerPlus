using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServerPlus.Plugin.Base.Interfaces
{
    public interface IAuthenticationProvider
    {
        string Scheme { get; }
        string FriendlyName { get; }

        AuthenticationBuilder Build(AuthenticationBuilder builder);
    }
}
