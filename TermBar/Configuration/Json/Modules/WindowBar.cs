using Spakov.Catppuccin;
using Spakov.TermBar.Configuration.Json.SchemaAttributes;

namespace Spakov.TermBar.Configuration.Json.Modules
{
    [Description("A TermBar window bar configuration.")]
    internal class WindowBar : IModule
    {
        private const int OrderDefault = 100;
        private const bool ExpandDefault = true;
        private const ColorEnum AccentColorDefault = ColorEnum.Mauve;
        private const string AccentColorDefaultAsString = "Mauve";
        private const string IconDefault = "•";

        [Description("The order in which the module should be displayed on the TermBar.")]
        [DefaultIntNumber(OrderDefault)]
        [MinimumInt(int.MinValue)]
        [MaximumInt(int.MaxValue)]
        public int Order { get; set; } = OrderDefault;

        [Description("Whether the module should expand to take up as much space as possible.")]
        [DefaultBoolean(ExpandDefault)]
        public bool Expand { get; set; } = ExpandDefault;

        [Description("The Catppuccin color to use for window icon colors by default.")]
        [DefaultString(AccentColorDefaultAsString)]
        public ColorEnum AccentColor { get; set; } = AccentColorDefault;

        [Description("The text to use as window icons by default.")]
        [DefaultString(IconDefault)]
        public string Icon { get; set; } = IconDefault;

        [Description("Windows configuration. Set to null to disable these features and allow system default behaviors to take place.")]
        public Windows? Windows { get; set; } = new();
    }

    [Description("A TermBar window bar windows configuration.")]
    internal class Windows
    {
        private const int MaxLengthDefault = 300;
        private const bool NiceTruncationDefault = true;
        private const bool ScrollIntoViewDefault = true;

        [Description("The fixed length of windows, in pixels. Set to null for variable size.")]
        [DefaultNull]
        [MinimumInt(1)]
        public int? FixedLength { get; set; } = null;

        [Description("The maximum length of a window, in pixels. Set to null for no maximum length.")]
        [DefaultIntNumber(MaxLengthDefault)]
        [MinimumInt(1)]
        public int? MaxLength { get; set; } = MaxLengthDefault;

        [Description("Whether the window name should be truncated with … if it’s too long.")]
        [DefaultBoolean(NiceTruncationDefault)]
        public bool NiceTruncation { get; set; } = NiceTruncationDefault;

        [Description("Whether to scroll the window list to show the selected window.")]
        [DefaultBoolean(ScrollIntoViewDefault)]
        public bool ScrollIntoView { get; set; } = ScrollIntoViewDefault;
    }
}