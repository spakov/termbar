namespace TermBar.Configuration.Json {
  /// <summary>
  /// A TermBar configuration file display.
  /// </summary>
  internal class Display {
    /// <summary>
    /// The display ID.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// The TermBar configuration for the display.
    /// </summary>
    public required TermBar TermBar { get; set; }
  }
}
