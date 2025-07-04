using TermBar.Catppuccin;

namespace TermBar.Configuration.Json {
  internal interface IModule {
    /// <summary>
    /// The order in which the module should be displayed.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Whether the module should expand to take up as much space as possible.
    /// </summary>
    public bool Expand { get; set; }

    /// <summary>
    /// The Catppuccin color to use as an accent color.
    /// </summary>
    public ColorEnum AccentColor { get; set; }
  }
}
