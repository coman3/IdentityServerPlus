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