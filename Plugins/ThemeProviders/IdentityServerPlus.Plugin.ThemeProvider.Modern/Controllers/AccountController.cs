using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer.Models.Attributes;
using IdentityServer.Models.Views;
using IdentityServer.Models;
using IdentityServer.Models.Config;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace IdentityServer.Controllers
{
    /// <summary>
    /// This sample controller implements a typical login/logout/provision workflow for local and external accounts.
    /// The login service encapsulates the interactions with the user data store. This data store is in-memory only and cannot be used for production!
    /// The interaction service provides a way for the UI to communicate with identityserver for validation and context retrieval
    /// </summary>
    [SecurityHeaders]
    [AllowAnonymous]
    public class AccountController : Controller
    {

        private IdentityServerPlus.Plugin.ThemeProvider.Core.Controllers.AccountController _accountController { get; }

        public AccountController(IdentityServerPlus.Plugin.ThemeProvider.Core.Controllers.AccountController accountController)
        {
            _accountController = accountController;
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            ModelState.Clear();
            model.ConfirmPassword = model.Password;            
            TryValidateModel(model);

            return await _accountController.Register(model);
        }
    }
}
