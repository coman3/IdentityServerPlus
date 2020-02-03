using IdentityServerPlus.Plugin.Base.Interfaces;
using IdentityServerPlus.Plugin.Base.Models;
using IdentityServerPlus.Plugin.Base.Structures;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace IdentityServerPlus.Plugin.Base.Models
{
    public sealed class PluginInstance
    {
        public Assembly Assembly { get; }
        public Type Type { get; }
        public string AssemblyLocation { get; internal set; }
        public bool Activated => Instance != null;
        public bool Injected { get; private set; }

        public List<IPluginProvider> Providers { get; private set; }

        public PluginConfig PluginConfig { get; }

        public PluginBase Instance { get; private set; }

        public PluginInstance(Assembly assembly, Type type, PluginConfig config)
        {
            Assembly = assembly;
            Type = type;
            PluginConfig = config;
        }

        public void Activate()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder();
            foreach (var configItem in PluginConfig.ConfigurationSources)
            {
                switch (configItem.ProviderType)
                {
                    case ConfigurationProviderType.AzureKeyVault:
                        break;
                    case ConfigurationProviderType.Files:
                        var fileConfig = configItem as FileConfigSource;
                        foreach (var file in fileConfig.FileNames)
                        {
                            if (file.ToLower().EndsWith(".json"))
                                builder = builder.AddJsonFile(file);
                            else if (file.ToLower().EndsWith(".xml"))
                                builder = builder.AddXmlFile(file);
                        }
                        break;
                    case ConfigurationProviderType.UserSecrets:

                        break;
                    case ConfigurationProviderType.EnvironmentVariables:
                        break;
                    default:
                        break;
                }
            }
            Instance = Activator.CreateInstance(Type) as PluginBase;
            Instance.BaseLocation = AssemblyLocation;
        }

        public IEnumerable<ProviderItem> Build(IConfiguration configuration)
        {

            return Instance.GetProviderTypesAndArguments(configuration);
        }

        public IPluginProvider ActivateProvider(ProviderItem item)
        {
            if (Providers == null) Providers = new List<IPluginProvider>();
            var provider = Activator.CreateInstance(item.Type, item.Parameters) as IPluginProvider;
            Providers.Add(provider);
            return provider;
        }


    }

}