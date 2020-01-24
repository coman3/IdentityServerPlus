using System;
using IdentityServerPlus.Plugin.Base.Interfaces;

namespace IdentityServerPlus.Plugin.Base {
    public abstract class PluginBase {

        /// <summary>The Display Name of this plugin</summary>
        public string Name { get; }

        /// <summary>The globally unique ID for this plugin to not get mixed up with others</summary>
        public abstract Guid Id { get; }

        /// <summary>When this plugin was last updated (compiled)</summary>
        public abstract DateTime LastUpdated { get; }

        /// <summary>The Version Code of this plugin for easy updating</summary>
        public string Version { get; }

        public string ConfigurationKey { get; }

        public IPluginProvider[] Providers { get; private set; }

        protected PluginBase(string name, string version, string configurationKey) {
            this.Name = name;
            this.Version = version;
            this.ConfigurationKey = configurationKey;
        }

        public abstract IPluginProvider[] BuildProviders();
    }
}