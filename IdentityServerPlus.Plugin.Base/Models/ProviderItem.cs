using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServerPlus.Plugin.Base.Models
{
    public class ProviderItem
    {
        public Type Type { get; set; }
        public object[] Parameters { get; set; }

        public ProviderItem(Type type)
        {
            Type = type;
            Parameters = new object[] { };
        }

        public ProviderItem(Type type, params object[] parameters)
        {
            Type = type;
            Parameters = parameters;
        }

    }

    public class ProviderItem<TItem> : ProviderItem
    {
        public ProviderItem() : base(typeof(TItem))
        {

        }
        public ProviderItem(params object[] parameters) : base(typeof(TItem), parameters)
        {

        }
    }


}
