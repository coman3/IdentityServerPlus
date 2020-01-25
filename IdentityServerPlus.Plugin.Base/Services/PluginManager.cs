using IdentityServerPlus.Models;
using IdentityServerPlus.Plugin.Base;
using IdentityServerPlus.Plugin.Base.Interfaces;
using IdentityServerPlus.Plugin.Base.Models;
using IdentityServerPlus.Plugin.Base.Structures;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
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
            foreach (var plugin in directory.GetFiles().Where(x => x.Extension == ".dll"))
            {
                _logger.LogInformation("Found assembly file {0}. Searching for plugins...", plugin.FullName.Replace(currentAssLocation, ""));
                var pluginAssembly = Assembly.LoadFrom(plugin.FullName);
                var types = pluginAssembly.GetTypes().Where(x => x.IsSubclassOf(typeof(PluginBase)) && x.IsClass && !x.IsAbstract).ToList();
                foreach (var type in types)
                {
                    var instance = new PluginInstance(pluginAssembly, type);
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
                var providers = plugin.Build(_configuration);
                foreach (var provider in providers)
                {
                    var providerInstance = plugin.ActivateProvider(provider);
                    _logger.LogInformation("    Activated provider {0}...", providerInstance.Name);
                }
                    
            }
            _logger.LogInformation("All compatable providers built!");


        }


    }

    public sealed class PluginInstance
    {
        public Assembly Assembly { get; }
        public Type Type { get; }
        public string AssemblyLocation { get; }
        public bool Activated => Instance != null;
        public bool Injected { get; private set; }

        public List<IPluginProvider> Providers { get; private set; }

        public PluginBase Instance { get; private set; }

        public PluginInstance(Assembly assembly, Type type)
        {
            Assembly = assembly;
            Type = type;
        }

        public void Activate()
        {
            Instance = Activator.CreateInstance(Type) as PluginBase;
        }

        public IEnumerable<ProviderItem> Build(IConfiguration configuration)
        {
            
            return Instance.GetProviderTypesAndArguments(configuration);
        }

        public IPluginProvider ActivateProvider(ProviderItem item)
        {
            if(Providers == null) Providers = new List<IPluginProvider>();
            var provider = Activator.CreateInstance(item.Type, item.Parameters) as IPluginProvider;
            Providers.Add(provider);
            return provider;
        }


    }

}