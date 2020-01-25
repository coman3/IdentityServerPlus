using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServerPlus.Plugin.Base.Models
{
    public class PluginConfigurationItem
    {
        public string Key { get; set; }
        public Type Type { get; set; }

        public PluginConfigurationItem(string key, Type type)
        {
            Key = key;
            Type = type;
        }
    }

    public class PluginConfigurationItem<TItem> : PluginConfigurationItem
    {
        public PluginConfigurationItem() : base (nameof(TItem), typeof(TItem))
        {
        }
        public PluginConfigurationItem(string key) : base(key, typeof(TItem))
        {
        }
    }
}
