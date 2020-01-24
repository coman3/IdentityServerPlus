using System;
using System.Collections.Generic;
using System.Linq;
namespace IdentityServer.Models.Account
{
    public class LoginViewModel
    {
        public bool AllowRememberLogin { get; set; } = false;
        public bool EnableLocalLogin { get; set; } = false;

        public IEnumerable<ExternalProvider> ExternalProviders { get; set; } = Enumerable.Empty<ExternalProvider>();
        public IEnumerable<ExternalProvider> VisibleExternalProviders => ExternalProviders.Where(x => !String.IsNullOrWhiteSpace(x.DisplayName));

        public bool IsExternalLoginOnly => EnableLocalLogin == false && ExternalProviders?.Count() == 1;
        public string ExternalLoginScheme => IsExternalLoginOnly ? ExternalProviders?.SingleOrDefault()?.AuthenticationScheme : null;
    }
}