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
  }
}
