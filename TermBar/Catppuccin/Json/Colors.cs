using System;
using System.Text.Json.Serialization;

namespace TermBar.Catppuccin.Json {
  /// <summary>
  /// Catppuccin flavor colors in palette.json.
  /// </summary>
  internal class Colors {
    [JsonPropertyName("rosewater")]
    public required Color Rosewater { get; set; }

    [JsonPropertyName("flamingo")]
    public required Color Flamingo { get; set; }

    [JsonPropertyName("pink")]
    public required Color Pink { get; set; }

    [JsonPropertyName("mauve")]
    public required Color Mauve { get; set; }

    [JsonPropertyName("red")]
    public required Color Red { get; set; }

    [JsonPropertyName("maroon")]
    public required Color Maroon { get; set; }

    [JsonPropertyName("peach")]
    public required Color Peach { get; set; }

    [JsonPropertyName("yellow")]
    public required Color Yellow { get; set; }

    [JsonPropertyName("green")]
    public required Color Green { get; set; }

    [JsonPropertyName("teal")]
    public required Color Teal { get; set; }

    [JsonPropertyName("sky")]
    public required Color Sky { get; set; }

    [JsonPropertyName("sapphire")]
    public required Color Sapphire { get; set; }

    [JsonPropertyName("blue")]
    public required Color Blue { get; set; }

    [JsonPropertyName("lavender")]
    public required Color Lavender { get; set; }

    [JsonPropertyName("text")]
    public required Color Text { get; set; }

    [JsonPropertyName("subtext1")]
    public required Color Subtext1 { get; set; }

    [JsonPropertyName("subtext0")]
    public required Color Subtext0 { get; set; }

    [JsonPropertyName("overlay2")]
    public required Color Overlay2 { get; set; }

    [JsonPropertyName("overlay1")]
    public required Color Overlay1 { get; set; }

    [JsonPropertyName("overlay0")]
    public required Color Overlay0 { get; set; }

    [JsonPropertyName("surface2")]
    public required Color Surface2 { get; set; }

    [JsonPropertyName("surface1")]
    public required Color Surface1 { get; set; }

    [JsonPropertyName("surface0")]
    public required Color Surface0 { get; set; }

    [JsonPropertyName("base")]
    public required Color Base { get; set; }

    [JsonPropertyName("mantle")]
    public required Color Mantle { get; set; }

    [JsonPropertyName("crust")]
    public required Color Crust { get; set; }

    /// <summary>
    /// Allows indexing of the color using <paramref name="color"/>.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns>A <see cref="Color"/>.</returns>
    /// <exception cref="IndexOutOfRangeException"></exception>
    internal Color this[ColorEnum color] {
      get {
        return color switch {
          ColorEnum.Rosewater => Rosewater,
          ColorEnum.Flamingo => Flamingo,
          ColorEnum.Pink => Pink,
          ColorEnum.Mauve => Mauve,
          ColorEnum.Red => Red,
          ColorEnum.Maroon => Maroon,
          ColorEnum.Peach => Peach,
          ColorEnum.Yellow => Yellow,
          ColorEnum.Green => Green,
          ColorEnum.Teal => Teal,
          ColorEnum.Sky => Sky,
          ColorEnum.Sapphire => Sapphire,
          ColorEnum.Blue => Blue,
          ColorEnum.Lavender => Lavender,
          ColorEnum.Text => Text,
          ColorEnum.Subtext1 => Subtext1,
          ColorEnum.Subtext0 => Subtext0,
          ColorEnum.Overlay2 => Overlay2,
          ColorEnum.Overlay1 => Overlay1,
          ColorEnum.Overlay0 => Overlay0,
          ColorEnum.Surface2 => Surface2,
          ColorEnum.Surface1 => Surface1,
          ColorEnum.Surface0 => Surface0,
          ColorEnum.Base => Base,
          ColorEnum.Mantle => Mantle,
          ColorEnum.Crust => Crust,
          _ => throw new IndexOutOfRangeException()
        };
      }
    }
  }
}
