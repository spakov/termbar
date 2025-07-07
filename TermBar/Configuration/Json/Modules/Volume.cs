using System.ComponentModel;
using TermBar.Catppuccin;

namespace TermBar.Configuration.Json.Modules {
  [Description("A TermBar volume level configuration.")]
  internal class Volume : IModule {
    [Description("The order in which the module should be displayed on the TermBar.")]
    public int Order { get; set; } = int.MaxValue - 1;

    [Description("Whether the module should expand to take up as much space as possible.")]
    public bool Expand { get; set; } = false;

    [Description("The Catppuccin color to use for the volume icon.")]
    public ColorEnum AccentColor { get; set; } = ColorEnum.Yellow;

    [Description("The text to use as the volume icon.")]
    public string Icon { get; set; } = "";

    [Description("The text to use as the muted volume icon.")]
    public string MutedIcon { get; set; } = "";

    [Description("The numeric format to use for the volume percentage. These are documented in https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings.")]
    public string Format { get; set; } = "{0:N0}%";

    [Description("Text to be displayed in place of the volume level when muted. Set to null to always display the volume level.")]
    public string? Muted { get; set; } = "";

    [Description("Volume module interaction configuration. Set to null to disable interaction.")]
    public VolumeInteraction? VolumeInteraction { get; set; } = new();
  }

  [Description("The volume module interaction configuration.")]
  internal class VolumeInteraction {
    [Description("Whether clicking the volume module toggles the mute state.")]
    public bool ClickToToggleMute { get; set; } = true;

    [Description("Whether scrolling over the volume module adjusts the volume level.")]
    public bool ScrollToAdjust { get; set; } = true;

    [Description("The volume level change per scroll. Does nothing if ScrollToAdjust is false.")]
    public uint PercentPerScroll { get; set; } = 2;
  }
}
