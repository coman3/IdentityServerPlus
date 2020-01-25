using IdentityServerPlus.Plugin.Base.Interfaces;
using IdentityServerPlus.Plugin.Base.Models;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServerPlus.Plugin.Base.Structures
{
    public abstract class IdentityProvider : IPluginProvider, IIdentityServerProvider, IServiceConfigurationProvider
    {
        public string Name { get; set; }
        public IdentityProviderType Type { get; set; }

        public string Description { get; set; }

        public IdentityProvider(string name, string desc)
        {
            Name = name;
            Description = desc;
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {

        }
        public abstract IIdentityServerBuilder Build(IIdentityServerBuilder builder);
    }
}