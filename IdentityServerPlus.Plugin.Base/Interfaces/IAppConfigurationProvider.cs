using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServerPlus.Plugin.Base.Interfaces
{
    public interface IAppConfigurationProvider
    {

        void Configure(IApplicationBuilder app);

    }
}
