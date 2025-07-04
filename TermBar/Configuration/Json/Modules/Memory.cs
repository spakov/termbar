using TermBar.Catppuccin;

namespace TermBar.Configuration.Json.Modules {
  /// <summary>
  /// A TermBar memory usage monitor configuration.
  /// </summary>
  internal class Memory : IModule {
    public int Order { get; set; } = int.MaxValue - 3;

    public bool Expand { get; set; } = false;

    /// <summary>
    /// The Catppuccin color to use for the memory icon.
    /// </summary>
    public ColorEnum AccentColor { get; set; } = ColorEnum.Teal;

    /// <summary>
    /// The text to use as the memory icon.
    /// </summary>
    public string Icon { get; set; } = "";

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
    /// The memory usage update interval, in milliseconds.
    /// </summary>
    public uint UpdateInterval { get; set; } = 5000;
  }
}
