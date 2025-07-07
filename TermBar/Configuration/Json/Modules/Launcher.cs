using System.Collections.Generic;
using System.ComponentModel;
using TermBar.Catppuccin;

namespace TermBar.Configuration.Json.Modules {
  [Description("A TermBar launcher configuration.")]
  internal class Launcher : IModule {
    [Description("The order in which the module should be displayed on the TermBar.")]
    public int Order { get; set; } = -1;

    [Description("Whether the module should expand to take up as much space as possible.")]
    public bool Expand { get; set; } = false;

    [Description("The Catppuccin color to use as an accent.")]
    public ColorEnum AccentColor { get; set; } = ColorEnum.Rosewater;

    [Description("The text to use as launcher icons by default.")]
    public string Icon { get; set; } = "•";

    [Description("Launcher entries.")]
    public List<LauncherEntry> LauncherEntries { get; set; } = [
      new("Windows Terminal", "wt", iconColor: ColorEnum.Overlay0, icon: ""),
      new("File Explorer", "explorer")
    ];
  }

  [Description("A TermBar launcher entry.")]
  internal class LauncherEntry {
    [Description("The name of the launcher entry. This is always displayed as a tooltip and is optionally displayed in the launcher button if DisplayName is true.")]
    public string? Name { get; set; } = null;

    [Description("The command to shell execute.")]
    public string? Command { get; set; } = null;

    [Description("Arguments to the command to shell execute.")]
    public string[]? CommandArguments { get; set; } = null;

    [Description("Whether to display Name in the launcher button.")]
    public bool DisplayName { get; set; } = false;

    [Description("The Catppuccin color to use for the launcher icon. Set to null to use matches in ProcessIconMap and fall back to AccentColor.")]
    public ColorEnum? IconColor { get; set; } = null;

    [Description("The text to use as the launcher icon. Set to null to use matches in ProcessIconMap and fall back to Icon.")]
    public string? Icon { get; set; } = null;

    public LauncherEntry() { }

    public LauncherEntry(string name, string command, string[]? commandArguments = null, ColorEnum? iconColor = null, string? icon = null, bool displayName = false) {
      Name = name;
      Command = command;
      CommandArguments = commandArguments;
      DisplayName = displayName;
      IconColor = iconColor;
      Icon = icon;
    }
  }
}
