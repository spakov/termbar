using Catppuccin;
using System.Collections.Generic;
using TermBar.Configuration.Json.SchemaAttributes;

namespace TermBar.Configuration.Json {
  [Description("The TermBar configuration for window lists. Process names must not include an extension!")]
  internal class WindowList {
    [Description("An ordered list of window process names that should be pinned, in this order, at the beginning of the window list. Set to null to disable this behavior.")]
    public List<string>? HighPriorityWindows = [
      "olk",
      "devenv",
      "librewolf"
    ];

    [Description("An ordered list of window process names that should be pinned, in this order, at the end of the window list. Set to null to disable this behavior. If a process name is in LowPriorityWindows and in HighPriorityWindows, HighPriorityWindows takes precedence.")]
    public List<string>? LowPriorityWindows = [
      "SteelSeriesEngine"
    ];

    [Description("Whether groups of windows with the same process name should be sorted alphabetically. If false, windows are simply added to the window list in the order they were created (or, when starting, in the order they are reported by the operating system).")]
    public bool SortGroupsAlphabetically = true;

    [Description("The map of window process names to ProcessIconMappings to apply to windows. Set to null to disable this behavior.")]
    public Dictionary<string, ProcessIconMapping>? ProcessIconMap { get; set; } = new() {
      {
        "devenv",
        new() {
          IconColor = ColorEnum.Mauve,
          Icon = ""
        }
      },
      {
        "explorer",
        new() {
          IconColor = ColorEnum.Yellow,
          Icon = ""
        }
      },
      {
        "librewolf",
        new() {
          IconColor = ColorEnum.Sky,
          Icon = "",
          WindowNameIconMap = new() {
            {
              "Microsoft Learn",
              new() {
                IconColor = ColorEnum.Mauve,
                Icon = ""
              }
            },
            {
              "Stardew Valley Wiki",
              new() {
                IconColor = ColorEnum.Peach,
                Icon = "󱐟"
              }
            }
          }
        }
      },
      {
        "olk",
        new() {
          IconColor = ColorEnum.Sapphire,
          Icon = ""
        }
      },
      {
        "steamwebhelper",
        new() {
          IconColor = ColorEnum.Blue,
          Icon = ""
        }
      },
      {
        "WindowsTerminal",
        new() {
          IconColor = ColorEnum.Overlay0,
          Icon = ""
        }
      }
    };
  }

  [Description("A window list process icon mapping, which allows configuration of window attributes per process name.")]
  internal class ProcessIconMapping {
    [Description("The Catppuccin color to use for the window icon. Set to null to fall back to the window list module’s AccentColor.")]
    [DefaultNull]
    public ColorEnum? IconColor { get; set; } = null;

    [Description("The text to use as the window icon. Set to null to fall back to the window list module’s Icon.")]
    [DefaultNull]
    public string? Icon { get; set; } = null;

    [Description("The map of window name regular expressions to WindowNameIconMappings to apply to windows with this process name. Set to null to disable this functionality. These override IconColor and Icon.")]
    [DefaultNull]
    public Dictionary<string, WindowNameIconMapping>? WindowNameIconMap { get; set; } = null;
  }

  [Description("A window name icon mapping, which allows configuration of window attributes per regular expression matching the window name.")]
  internal class WindowNameIconMapping {
    [Description("The Catppuccin color to use for the window icon. Set to null to fall back to the window list module’s AccentColor.")]
    [DefaultNull]
    public ColorEnum? IconColor { get; set; } = null;

    [Description("The text to use as the window icon. Set to null to fall back to the window list module’s Icon.")]
    [DefaultNull]
    public string? Icon { get; set; } = null;
  }
}
