using TermBar.Catppuccin;

namespace TermBar.Configuration.Json.Modules {
  /// <summary>
  /// A TermBar static text configuration.
  /// </summary>
  internal class StaticText : IModule {
    public int Order { get; set; } = 0;

    public bool Expand { get; set; } = false;

    /// <summary>
    /// The Catppuccin color to use for the icon.
    /// </summary>
    public ColorEnum AccentColor { get; set; } = ColorEnum.Green;

    /// <summary>
    /// The text to use as the icon.
    /// </summary>
    public string Icon { get; set; } = "";

    /// <summary>
    /// The text to display.
    /// </summary>
    public string Text { get; set; } = "Hello, world!";
  }
}
