using System.Text.Json.Serialization;

namespace TermBar.Catppuccin.Json {
  internal class AnsiColorPair {
    [JsonPropertyName("normal")]
    public required Color Normal { get; set; }

    [JsonPropertyName("bright")]
    public required Color Bright { get; set; }
  }
}
