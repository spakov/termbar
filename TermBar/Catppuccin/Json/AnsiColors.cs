using System;
using System.Text.Json.Serialization;

namespace TermBar.Catppuccin.Json {
  /// <summary>
  /// Catppuccin flavor ANSI colors in palette.json.
  /// </summary>
  internal class AnsiColors {
    [JsonPropertyName("black")]
    public required AnsiColorPair Black { get; set; }

    [JsonPropertyName("red")]
    public required AnsiColorPair Red { get; set; }

    [JsonPropertyName("green")]
    public required AnsiColorPair Green { get; set; }

    [JsonPropertyName("yellow")]
    public required AnsiColorPair Yellow { get; set; }

    [JsonPropertyName("blue")]
    public required AnsiColorPair Blue { get; set; }

    [JsonPropertyName("magenta")]
    public required AnsiColorPair Magenta { get; set; }

    [JsonPropertyName("cyan")]
    public required AnsiColorPair Cyan { get; set; }

    [JsonPropertyName("white")]
    public required AnsiColorPair White { get; set; }

    /// <summary>
    /// Allows indexing of the ANSI color using <paramref name="ansiColor"/>.
    /// </summary>
    /// <param name="ansiColor">The ANSI color.</param>
    /// <returns>An <see cref="AnsiColorPair"/>.</returns>
    /// <exception cref="IndexOutOfRangeException"></exception>
    internal Color this[AnsiColorEnum ansiColor] {
      get {
        return ansiColor switch {
          AnsiColorEnum.Black => Black.Normal,
          AnsiColorEnum.Red => Red.Normal,
          AnsiColorEnum.Green => Green.Normal,
          AnsiColorEnum.Yellow => Yellow.Normal,
          AnsiColorEnum.Blue => Blue.Normal,
          AnsiColorEnum.Magenta => Magenta.Normal,
          AnsiColorEnum.Cyan => Cyan.Normal,
          AnsiColorEnum.White => White.Normal,
          AnsiColorEnum.BrightBlack => Black.Bright,
          AnsiColorEnum.BrightRed => Red.Bright,
          AnsiColorEnum.BrightGreen => Green.Bright,
          AnsiColorEnum.BrightYellow => Yellow.Bright,
          AnsiColorEnum.BrightBlue => Blue.Bright,
          AnsiColorEnum.BrightMagenta => Magenta.Bright,
          AnsiColorEnum.BrightCyan => Cyan.Bright,
          AnsiColorEnum.BrightWhite => White.Bright,
          _ => throw new IndexOutOfRangeException()
        };
      }
    }
  }
}
