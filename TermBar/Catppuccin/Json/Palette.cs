using System;
using System.Text.Json.Serialization;

namespace TermBar.Catppuccin.Json {
  /// <summary>
  /// The Catppuccin palette.json file.
  /// </summary>
  internal class Palette {
    /// <summary>
    /// The Latte flavor.
    /// </summary>
    [JsonPropertyName("latte")]
    public required Flavor Latte { get; set; }

    /// <summary>
    /// The Frappé flavor.
    /// </summary>
    [JsonPropertyName("frappe")]
    public required Flavor Frappe { get; set; }

    /// <summary>
    /// The Macchiato flavor.
    /// </summary>
    [JsonPropertyName("macchiato")]
    public required Flavor Macchiato { get; set; }

    /// <summary>
    /// The Mocha flavor.
    /// </summary>
    [JsonPropertyName("mocha")]
    public required Flavor Mocha { get; set; }

    /// <summary>
    /// Allows indexing of the palette flavor using <paramref name="flavor"/>.
    /// </summary>
    /// <param name="flavor">The Catppuccin flavor.</param>
    /// <returns>A <see cref="Flavor"/>.</returns>
    /// <exception cref="IndexOutOfRangeException"></exception>
    internal Flavor this[FlavorEnum flavor] {
      get {
        return flavor switch {
          FlavorEnum.Latte => Latte,
          FlavorEnum.Frappe => Frappe,
          FlavorEnum.Macchiato => Macchiato,
          FlavorEnum.Mocha => Mocha,
          _ => throw new IndexOutOfRangeException()
        };
      }
    }
  }
}
