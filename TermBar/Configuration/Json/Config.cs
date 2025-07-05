using System.Collections.Generic;

namespace TermBar.Configuration.Json {
  /// <summary>
  /// The TermBar configuration file.
  /// </summary>
  internal class Config {
    /// <summary>
    /// A list of displays.
    /// </summary>
    public required List<Display> Displays { get; set; }

    /// <summary>
    /// The directory to change to when starting.
    /// </summary>
    /// <remarks>Set to <see langword="null"/> to keep the directory inherited
    /// from the parent process.</remarks>
    public string? StartDirectory { get; set; } = "%USERPROFILE%";
  }
}
