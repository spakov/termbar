using TermBar.Catppuccin;

namespace TermBar.Configuration.Json.Modules {
  /// <summary>
  /// A TermBar window bar configuration.
  /// </summary>
  internal class WindowBar : IModule {
    public int Order { get; set; } = 100;

    public bool Expand { get; set; } = true;

    /// <summary>
    /// The Catppuccin color to use for window icons.
    /// </summary>
    public ColorEnum AccentColor { get; set; } = ColorEnum.Mauve;

    /// <summary>
    /// The text to use as window icons by default.
    /// </summary>
    public string Icon { get; set; } = "•";

    /// <summary>
    /// Windows configuration.
    /// </summary>
    public Windows? Windows { get; set; } = new();
  }

  /// <summary>
  /// A TermBar window bar windows configuration.
  /// </summary>
  internal class Windows {
    /// <summary>
    /// Keep windows set to a fixed size, in pixels.
    /// </summary>
    /// <remarks>Set to <c>null</c> for variable size.</remarks>
    public uint? FixedSize { get; set; } = null;

    /// <summary>
    /// The maximum length of a window, in pixels.
    /// </summary>
    /// <remarks>Set to <c>null</c> for no maximum length.</remarks>
    public uint? MaxLength { get; set; } = 300;

    /// <summary>
    /// Whether the window name should be truncated with … if it's too long.
    /// </summary>
    public bool NiceTruncation { get; set; } = true;
  }
}
