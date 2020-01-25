using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServerPlus.Plugin.Base.Interfaces
{
    public interface IServiceConfigurationProvider
    {
        void ConfigureServices(IServiceCollection services);
    }
}
