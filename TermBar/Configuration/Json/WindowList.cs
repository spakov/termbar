using System.Collections.Generic;
using TermBar.Catppuccin;

namespace TermBar.Configuration.Json {
  /// <summary>
  /// A TermBar configuration for the window list modules.
  /// </summary>
  internal class WindowList {
    /// <summary>
    /// The map of window process names to <see cref="ProcessIconMapping"/> to
    /// apply to windows.
    /// </summary>
    /// <remarks>These process names do not have an extension!</remarks>
    public Dictionary<string, ProcessIconMapping>? ProcessIconMap { get; set; } = new() {
      { "devenv", new(ColorEnum.Mauve, "") },
      { "explorer", new(ColorEnum.Yellow, "") },
      {
        "librewolf",
        new(
          ColorEnum.Sky,
          "",
          new() {
            { "Microsoft Learn", new(ColorEnum.Mauve, "") },
            { "Stardew Valley Wiki", new(ColorEnum.Peach, "󱐟") }
          }
        )
      },
      { "olk", new(ColorEnum.Sapphire, "") },
      { "steamwebhelper", new(ColorEnum.Blue, "") },
      { "WindowsTerminal", new(ColorEnum.Overlay0, "") }
    };
  }

  /// <summary>
  /// A TermBar configuration for the window list modules' icon mapping.
  /// </summary>
  internal class ProcessIconMapping {
    /// <summary>
    /// The Catppuccin color to use for the window icon.
    /// </summary>
    /// <remarks>Set to <c>null</c> to use the window list module's <see
    /// cref="Modules.WindowBar.AccentColor"/>.</remarks>
    public ColorEnum? IconColor { get; set; } = null;

    /// <summary>
    /// The text to use as window icons.
    /// </summary>
    /// <remarks>Set to <c>null</c> to use the window list module's <see
    /// cref="Modules.WindowBar.Icon"/>.</remarks>
    public string? Icon { get; set; } = null;

    /// <summary>
    /// The map of window name regular expressions to match to <see
    /// cref="WindowNameIconMapping"/> to apply to windows.
    /// </summary>
    /// <remarks>These override <see cref="IconColor"/> and <see
    /// cref="Icon"/>.</remarks>
    public Dictionary<string, WindowNameIconMapping>? WindowNameIconMap { get; set; } = null;

    public ProcessIconMapping() { }

    public ProcessIconMapping(ColorEnum iconColor, string icon, Dictionary<string, WindowNameIconMapping>? windowNameIconMap = null) {
      IconColor = iconColor;
      Icon = icon;
      WindowNameIconMap = windowNameIconMap;
    }
  }

  /// <summary>
  /// A TermBar configuration for the window list modules' icon mapping.
  /// </summary>
  internal class WindowNameIconMapping {
    /// <summary>
    /// The Catppuccin color to use for the window icon.
    /// </summary>
    /// <remarks>Set to <c>null</c> to use the window list module's <see
    /// cref="Modules.WindowBar.AccentColor"/>.</remarks>
    public ColorEnum? IconColor { get; set; } = null;

    /// <summary>
    /// The text to use as window icons.
    /// </summary>
    /// <remarks>Set to <c>null</c> to use the window list module's <see
    /// cref="Modules.WindowBar.Icon"/>.</remarks>
    public string? Icon { get; set; } = null;

    public WindowNameIconMapping() { }

    public WindowNameIconMapping(ColorEnum iconColor, string icon) {
      IconColor = iconColor;
      Icon = icon;
    }
  }
}
