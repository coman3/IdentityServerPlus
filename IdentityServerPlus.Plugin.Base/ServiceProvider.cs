using IdentityServerPlus.Plugin.Base.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServerPlus.Plugin.Base {
    public abstract class ServiceProvider : IPluginProvider {

        public virtual void ConfigureServices(IServiceCollection services) {

        }
        public abstract IIdentityServerBuilder Build(IIdentityServerBuilder builder);

        public abstract ProviderType GetProviderType();
    }
}