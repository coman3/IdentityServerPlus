using IdentityServerPlus.Models;
using IdentityServerPlus.Plugin.Base;
using IdentityServerPlus.Plugin.Base.Interfaces;
using IdentityServerPlus.Plugin.Base.Models;
using IdentityServerPlus.Plugin.Base.Structures;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing.Matching;
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
        private Assembly[] AssemblyState { get; set; }
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

        public PluginInstance GetPluginInstance()
        {
            var assemblyLocation = new FileInfo(Assembly.GetCallingAssembly().Location).DirectoryName;

            return PluginInstances.SingleOrDefault(x => x.AssemblyLocation == assemblyLocation);
        }

        public void CollectAll()
        {

            AssemblyState = AppDomain.CurrentDomain.GetAssemblies();
            DirectoryInfo directory = null;
            var currentAssLocation = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName;
            if (Directory.Exists(Path.Combine(currentAssLocation, _pluginConfiguration.Directory)))
            {
                _logger.LogDebug("Relative plugin directory found.");
                directory = new DirectoryInfo(Path.Combine(currentAssLocation, _pluginConfiguration.Directory));
            }
            else if (Directory.Exists(_pluginConfiguration.Directory))
            {
                _logger.LogDebug("Absolute plugin directory found.");
                directory = new DirectoryInfo(_pluginConfiguration.Directory);
            }
            else
            {
                _logger.LogWarning("Plugin directory not found. Failed to load any plugins!");
                Directory.CreateDirectory(Path.Combine(currentAssLocation, _pluginConfiguration.Directory));
                return;
            }



            _logger.LogDebug("Searching for plugins...");
            foreach (var plugin in directory.GetDirectories().Where(x => x.GetFiles().Any(c => c.Name == "plugin.json")))
            {
                _logger.LogTrace("Found plugin folder {0}. Loading Assembly File...", plugin.FullName.Replace(currentAssLocation, ""));
                var pluginConfig = JsonConvert.DeserializeObject<PluginConfig>(File.ReadAllText(Path.Combine(plugin.FullName, "plugin.json")));
                var filesToLoad = new List<string>();
                if (!string.IsNullOrWhiteSpace(pluginConfig.Assembly))
                {
                    filesToLoad.Add(pluginConfig.Assembly);
                }
                else
                {
                    filesToLoad.AddRange(pluginConfig.Assemblies);
                }
                foreach (var ass in filesToLoad)
                {
                    _logger.LogTrace("    Loading Assembly: {0}", ass);
                    var pluginAssembly = Assembly.LoadFrom(Path.Combine(plugin.FullName, ass));
                    var types = pluginAssembly.GetTypes().Where(x => x.IsSubclassOf(typeof(PluginBase)) && x.IsClass && !x.IsAbstract).ToList();
                    foreach (var type in types)
                    {
                        var instance = new PluginInstance(pluginAssembly, type, pluginConfig) { AssemblyLocation = plugin.FullName };

                        _logger.LogTrace("    Activating Plugin: {0}", type.Name);
                        instance.Activate();
                        PluginInstances.Add(instance);
                        _logger.LogDebug("    Activated Plugin: {0} ({1})", instance.Instance.Name, instance.Instance.Version);
                    }
                }
            }
            _logger.LogDebug("All compatable plugins loaded!");

            _logger.LogTrace("Building plugin providers...");
            foreach (var plugin in PluginInstances)
            {
                _logger.LogTrace("    Building Plugin Providers for {0}...", plugin.Instance.Name);
                var providers = plugin.Build();
                foreach (var provider in providers)
                {
                    var providerInstance = plugin.ActivateProvider(provider);
                    _logger.LogDebug("    Activated provider {0}...", providerInstance.Name);
                }

            }
            _logger.LogDebug("All compatable providers built!");


            LogLoadedAssemblies();

        }

        public void Finalize()
        {
            LogLoadedAssemblies();
            _logger.LogInformation("All Plugins Loaded.");
        }

        private IEnumerable<Assembly> GetLoadedAssemblies()
        {
            var endAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            foreach (var ass in AssemblyState)
            {
                endAssemblies.Remove(ass);
            }
            AssemblyState = AssemblyState.Concat(endAssemblies).ToArray();
            return endAssemblies;
        }

        private void LogLoadedAssemblies(string message = "Loaded Assemblies:", string indent = "\t")
        {
            var endAssemblies = GetLoadedAssemblies();

            _logger.LogTrace(message);
            foreach (var ass in endAssemblies.OrderBy(x => x.FullName))
            {
                _logger.LogTrace("    " + ass.FullName);
            }
        }


        public AuthenticationBuilder BuildAuthentication(AuthenticationBuilder builder)
        {
            _logger.LogTrace("Building Authentication Providers...");
            foreach (var plugin in PluginInstances)
            {
                foreach (var provider in plugin.Providers.OfType<IAuthenticationProvider>())
                {
                    if (provider == null) continue;
                    _logger.LogTrace("    Building Authentication Provider {0} ({1} - {2})...", provider.Name, provider.FriendlyName, provider.Scheme);
                    builder = provider.Build(builder);
                }
            }
            _logger.LogDebug("Built all Authentication Providers!");
            LogLoadedAssemblies();
            return builder;
        }

        public IdentityBuilder BuildIdentity(IdentityBuilder builder)
        {
            _logger.LogTrace("Building Identity Providers...");
            foreach (var plugin in PluginInstances)
            {
                foreach (var provider in plugin.Providers.OfType<IIdentityProvider>())
                {
                    if (provider == null) continue;
                    _logger.LogTrace("    Building Identity Provider {0} (type: {1})...", provider.Name, provider.Type);
                    builder = provider.Build(builder);
                }
            }
            _logger.LogDebug("Built all Identity Providers!");
            LogLoadedAssemblies();
            return builder;

        }

        public IIdentityServerBuilder BuildIdentityServer(IIdentityServerBuilder builder)
        {
            _logger.LogTrace("Building Identity Server Providers...");
            foreach (var plugin in PluginInstances)
            {
                foreach (var provider in plugin.Providers.OfType<IIdentityServerProvider>())
                {
                    if (provider == null) continue;
                    _logger.LogTrace("    Building Identity Server Provider {0} (type: {1})...", provider.Name, provider.Type);
                    builder = provider.Build(builder);
                }
            }
            _logger.LogDebug("Built all Identity Server Providers!");
            LogLoadedAssemblies();
            return builder;
        }

        public void BuildServices(IServiceCollection services)
        {
            _logger.LogTrace("Building Service Configuration Providers...");
            foreach (var plugin in PluginInstances)
            {
                foreach (var provider in plugin.Providers.OfType<IServiceConfigurationProvider>())
                {
                    if (provider == null) continue;
                    _logger.LogTrace("    Building Service Configuration Provider {0}...", provider.Name);
                    provider.ConfigureServices(services);
                }
            }
            _logger.LogDebug("Built all Service Configuration Providers!");
            LogLoadedAssemblies();
        }

        public void BuildAppConfiguration(IApplicationBuilder app)
        {
            _logger.LogTrace("Building App Configuration Providers...");
            foreach (var plugin in PluginInstances)
            {
                foreach (var provider in plugin.Providers.OfType<IAppConfigurationProvider>())
                {
                    if (provider == null) continue;
                    _logger.LogTrace("    Building App Configuration Provider {0}...", provider.Name);
                    provider.Configure(app);
                }
            }
            _logger.LogDebug("Built all App Configuration Providers!");
            LogLoadedAssemblies();
        }

        public IMvcBuilder BuildThemeProviders(IMvcBuilder builder)
        {
            _logger.LogTrace("Building Theme Providers...");

            foreach (var plugin in PluginInstances)
            {
                foreach (var provider in plugin.Providers.OfType<IThemeProvider>().OrderBy(x => x.Index))
                {
                    if (provider == null) continue;
                    _logger.LogTrace("    Building Theme Provider {0}...", provider.Name);
                    provider.ConfigureRazor(builder);
                }
            }
            _logger.LogDebug("Combining Theme Assets...");
            var wwwRootDirectory = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName, "wwwroot");
            foreach (var provider in PluginInstances.SelectMany(x => x.Providers.OfType<IThemeProvider>()).OrderBy(x => x.Index))
            {
                var assetsFolder = Path.Combine(PluginInstances.FirstOrDefault(x => x.Providers.Contains(provider))?.AssemblyLocation, "wwwroot");
                if (Directory.Exists(assetsFolder))
                {
                    _logger.LogDebug("    Copying Assets from provider {0}:", provider.Name);
                    DirectoryCopy(assetsFolder, wwwRootDirectory, true, true);
                }

            }

            builder.Services.AddSingleton<EndpointSelector, PluginEndpointSelector>();
            _logger.LogDebug("Built all Theme Providers!");
            LogLoadedAssemblies();
            return builder;
        }

        public IEnumerable<TType> GetProviders<TType>()
        {
            return PluginInstances.SelectMany(p => p.Providers.OfType<TType>());
        }
        private void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, bool overwrite)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, overwrite);
                _logger.LogDebug("        Copied: {0}", temppath);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs, overwrite);
                }
            }
        }
    }

}