using System.Collections.Generic;
using TermBar.Catppuccin;

namespace TermBar.Configuration.Json.Modules {
  /// <summary>
  /// A TermBar launcher configuration.
  /// </summary>
  internal class Launcher : IModule {
    public int Order { get; set; } = -1;

    public bool Expand { get; set; } = false;

    /// <summary>
    /// The Catppuccin color to use as the default for launcher entry icons.
    /// </summary>
    public ColorEnum AccentColor { get; set; } = ColorEnum.Rosewater;

    /// <summary>
    /// The text to use as launcher icons by default.
    /// </summary>
    public string Icon { get; set; } = "•";

    /// <summary>
    /// Launcher entries.
    /// </summary>
    public List<LauncherEntry> LauncherEntries { get; set; } = [
      new("Windows Terminal", "wt", iconColor: ColorEnum.Overlay0, icon: ""),
      new("File Explorer", "explorer")
    ];
  }

  /// <summary>
  /// A TermBar launcher entry configuration.
  /// </summary>
  internal class LauncherEntry {
    /// <summary>
    /// The name of the launcher entry.
    /// </summary>
    /// <remarks>This is always displayed as a tooltip and is optionally
    /// displayed in the launcher button if <see cref="DisplayName"/> is <see
    /// langword="true"/>.</remarks>
    public string? Name { get; set; } = null;

    /// <summary>
    /// The command to shell execute.
    /// </summary>
    public string? Command { get; set; } = null;

    /// <summary>
    /// Arguments to the command to shell execute.
    /// </summary>
    public string[]? CommandArguments { get; set; } = null;

    /// <summary>
    /// Whether to display <see cref="Name"/>.
    /// </summary>
    public bool DisplayName { get; set; } = false;

    /// <summary>
    /// The Catppuccin color to use for the launcher icon.
    /// </summary>
    /// <remarks>Set to <see langword="null"/> to use matches in <see
    /// cref="WindowList.ProcessIconMap"/> and fall back to the launcher
    /// module's <see cref="Launcher.AccentColor"/>.</remarks>
    public ColorEnum? IconColor { get; set; } = null;

    /// <summary>
    /// The text to use as the launcher icon.
    /// </summary>
    /// <remarks>Set to <see langword="null"/> to use matches in <see
    /// cref="WindowList.ProcessIconMap"/> and fall back to the launcher
    /// module's <see cref="Launcher.Icon"/>.</remarks>
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
