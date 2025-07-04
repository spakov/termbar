using TermBar.Catppuccin;

namespace TermBar.Configuration.Json.Modules {
  /// <summary>
  /// A TermBar volume configuration.
  /// </summary>
  internal class Volume : IModule {
    public int Order { get; set; } = int.MaxValue - 1;

    public bool Expand { get; set; } = false;

    /// <summary>
    /// The Catppuccin color to use for the volume icon.
    /// </summary>
    public ColorEnum AccentColor { get; set; } = ColorEnum.Yellow;

    /// <summary>
    /// The text to use as the volume icon.
    /// </summary>
    public string Icon { get; set; } = "";

    /// <summary>
    /// The text to use as the muted volume icon.
    /// </summary>
    public string MutedIcon { get; set; } = "";

    /// <summary>
    /// The numeric format to use, as in
    /// https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings.
    /// </summary>
    public string Format { get; set; } = "{0:N0}%";

    /// <summary>
    /// The text to use when the volume is muted.
    /// </summary>
    /// <remarks>Set to <c>null</c> to use <see cref="Format"/>
    /// instead.</remarks>
    public string? Muted { get; set; } = "";

    /// <summary>
    /// Volume module interaction configuration.
    /// </summary>
    /// <remarks>Set to <c>null</c> to disable the interaction.</remarks>
    public VolumeInteraction? VolumeInteraction { get; set; } = new();
  }

  internal class VolumeInteraction {
    /// <summary>
    /// Whether clicking the volume toggles the mute state.
    /// </summary>
    public bool ClickToToggleMute { get; set; } = true;

    /// <summary>
    /// Whether scrolling over the volume adjusts it.
    /// </summary>
    public bool ScrollToAdjust { get; set; } = true;

    /// <summary>
    /// The volume percentage change per scroll.
    /// </summary>
    public uint PercentPerScroll { get; set; } = 2;
  }
}
