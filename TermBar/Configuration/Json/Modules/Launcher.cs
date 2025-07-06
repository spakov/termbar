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
    /// Launcher entries.
    /// </summary>
    public List<LauncherEntry> LauncherEntries { get; set; } = [
      new("Windows Terminal", "wt", ColorEnum.Overlay0, ""),
      new("File Explorer", "explorer", ColorEnum.Yellow, "")
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
    /// Whether to display <see cref="Name"/>.
    /// </summary>
    public bool DisplayName { get; set; } = false;

    /// <summary>
    /// The Catppuccin color to use for the launcher icon.
    /// </summary>
    /// <remarks>Set to <c>null</c> to use the launcher module's <see
    /// cref="Launcher.AccentColor"/>.</remarks>
    public ColorEnum? IconColor { get; set; } = null;

    /// <summary>
    /// The text to use as the launcher icon.
    /// </summary>
    /// <remarks>Set to <c>null</c> to use the WindowList module's <see
    /// cref="Launcher.Icon"/>.</remarks>
    public string? Icon { get; set; } = null;

    public LauncherEntry() { }

    public LauncherEntry(string name, string command, ColorEnum iconColor, string icon, bool displayName = false) {
      Name = name;
      Command = command;
      DisplayName = displayName;
      IconColor = iconColor;
      Icon = icon;
    }
  }
}
