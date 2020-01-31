using IdentityServer.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServerPlus.Plugin.Base.Interfaces
{
    public interface IAuthenticationProvider
    {
        string Scheme { get; }
        string FriendlyName { get; }

        AuthenticationBuilder Build(AuthenticationBuilder builder);

        string GetProviderId(AuthenticateResult authResult);

        ApplicationUser ProvisionUser(AuthenticateResult authResult);
        Task UpdateUserAsync(ApplicationUser user, AuthenticateResult result);

    }
}
