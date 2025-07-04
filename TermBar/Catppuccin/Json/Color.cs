using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TermBar.Catppuccin.Json {
  /// <summary>
  /// A Catppuccin flavor color in palette.json.
  /// </summary>
  internal class Color {
    /// <summary>
    /// The color name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>
    /// The color's RGB triplet.
    /// </summary>
    [JsonPropertyName("rgb")]
    public required Rgb Rgb { get; set; }

    /// <summary>
    /// Whether this is an accent color.
    /// </summary>
    [JsonPropertyName("accent")]
    public required bool IsAccent { get; set; }

    private static readonly Dictionary<Rgb, Microsoft.UI.Xaml.Media.SolidColorBrush> cache = [];

    /// <summary>
    /// A <see cref="Microsoft.UI.Xaml.Media.SolidColorBrush"/> for the color.
    /// </summary>
    internal Microsoft.UI.Xaml.Media.SolidColorBrush SolidColorBrush => SolidColorBrushCache(Rgb);

    /// <summary>
    /// Caches and returns a <see
    /// cref="Microsoft.UI.Xaml.Media.SolidColorBrush"/> based on <paramref
    /// name="rgb"/>.
    /// </summary>
    /// <param name="rgb">The color for which to return a <see
    /// cref="Microsoft.UI.Xaml.Media.SolidColorBrush"/>.</param>
    /// <returns>A <see
    /// cref="Microsoft.UI.Xaml.Media.SolidColorBrush"/>.</returns>
    private static Microsoft.UI.Xaml.Media.SolidColorBrush SolidColorBrushCache(Rgb rgb) {
      return cache.TryGetValue(rgb, out Microsoft.UI.Xaml.Media.SolidColorBrush? value)
        ? value
        : (cache[rgb] = new Microsoft.UI.Xaml.Media.SolidColorBrush(new Windows.UI.Color() { A = 0xff, R = (byte) rgb.Red, G = (byte) rgb.Green, B = (byte) rgb.Blue }));
    }
  }
}
