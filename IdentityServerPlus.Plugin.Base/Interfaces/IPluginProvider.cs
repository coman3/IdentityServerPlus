namespace IdentityServerPlus.Plugin.Base.Interfaces {
    public interface IPluginProvider {

        ProviderType GetProviderType();

    }

    public enum ProviderType {
        DatabaseProvider,
        AuthenticationProvider,
        ThemeProvider,
        ControllerProvider,
    }
}