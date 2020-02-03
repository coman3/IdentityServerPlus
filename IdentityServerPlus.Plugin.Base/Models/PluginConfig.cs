
using System.Collections.Generic;

namespace IdentityServerPlus.Plugin.Base.Models
{
    public class PluginConfig
    {
        public string Assembly { get; set; }
        public List<PluginConfigurationSource> ConfigurationSources { get; set; }
    }

    public abstract class PluginConfigurationSource
    {
        public ConfigurationProviderType ProviderType { get; set; }

    }

    public sealed class FileConfigSource : PluginConfigurationSource
    {
        /// <summary>
        /// Files to load in order, lower index first
        /// </summary>
        public string[] FileNames { get; set; }
    }

    public enum ConfigurationProviderType
    {
        AzureKeyVault,
        Files,
        UserSecrets,
        EnvironmentVariables
    }
}