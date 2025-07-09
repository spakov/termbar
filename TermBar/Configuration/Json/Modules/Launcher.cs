using Catppuccin;
using System.Collections.Generic;
using TermBar.Configuration.Json.SchemaAttributes;

namespace TermBar.Configuration.Json.Modules {
  [Description("A TermBar launcher configuration.")]
  internal class Launcher : IModule {
    private const int orderDefault = -1;
    private const bool expandDefault = false;
    private const ColorEnum accentColorDefault = ColorEnum.Rosewater;
    private const string accentColorDefaultAsString = "Rosewater";
    private const string iconDefault = "•";

    [Description("The order in which the module should be displayed on the TermBar.")]
    [DefaultIntNumber(orderDefault)]
    [MinimumInt(int.MinValue)]
    [MaximumInt(int.MaxValue)]
    public int Order { get; set; } = orderDefault;

    [Description("Whether the module should expand to take up as much space as possible.")]
    [DefaultBoolean(expandDefault)]
    public bool Expand { get; set; } = expandDefault;

    [Description("The Catppuccin color to use as an accent.")]
    [DefaultString(accentColorDefaultAsString)]
    public ColorEnum AccentColor { get; set; } = accentColorDefault;

    [Description("The text to use as launcher icons by default.")]
    [DefaultString(iconDefault)]
    public string Icon { get; set; } = iconDefault;

    [Description("Launcher entries.")]
    public List<LauncherEntry> LauncherEntries { get; set; } = [
      new() {
        Name = "Windows Terminal",
        Command = "wt",
        IconColor = ColorEnum.Overlay0,
        Icon = ""
      },
      new() {
        Name = "File Explorer",
        Command = "explorer"
      }
    ];
  }

  [Description("A TermBar launcher entry.")]
  internal class LauncherEntry {
    private const bool displayNameDefault = false;

    [Description("The name of the launcher entry. This is always displayed as a tooltip and is optionally displayed in the launcher button if DisplayName is true.")]
    public required string Name { get; set; }

    [Description("The command to shell execute.")]
    public required string Command { get; set; }

    [Description("Arguments to the command to shell execute. Set to null for no arguments.")]
    [DefaultNull]
    public string[]? CommandArguments { get; set; } = null;

    [Description("Whether to display Name in the launcher button.")]
    [DefaultBoolean(displayNameDefault)]
    public bool DisplayName { get; set; } = displayNameDefault;

    [Description("The Catppuccin color to use for the launcher icon. Set to null to use matches in ProcessIconMap and fall back to AccentColor.")]
    [DefaultNull]
    public ColorEnum? IconColor { get; set; } = null;

    [Description("The text to use as the launcher icon. Set to null to use matches in ProcessIconMap and fall back to Icon.")]
    [DefaultNull]
    public string? Icon { get; set; } = null;
  }
}
