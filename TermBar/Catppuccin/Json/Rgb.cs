using System.Text.Json.Serialization;

namespace TermBar.Catppuccin.Json {
  /// <summary>
  /// A Catppuccin flavor color rgb object in palette.json.
  /// </summary>
  internal class Rgb {
    [JsonPropertyName("r")]
    public required int Red { get; set; }

    [JsonPropertyName("g")]
    public required int Green { get; set; }

    [JsonPropertyName("b")]
    public required int Blue { get; set; }
  }
}
