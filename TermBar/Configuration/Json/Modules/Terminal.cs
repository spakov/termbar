using TermBar.Catppuccin;

namespace TermBar.Configuration.Json.Modules {
  /// <summary>
  /// A TermBar terminal configuration.
  /// </summary>
  internal class Terminal : IModule {
    public int Order { get; set; } = 0;

    public bool Expand { get; set; } = false;

    /// <summary>
    /// The Catppuccin color to use for the terminal icon.
    /// </summary>
    public ColorEnum AccentColor { get; set; } = ColorEnum.Lavender;

    /// <summary>
    /// The text to use as the terminal icon.
    /// </summary>
    public string Icon { get; set; } = "";

    /// <summary>
    /// The text to use as the terminal visual bell icon.
    /// </summary>
    public string VisualBellIcon { get; set; } = "";

    /// <summary>
    /// The command to run in the terminal.
    /// </summary>
    /// <remarks>This is typically a shell.</remarks>
    public string Command { get; set; } = "pwsh.exe";

    /// <summary>
    /// Whether to restart <see cref="Command"/> when it exits normally.
    /// </summary>
    public bool RestartOnExit { get; set; } = true;

    /// <summary>
    /// The number of rows in the terminal.
    /// </summary>
    public uint Rows { get; set; } = 3;

    /// <summary>
    /// The number of columsn in the terminal.
    /// </summary>
    public uint Columns { get; set; } = 80;
  }
}
