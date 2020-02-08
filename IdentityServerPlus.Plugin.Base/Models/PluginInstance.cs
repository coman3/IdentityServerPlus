using IdentityServerPlus.Plugin.Base.Interfaces;
using IdentityServerPlus.Plugin.Base.Models;
using IdentityServerPlus.Plugin.Base.Structures;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

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
        public IConfiguration Configuration { get; private set; }

        public PluginBase Instance { get; private set; }

        public PluginInstance(Assembly assembly, Type type, PluginConfig config)
        {
            Assembly = assembly;
            Type = type;
            PluginConfig = config;
        }

        public void Activate()
        {


            if (PluginConfig.ConfigurationSources != null && Type.GetConstructors().Any(x => x.GetParameters().Any(c => c.ParameterType == typeof(IConfiguration))))
            {
                IConfigurationBuilder builder = new ConfigurationBuilder();
                builder.SetBasePath(AssemblyLocation);
                foreach (var configItem in PluginConfig.ConfigurationSources)
                {
                    switch (configItem.ProviderType)
                    {
                        case ConfigurationProviderType.AzureKeyVault:
                            var azureConfig = configItem as AzureKeyVaultSource;
                            using (var store = new X509Store(StoreLocation.CurrentUser))
                            {
                                store.Open(OpenFlags.ReadOnly);
                                var certs = store.Certificates
                                    .Find(X509FindType.FindByThumbprint,
                                        azureConfig.CertificateThumbprint, false);

                                builder.AddAzureKeyVault(
                                    $"https://{azureConfig.VaultName}.vault.azure.net/",
                                    azureConfig.ApplicationId,
                                    certs.OfType<X509Certificate2>().Single());
                                store.Close();
                            }
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
                            var userConfig = configItem as UserSecretsSource;
                            builder.AddUserSecrets(userConfig.Id);
                            break;
                        case ConfigurationProviderType.EnvironmentVariables:
                            var envConfig = configItem as EnvriomentVariablesSource;
                            builder.AddEnvironmentVariables(envConfig.Prefix);
                            break;
                        default:
                            break;
                    }
                }
                Configuration = builder.Build();
                Instance = Activator.CreateInstance(Type, Configuration) as PluginBase;
            }
            else
            {
                Instance = Activator.CreateInstance(Type) as PluginBase;
            }
            Instance.BaseLocation = AssemblyLocation;
        }

        public IEnumerable<ProviderItem> Build()
        {

            return Instance.GetProviderTypesAndArguments();
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