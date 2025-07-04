using TermBar.Catppuccin;

namespace TermBar.Configuration.Json.Modules {
  /// <summary>
  /// A TermBar window dropdown configuration.
  /// </summary>
  internal class WindowDropdown : IModule {
    public int Order { get; set; } = 101;

    public bool Expand { get; set; } = false;

    /// <summary>
    /// The Catppuccin color to use for the dropdown icon.
    /// </summary>
    public ColorEnum AccentColor { get; set; } = ColorEnum.Mauve;

    /// <summary>
    /// The text to use as the icon for the dropdown.
    /// </summary>
    public string DropdownIcon { get; set; } = "";

    /// <summary>
    /// The text to use as window icons.
    /// </summary>
    public string Icon { get; set; } = "•";
  }
}
