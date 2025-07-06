using System.Collections.Generic;
using TermBar.Catppuccin;

namespace TermBar.Configuration.Json {
  /// <summary>
  /// A TermBar configuration for the window list modules.
  /// </summary>
  /// <remarks>Process names must not include an extension!</remarks>
  internal class WindowList {
    /// <summary>
    /// An ordered list of window process names that should be pinned (in this
    /// order) at the beginning of the window list.
    /// </summary>
    /// <remarks>These are case insensitive.</remarks>
    public List<string>? HighPriorityWindows = [
      "olk",
      "devenv",
      "librewolf"
    ];

    /// <summary>
    /// An ordered list of window process names that should be pinned (in this
    /// order) at the end of the window list.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>If a process name is in <see cref="LowPriorityWindows"/> and
    /// <see cref="HighPriorityWindows"/>, <see cref="HighPriorityWindows"/>
    /// takes precedence.</item>
    /// <item>These are case insensitive.</item>
    /// </list>
    /// </remarks>
    public List<string>? LowPriorityWindows = [
      "SteelSeriesEngine"
    ];

    /// <summary>
    /// Whether groups of windows with the same process name should be sorted
    /// alphabetically.
    /// </summary>
    /// <remarks>If <see langword="false"/>, windows are simply added in the
    /// order they were created (or, when starting, by the order they are
    /// reported by <c>EnumWindows()</c>).</remarks>
    public bool SortGroupsAlphabetically = true;

    /// <summary>
    /// The map of window process names to <see cref="ProcessIconMapping"/> to
    /// apply to windows.
    /// </summary>
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
