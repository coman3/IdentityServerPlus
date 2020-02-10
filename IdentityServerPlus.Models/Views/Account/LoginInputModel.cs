using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace IdentityServer.Models.Views
{
    public class AuthenticateViewModel {
        [Required]
        public virtual string Username { get; set; }
        [Required]
        public virtual string Password { get; set; }
        public bool RememberLogin { get; set; }

        public string ReturnUrl { get; set; }

        public bool AllowRememberLogin { get; set; } = false;
        public bool EnableLocalLogin { get; set; } = false;

        public IEnumerable<ExternalProvider> ExternalProviders { get; set; } = Enumerable.Empty<ExternalProvider>();
        public IEnumerable<ExternalProvider> VisibleExternalProviders => ExternalProviders.Where(x => !String.IsNullOrWhiteSpace(x.DisplayName));

        public bool IsExternalLoginOnly => EnableLocalLogin == false && ExternalProviders?.Count() == 1;
        public string ExternalLoginScheme => IsExternalLoginOnly ? ExternalProviders?.SingleOrDefault()?.AuthenticationScheme : null;


        public AuthenticateViewModel()
        {

        }
        public AuthenticateViewModel(AuthenticateViewModel model)
        {
            Username = model.Username;
            Password = model.Password;
            RememberLogin = model.RememberLogin;
            ReturnUrl = model.ReturnUrl;
            AllowRememberLogin = model.AllowRememberLogin;
            EnableLocalLogin = model.EnableLocalLogin;

            ExternalProviders = model.ExternalProviders;
        }
    }
}