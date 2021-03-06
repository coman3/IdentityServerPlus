﻿
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace IdentityServerPlus.Plugin.Base.Models
{
    public class PluginConfig
    {
        public string Assembly { get; set; }
        public string[] Assemblies { get; set; }
        public List<PluginConfigurationSource> ConfigurationSources { get; set; }
    }

    [JsonConverter(typeof(PluginConfigurationSourceConverter))]
    public abstract class PluginConfigurationSource
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ConfigurationProviderType ProviderType { get; set; }

    }

    internal class BaseSpecifiedConcreteClassConverter : DefaultContractResolver
    {
        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (typeof(PluginConfigurationSource).IsAssignableFrom(objectType) && !objectType.IsAbstract)
                return null; // pretend TableSortRuleConvert is not specified (thus avoiding a stack overflow)
            return base.ResolveContractConverter(objectType);
        }
    }
    internal class PluginConfigurationSourceConverter : JsonConverter
    {
        static JsonSerializerSettings SpecifiedSubclassConversion = new JsonSerializerSettings() { ContractResolver = new BaseSpecifiedConcreteClassConverter() };


        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(PluginConfigurationSource));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            switch (Enum.Parse<ConfigurationProviderType>(jo["ProviderType"].Value<string>()))
            {
                case ConfigurationProviderType.AzureKeyVault:
                    return JsonConvert.DeserializeObject<AzureKeyVaultSource>(jo.ToString(), SpecifiedSubclassConversion);
                case ConfigurationProviderType.EnvironmentVariables:
                    return JsonConvert.DeserializeObject<EnvriomentVariablesSource>(jo.ToString(), SpecifiedSubclassConversion);
                case ConfigurationProviderType.Files:
                    return JsonConvert.DeserializeObject<FileConfigSource>(jo.ToString(), SpecifiedSubclassConversion);
                case ConfigurationProviderType.UserSecrets:
                    return JsonConvert.DeserializeObject<UserSecretsSource>(jo.ToString(), SpecifiedSubclassConversion);
                default:
                    throw new Exception("Cant find config provider type");
            }

            throw new NotImplementedException("Cant find config provider type");
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException(); // won't be called because CanWrite returns false
        }
    }

    public sealed class FileConfigSource : PluginConfigurationSource
    {
        /// <summary>
        /// Files to load in order, lower index first
        /// </summary>
        public string[] FileNames { get; set; }
    }

    public sealed class UserSecretsSource : PluginConfigurationSource
    {
        /// <summary>
        /// The User Secret ID generated by "dotnet user-secrets init" or in the project file under "UserSecretsId"
        /// </summary>
        public string Id { get; set; }
    }

    public sealed class AzureKeyVaultSource : PluginConfigurationSource
    {
        public string VaultName { get; set; }
        public string ApplicationId { get; set; }
        public string CertificateThumbprint { get; set; }

    }

    public sealed class EnvriomentVariablesSource : PluginConfigurationSource
    {
        public string Prefix { get; set; }

    }

    public enum ConfigurationProviderType
    {
        AzureKeyVault,
        Files,
        UserSecrets,
        EnvironmentVariables
    }
}