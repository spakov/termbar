using System.Text.Json.Serialization;

namespace Spakov.TermBar.Configuration.Json
{
    /// <summary>
    /// A <see cref="JsonSerializerContext"/> for reading and writing JSON
    /// configuration via generated code.
    /// </summary>
    [JsonSourceGenerationOptions(WriteIndented = true, PropertyNameCaseInsensitive = true, UseStringEnumConverter = true)]
    [JsonSerializable(typeof(Config))]
    internal partial class ConfigContext : JsonSerializerContext {
    }
}