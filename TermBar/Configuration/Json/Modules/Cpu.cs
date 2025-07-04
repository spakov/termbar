using TermBar.Catppuccin;

namespace TermBar.Configuration.Json.Modules {
  /// <summary>
  /// A TermBar CPU usage monitor configuration.
  /// </summary>
  internal class Cpu : IModule {
    public int Order { get; set; } = int.MaxValue - 2;

    public bool Expand { get; set; } = false;

    /// <summary>
    /// The Catppuccin color to use for the CPU icon.
    /// </summary>
    public ColorEnum AccentColor { get; set; } = ColorEnum.Pink;

    /// <summary>
    /// The text to use as the CPU icon.
    /// </summary>
    public string Icon { get; set; } = "";

    /// <summary>
    /// The numeric format to use, as in
    /// https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings.
    /// </summary>
    public string Format { get; set; } = "{0:N0}%";

    /// <summary>
    /// Whether to round the value before formatting.
    /// </summary>
    public bool Round { get; set; } = true;

    /// <summary>
    /// The CPU usage update interval, in milliseconds.
    /// </summary>
    public uint UpdateInterval { get; set; } = 5000;
  }
}
