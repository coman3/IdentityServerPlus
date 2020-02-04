using IdentityServerPlus.Models;
using IdentityServerPlus.Plugin.Base;
using IdentityServerPlus.Plugin.Base.Interfaces;
using IdentityServerPlus.Plugin.Base.Models;
using IdentityServerPlus.Plugin.Base.Structures;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace IdentityServerPlus.Plugin.Base.Services
{

    public sealed class PluginManager
    {
        private ILogger _logger;
        private IConfiguration _configuration;
        private PluginManagerConfiguration _pluginConfiguration => _configuration.GetSection("PluginManager").Get<PluginManagerConfiguration>();

        public List<PluginInstance> PluginInstances = new List<PluginInstance>();
        

        public PluginManager(ILogger<PluginManager> logger)
        {
            _logger = logger;
        }
        public void Configure(IConfiguration config)
        {
            _configuration = config;
        }

        public void CollectAll()
        {
            DirectoryInfo directory = null;
            var currentAssLocation = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName;
            if (Directory.Exists(Path.Combine(currentAssLocation, _pluginConfiguration.Directory)))
            {
                _logger.LogInformation("Relative plugin directory found.");
                directory = new DirectoryInfo(Path.Combine(currentAssLocation, _pluginConfiguration.Directory));
            }
            else if (Directory.Exists(_pluginConfiguration.Directory))
            {
                _logger.LogInformation("Absolute plugin directory found.");
                directory = new DirectoryInfo(_pluginConfiguration.Directory);
            }
            else
            {
                _logger.LogWarning("Plugin directory not found. Failed to load any plugins!");
                Directory.CreateDirectory(Path.Combine(currentAssLocation, _pluginConfiguration.Directory));
                return;
            }



            _logger.LogInformation("Searching for plugins...");
            foreach (var plugin in directory.GetDirectories().Where(x => x.GetFiles().Any(c => c.Name == "plugin.json")))
            {
                _logger.LogInformation("Found plugin folder {0}. Loading Assembly File...", plugin.FullName.Replace(currentAssLocation, ""));
                var pluginConfig = JsonConvert.DeserializeObject<PluginConfig>(File.ReadAllText(Path.Combine(plugin.FullName, "plugin.json")));
                var pluginAssembly = Assembly.LoadFrom(Path.Combine(plugin.FullName, pluginConfig.Assembly));
                var types = pluginAssembly.GetTypes().Where(x => x.IsSubclassOf(typeof(PluginBase)) && x.IsClass && !x.IsAbstract).ToList();
                foreach (var type in types)
                {
                    var instance = new PluginInstance(pluginAssembly, type, pluginConfig) { AssemblyLocation = plugin.FullName };
                    
                    _logger.LogInformation("    Activating Plugin: {0}", type.Name);
                    instance.Activate();
                    PluginInstances.Add(instance);
                    _logger.LogInformation("    Activated Plugin: {0} ({1})", instance.Instance.Name, instance.Instance.Version);
                }
            }
            _logger.LogInformation("All compatable plugins loaded!");

            _logger.LogInformation("Building plugin providers...");
            foreach (var plugin in PluginInstances)
            {
                _logger.LogInformation("    Building Plugin Providers for {0}...", plugin.Instance.Name);
                var providers = plugin.Build();
                foreach (var provider in providers)
                {
                    var providerInstance = plugin.ActivateProvider(provider);
                    _logger.LogInformation("    Activated provider {0}...", providerInstance.Name);
                }
                    
            }
            _logger.LogInformation("All compatable providers built!");


        }

        

        public AuthenticationBuilder BuildAuthentication(AuthenticationBuilder builder)
        {
            _logger.LogInformation("Building Authentication Providers...");
            foreach (var plugin in PluginInstances)
            {
                foreach (var provider in plugin.Providers.OfType<IAuthenticationProvider>())
                {
                    if (provider == null) continue;
                    _logger.LogInformation("    Building Authentication Provider {0} ({1})...", provider.FriendlyName, provider.Scheme);
                    builder = provider.Build(builder);
                }
            }
            _logger.LogInformation("Built all Authentication Providers!");

            return builder;
        }

        public IdentityBuilder BuildIdentity(IdentityBuilder builder)
        {
            _logger.LogInformation("Building Identity Providers...");
            foreach (var plugin in PluginInstances)
            {
                foreach (var provider in plugin.Providers.OfType<IIdentityProvider>())
                {
                    if (provider == null) continue;
                    _logger.LogInformation("    Building Identity Provider {0} (type: {1})...", provider.Name, provider.Type);
                    builder = provider.Build(builder);
                }
            }
            _logger.LogInformation("Built all Identity Providers!");

            return builder;

        }

        public IIdentityServerBuilder BuildIdentityServer(IIdentityServerBuilder builder)
        {
            _logger.LogInformation("Building Identity Server Providers...");
            foreach (var plugin in PluginInstances)
            {
                foreach (var provider in plugin.Providers.OfType<IIdentityServerProvider>())
                {
                    if (provider == null) continue;
                    _logger.LogInformation("    Building Identity Server Provider {0} (type: {1})...", provider.Name, provider.Type);
                    builder = provider.Build(builder);
                }
            }
            _logger.LogInformation("Built all Identity Server Providers!");

            return builder;
        }

        public void BuildServices(IServiceCollection services)
        {
            _logger.LogInformation("Building Service Configuration Providers...");
            foreach (var plugin in PluginInstances)
            {
                foreach (var provider in plugin.Providers.OfType<IServiceConfigurationProvider>())
                {
                    if (provider == null) continue;
                    _logger.LogInformation("    Building Service Configuration Provider...");
                    provider.ConfigureServices(services);
                }
            }
            _logger.LogInformation("Built all Service Configuration Providers!");
        }

        public void BuildAppConfiguration(IApplicationBuilder app)
        {
            _logger.LogInformation("Building App Configuration Providers...");
            foreach (var plugin in PluginInstances)
            {
                foreach (var provider in plugin.Providers.OfType<IAppConfigurationProvider>())
                {
                    if (provider == null) continue;
                    _logger.LogInformation("    Building App Configuration Provider...");
                    provider.Configure(app);
                }
            }
            _logger.LogInformation("Built all App Configuration Providers!");
        }

        public IEnumerable<TType> GetProviders<TType>()
        {
            return PluginInstances.SelectMany(p => p.Providers.OfType<TType>());
        }
    }

}