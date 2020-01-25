namespace IdentityServerPlus.Plugin.Base.Models
{
    public enum IdentityProviderType
    {
        /// <summary>
        /// Only one database type is allowed
        /// </summary>
        Database,

        /// <summary>
        /// Other is allowed, however frowned upon; this enum should have more descriptive types.
        /// TODO: Create other possible  types
        /// </summary>
        Other
    }
}