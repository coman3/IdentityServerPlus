using IdentityServerPlus.Models;
using IdentityServerPlus.Plugin.Base;
using IdentityServerPlus.Plugin.Base.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace IdentityServerPlus.Services
{

    public sealed class PluginManager
    {
        private ILogger _logger;
        private PluginManagerConfiguration _configuration;

        public List<PluginInstance> PluginInstances = new List<PluginInstance>();
        

        public PluginManager(ILogger<PluginManager> logger)
        {
            _logger = logger;
        }
        public void Configure(PluginManagerConfiguration config)
        {
            _configuration = config;
        }

        public AuthenticationBuilder BuildAuthentication(AuthenticationBuilder builder)
        {
            _logger.LogInformation("Building Authentication Providers...");
            foreach (var plugin in PluginInstances)
            {
                foreach (var provider in plugin.Providers.Where(x => x.GetProviderType() == ProviderType.AuthenticationProvider))
                {
                    var authProvider = provider as AuthenticationProvider;
                    if (authProvider == null) continue;
                    _logger.LogInformation("    Building Authentication Provider {0} ({1})...", authProvider.ProviderName, authProvider.ProviderScheme);
                    builder = authProvider.Build(builder);
                }
            }
            _logger.LogInformation("Built all Authentication Providers!");

            return builder;
        }

        public void CollectAll(IServiceProvider serviceProvider)
        {
            DirectoryInfo directory = null;
            var currentAssLocation = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName;
            if (Directory.Exists(Path.Combine(currentAssLocation, _configuration.Directory)))
            {
                _logger.LogInformation("Relative plugin directory found.");
                directory = new DirectoryInfo(Path.Combine(currentAssLocation, _configuration.Directory));
            }
            else if (Directory.Exists(_configuration.Directory))
            {
                _logger.LogInformation("Absolute plugin directory found.");
                directory = new DirectoryInfo(_configuration.Directory);
            }
            else
            {
                _logger.LogWarning("Plugin directory not found. Failed to load any plugins!");
                Directory.CreateDirectory(Path.Combine(currentAssLocation, _configuration.Directory));
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
                    instance.Activate(serviceProvider);
                    PluginInstances.Add(instance);
                    _logger.LogInformation("    Activated Plugin: {0} ({1})", instance.Instance.Name, instance.Instance.Version);
                }
            }
            _logger.LogInformation("All compatable plugins loaded!");

            _logger.LogInformation("Building plugin providers...");
            foreach (var plugin in PluginInstances)
            {
                _logger.LogInformation("    Building Plugin Providers for {0}...", plugin.Instance.Name);
                plugin.Build();

                foreach (var provider in plugin.Providers)
                {
                    _logger.LogInformation("        Built {0} from {1}...", provider.GetProviderType().ToString(), plugin.Instance.Name);
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

        public IPluginProvider[] Providers { get; private set; }

        public PluginBase Instance { get; private set; }

        public PluginInstance(Assembly assembly, Type type)
        {
            Assembly = assembly;
            Type = type;
        }

        public void Activate(IServiceProvider serviceProvider)
        {
            Instance = ActivatorUtilities.CreateInstance(serviceProvider, Type) as PluginBase;
        }

        public void Build()
        {
            Providers = Instance.BuildProviders();
        }


    }

}