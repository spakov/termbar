using System.Text.Json.Serialization;

namespace TermBar.Catppuccin.Json {
  /// <summary>
  /// A Catppuccin palette.json flavor.
  /// </summary>
  internal class Flavor {
    /// <summary>
    /// Flavor colors.
    /// </summary>
    [JsonPropertyName("colors")]
    public required Colors Colors { get; set; }

    /// <summary>
    /// Flavor ANSI colors.
    /// </summary>
    [JsonPropertyName("ansiColors")]
    public required AnsiColors AnsiColors { get; set; }
  }
}
