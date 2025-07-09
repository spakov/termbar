using System.Text.Json.Serialization;

namespace TermBar.Catppuccin.Json {
  [JsonSerializable(typeof(Palette))]
  internal partial class PaletteContext : JsonSerializerContext { }
}
