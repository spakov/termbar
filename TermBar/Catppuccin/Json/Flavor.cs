using System.Text.Json.Serialization;

namespace TermBar.Catppuccin.Json {
  /// <summary>
  /// A Catppuccin palette.json flavor.
  /// </summary>
  internal class Flavor {
    /// <summary>
    /// The flavor name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>
    /// The flavor emoji.
    /// </summary>
    [JsonPropertyName("emoji")]
    public required string Emoji { get; set; }

    /// <summary>
    /// Whether the flavor is dark.
    /// </summary>
    [JsonPropertyName("dark")]
    public required bool IsDark { get; set; }

    /// <summary>
    /// Flavor colors.
    /// </summary>
    [JsonPropertyName("colors")]
    public required Colors Colors { get; set; }
  }
}
