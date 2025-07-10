using System.Text.Json.Serialization;

namespace Spakov.TermBar.Configuration.Json {
  [JsonSourceGenerationOptions(WriteIndented = true, PropertyNameCaseInsensitive = true, UseStringEnumConverter = true)]
  [JsonSerializable(typeof(Config))]
  internal partial class ConfigContext : JsonSerializerContext { }
}