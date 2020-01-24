using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServerPlus.Plugin.AuthenticationProvider.Microsoft
{
    internal class MicrosoftAuthenticationProvider : Base.AuthenticationProvider
    {
        public MicrosoftAuthenticationProvider() : base("Microsoft", "microsoft")
        {
        }

        public override AuthenticationBuilder Build(AuthenticationBuilder builder)
        {
            return builder.AddOpenIdConnect("microsoft", options =>
             {
                 options.SaveTokens = true;
                 options.Scope.Add("profile");
                 options.Scope.Add("User.Read");
                 options.ResponseType = "id_token token";
                 options.ClientId = "7d6d3978-f958-4759-887d-498b52ede1df";
                 options.ClientSecret = "DSBjc:/?V-FBGAfY64t96gm@GG=qfcct";
                 options.MetadataAddress = "https://login.microsoftonline.com/15864aef-67b6-4b0e-8bfe-cd61201a6837/v2.0/.well-known/openid-configuration";
                 options.Authority = "https://login.microsoftonline.com/15864aef-67b6-4b0e-8bfe-cd61201a6837";
             });
        }
    }
}