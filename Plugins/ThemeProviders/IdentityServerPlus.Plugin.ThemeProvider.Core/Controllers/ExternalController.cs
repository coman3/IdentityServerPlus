using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer;
using IdentityServer.Models;
using IdentityServer.Models.Attributes;
using IdentityServer.Models.Views;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Test;
using IdentityServerPlus.Plugin.Base.Interfaces;
using IdentityServerPlus.Plugin.Base.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IdentityServerPlus.Plugin.ThemeProvider.Core.Controllers
{

    [SecurityHeaders]
    [AllowAnonymous]
    public class ExternalController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IEventService _events;
        private readonly ILogger<ExternalController> _logger;
        private readonly PluginManager _pluginManager;

        public ExternalController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IEventService events,
            ILogger<ExternalController> logger,
            PluginManager pluginManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _clientStore = clientStore;
            _events = events;
            _logger = logger;
            _pluginManager = pluginManager;
        }

        /// <summary>
        /// initiate roundtrip to external authentication provider
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Challenge(string provider, string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl)) returnUrl = "~/";

            // validate returnUrl - either it is a valid OIDC URL or back to a local page
            if (Url.IsLocalUrl(returnUrl) == false && _interaction.IsValidReturnUrl(returnUrl) == false)
            {
                // user might have clicked on a malicious link - should be logged
                throw new Exception("invalid return URL");
            }

            // start challenge and roundtrip the return URL and scheme 
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(Callback)),
                Items = { { "returnUrl", returnUrl },
                { "scheme", provider },
                }
            };

            return Challenge(props, provider);

        }

        /// <summary>
        /// Post processing of external authentication
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Callback()
        {
            // read external identity from the temporary cookie
            var result = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
            if (result?.Succeeded != true)
            {
                throw new Exception("External authentication error");
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                var externalClaims = result.Principal.Claims.Select(c => $"{c.Type}: {c.Value}");
                _logger.LogDebug("External claims: {@claims}", externalClaims);
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            

            // lookup our user and external provider info
            var (user, authProvider, providerUserId, claims) = await FindUserFromExternalProviderAsync(result);
            
            if (user == null)
            {
                // TODO: Enable support for custom workflows
                user = await AutoProvisionUserAsync(authProvider, providerUserId, result);
            }else
            {
                await authProvider.UpdateUserAsync(user, result);
            }

            var additionalLocalClaims = new List<Claim>();
            var localSignInProps = new AuthenticationProperties();
            ProcessLoginCallbackForOidc(result, additionalLocalClaims, localSignInProps);
            //ProcessLoginCallbackForWsFed(result, additionalLocalClaims, localSignInProps);
            //ProcessLoginCallbackForSaml2p(result, additionalLocalClaims, localSignInProps);

            // issue authentication cookie for user
            // we must issue the cookie maually, and can't use the SignInManager because
            // it doesn't expose an API to issue additional claims from the login workflow
            var principal = await _signInManager.CreateUserPrincipalAsync(user);
            additionalLocalClaims.AddRange(principal.Claims);

            var isuser = new IdentityServerUser(user.Id.ToString())
            {
                IdentityProvider = authProvider.Scheme,
                AdditionalClaims = additionalLocalClaims
            };

            await HttpContext.SignInAsync(isuser, localSignInProps);

            // delete temporary cookie used during external authentication
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // retrieve return URL
            var returnUrl = result.Properties.Items["returnUrl"] ?? "~/";

            // check if external login is in the context of an OIDC request
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);

            await _userManager.UpdateAsync(user);
            await _events.RaiseAsync(new UserLoginSuccessEvent(authProvider.Scheme, providerUserId, user.Id.ToString(), user.Email, true, context?.ClientId));
            

            if (context != null)
            {
                var client = await _clientStore.FindClientByIdAsync(context.ClientId);
                if (client.RequirePkce)
                {
                    // if the client is PKCE then we assume it's native, so this change in how to
                    // return the response is for better UX for the end user.
                    return View("Redirect", new RedirectViewModel() { RedirectUrl = returnUrl });
                }
            }

            return Redirect(returnUrl);
        }

        private async Task<(ApplicationUser user, IAuthenticationProvider authProvider, string providerUserId, IEnumerable<Claim> claims)> FindUserFromExternalProviderAsync(AuthenticateResult result)
        {
            var externalUser = result.Principal;
            var provider = result.Properties.Items["scheme"];
            var authProvider = _pluginManager.GetProviders<IAuthenticationProvider>().SingleOrDefault(x => x.Scheme == provider);
            var externalId = authProvider.GetProviderId(result);
            var user = await _userManager.FindByLoginAsync(provider, externalId);

            return (user, authProvider, externalId, result.Principal.Claims);
        }

        private async Task<ApplicationUser> AutoProvisionUserAsync(IAuthenticationProvider authProvider, string providerUserId, AuthenticateResult result)
        {
            
            var user = authProvider.ProvisionUser(result);

            var identityResult = await _userManager.CreateAsync(user);

            if (!identityResult.Succeeded)
                throw new Exception(identityResult.Errors.First().Description);           

            identityResult = await _userManager.AddLoginAsync(user, new UserLoginInfo(authProvider.Scheme, providerUserId, authProvider.Scheme));
            if (!identityResult.Succeeded) 
                throw new Exception(identityResult.Errors.First().Description);

            await authProvider.UpdateUserAsync(user, result);

            return user;
        }

        private void ProcessLoginCallbackForOidc(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
        {
            // if the external system sent a session id claim, copy it over
            // so we can use it for single sign-out
            var sid = externalResult.Principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
            if (sid != null)
            {
                localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
            }
            //if the external provider issued an id_token, we'll keep it for signout
            var id_token = externalResult.Properties.GetTokenValue("id_token");
            if (id_token != null)
            {
                localSignInProps.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = id_token } });
            }
        }
    }
}