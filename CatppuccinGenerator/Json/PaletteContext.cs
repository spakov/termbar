using System.Text.Json.Serialization;

namespace CatppuccinGenerator.Json {
  [JsonSerializable(typeof(Palette))]
  internal partial class PaletteContext : JsonSerializerContext { }
}
