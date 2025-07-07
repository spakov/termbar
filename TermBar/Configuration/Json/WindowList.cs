using System.Collections.Generic;
using System.ComponentModel;
using TermBar.Catppuccin;

namespace TermBar.Configuration.Json {
  [Description("The TermBar configuration for window lists. Process names must not include an extension!")]
  internal class WindowList {
    [Description("An ordered list of window process names that should be pinned, in this order, at the beginning of the window list.")]
    public List<string>? HighPriorityWindows = [
      "olk",
      "devenv",
      "librewolf"
    ];

    [Description("An ordered list of window process names that should be pinned, in this order, at the end of the window list. If a process name is in LowPriorityWindows and in HighPriorityWindows, HighPriorityWindows takes precedence.")]
    public List<string>? LowPriorityWindows = [
      "SteelSeriesEngine"
    ];

    [Description("Whether groups of windows with the same process name should be sorted alphabetically. If false, windows are simply added to the window list in the order they were created (or, when starting, in the order they are reported by the operating system).")]
    public bool SortGroupsAlphabetically = true;

    [Description("The map of window process names to ProcessIconMappings to apply to windows.")]
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

  [Description("A window list process icon mapping, which allows configuration of window attributes per process name.")]
  internal class ProcessIconMapping {
    [Description("The Catppuccin color to use for the window icon. Set to null to fall back to the window list module's AccentColor.")]
    public ColorEnum? IconColor { get; set; } = null;

    [Description("The text to use as the window icon. Set to null to fall back to the window list module's Icon.")]
    public string? Icon { get; set; } = null;

    [Description("The map of window name regular expressions to WindowNameIconMappings to apply to windows with this process name. These override IconColor and Icon. Set to null to disable this functionality.")]
    public Dictionary<string, WindowNameIconMapping>? WindowNameIconMap { get; set; } = null;

    public ProcessIconMapping() { }

    public ProcessIconMapping(ColorEnum iconColor, string icon, Dictionary<string, WindowNameIconMapping>? windowNameIconMap = null) {
      IconColor = iconColor;
      Icon = icon;
      WindowNameIconMap = windowNameIconMap;
    }
  }

  [Description("A window name icon mapping, which allows configuration of window attributes per regular expression matching the window name.")]
  internal class WindowNameIconMapping {
    [Description("The Catppuccin color to use for the window icon. Set to null to fall back to the window list module's AccentColor.")]
    public ColorEnum? IconColor { get; set; } = null;

    [Description("The text to use as the window icon. Set to null to fall back to the window list module's Icon.")]
    public string? Icon { get; set; } = null;

    public WindowNameIconMapping() { }

    public WindowNameIconMapping(ColorEnum iconColor, string icon) {
      IconColor = iconColor;
      Icon = icon;
    }
  }
}
