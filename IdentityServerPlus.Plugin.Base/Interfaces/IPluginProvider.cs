using IdentityServerPlus.Plugin.Base.Models;
using System.Collections.Generic;

namespace IdentityServerPlus.Plugin.Base.Interfaces {
    public interface IPluginProvider {

        string Name { get; }
        string Description { get; }

    }
}