using Spakov.Catppuccin;
using Spakov.TermBar.Configuration.Json.SchemaAttributes;

namespace Spakov.TermBar.Configuration.Json.Modules
{
    [Description("A TermBar volume level configuration.")]
    internal class Volume : IModule
    {
        private const int OrderDefault = int.MaxValue - 1;
        private const bool ExpandDefault = false;
        private const ColorEnum AccentColorDefault = ColorEnum.Yellow;
        private const string AccentColorDefaultAsString = "Yellow";
        private const string IconDefault = "";
        private const string MutedIconDefault = "";
        private const string FormatDefault = "{0:N0}%";
        private const string MutedDefault = "";

        [Description("The order in which the module should be displayed on the TermBar.")]
        [DefaultIntNumber(OrderDefault)]
        [MinimumInt(int.MinValue)]
        [MaximumInt(int.MaxValue)]
        public int Order { get; set; } = OrderDefault;

        [Description("Whether the module should expand to take up as much space as possible.")]
        [DefaultBoolean(ExpandDefault)]
        public bool Expand { get; set; } = ExpandDefault;

        [Description("The Catppuccin color to use for the volume icon.")]
        [DefaultString(AccentColorDefaultAsString)]
        public ColorEnum AccentColor { get; set; } = AccentColorDefault;

        [Description("The text to use as the volume icon.")]
        [DefaultString(IconDefault)]
        public string Icon { get; set; } = IconDefault;

        [Description("The text to use as the muted volume icon.")]
        [DefaultString(MutedIconDefault)]
        public string MutedIcon { get; set; } = MutedIconDefault;

        [Description("The numeric format to use for the volume percentage. These are documented in https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings.")]
        [DefaultString(FormatDefault)]
        public string Format { get; set; } = FormatDefault;

        [Description("Text to be displayed in place of the volume level when muted. Set to null to always display the volume level.")]
        [DefaultString(MutedDefault)]
        public string? Muted { get; set; } = MutedDefault;

        [Description("Volume module interaction configuration. Set to null to disable interaction.")]
        public VolumeInteraction? VolumeInteraction { get; set; } = new();
    }

    [Description("The volume module interaction configuration.")]
    internal class VolumeInteraction
    {
        private const bool ClickToToggleMuteDefault = true;
        private const bool ScrollToAdjustDefault = true;
        private const int PercentPerScrollDefault = 2;

        [Description("Whether clicking the volume module toggles the mute state.")]
        [DefaultBoolean(ClickToToggleMuteDefault)]
        public bool ClickToToggleMute { get; set; } = ClickToToggleMuteDefault;

        [Description("Whether scrolling over the volume module adjusts the volume level.")]
        [DefaultBoolean(ScrollToAdjustDefault)]
        public bool ScrollToAdjust { get; set; } = ScrollToAdjustDefault;

        [Description("The volume level change per scroll. Does nothing if ScrollToAdjust is false.")]
        [DefaultIntNumber(PercentPerScrollDefault)]
        [MinimumInt(0)]
        [MaximumInt(100)]
        public int PercentPerScroll { get; set; } = PercentPerScrollDefault;
    }
}