using TermBar.Catppuccin;
using TermBar.Configuration.Json.SchemaAttributes;

namespace TermBar.Configuration.Json.Modules {
  [Description("A TermBar volume level configuration.")]
  internal class Volume : IModule {
    private const int orderDefault = int.MaxValue - 1;
    private const bool expandDefault = false;
    private const ColorEnum accentColorDefault = ColorEnum.Yellow;
    private const string accentColorDefaultAsString = "Yellow";
    private const string iconDefault = "";
    private const string mutedIconDefault = "";
    private const string formatDefault = "{0:N0}%";
    private const string mutedDefault = "";

    [Description("The order in which the module should be displayed on the TermBar.")]
    [DefaultIntNumber(orderDefault)]
    [MinimumInt(int.MinValue)]
    [MaximumInt(int.MaxValue)]
    public int Order { get; set; } = orderDefault;

    [Description("Whether the module should expand to take up as much space as possible.")]
    [DefaultBoolean(expandDefault)]
    public bool Expand { get; set; } = expandDefault;

    [Description("The Catppuccin color to use for the volume icon.")]
    [DefaultString(accentColorDefaultAsString)]
    public ColorEnum AccentColor { get; set; } = accentColorDefault;

    [Description("The text to use as the volume icon.")]
    [DefaultString(iconDefault)]
    public string Icon { get; set; } = iconDefault;

    [Description("The text to use as the muted volume icon.")]
    [DefaultString(mutedIconDefault)]
    public string MutedIcon { get; set; } = mutedIconDefault;

    [Description("The numeric format to use for the volume percentage. These are documented in https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings.")]
    [DefaultString(formatDefault)]
    public string Format { get; set; } = formatDefault;

    [Description("Text to be displayed in place of the volume level when muted. Set to null to always display the volume level.")]
    [DefaultString(mutedDefault)]
    public string? Muted { get; set; } = mutedDefault;

    [Description("Volume module interaction configuration. Set to null to disable interaction.")]
    public VolumeInteraction? VolumeInteraction { get; set; } = new();
  }

  [Description("The volume module interaction configuration.")]
  internal class VolumeInteraction {
    private const bool clickToToggleMuteDefault = true;
    private const bool scrollToAdjustDefault = true;
    private const int percentPerScrollDefault = 2;

    [Description("Whether clicking the volume module toggles the mute state.")]
    [DefaultBoolean(clickToToggleMuteDefault)]
    public bool ClickToToggleMute { get; set; } = clickToToggleMuteDefault;

    [Description("Whether scrolling over the volume module adjusts the volume level.")]
    [DefaultBoolean(scrollToAdjustDefault)]
    public bool ScrollToAdjust { get; set; } = scrollToAdjustDefault;

    [Description("The volume level change per scroll. Does nothing if ScrollToAdjust is false.")]
    [DefaultIntNumber(percentPerScrollDefault)]
    [MinimumInt(0)]
    [MaximumInt(100)]
    public int PercentPerScroll { get; set; } = percentPerScrollDefault;
  }
}
