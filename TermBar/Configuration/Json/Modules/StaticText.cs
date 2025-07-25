using Spakov.Catppuccin;
using Spakov.TermBar.Configuration.Json.SchemaAttributes;

namespace Spakov.TermBar.Configuration.Json.Modules
{
    [Description("A TermBar static text display configuration.")]
    internal class StaticText : IModule
    {
        private const int OrderDefault = -1;
        private const bool ExpandDefault = false;
        private const ColorEnum AccentColorDefault = ColorEnum.Green;
        private const string AccentColorDefaultAsString = "Green";
        private const string IconDefault = "";
        private const string TextDefault = "Hello, world!";

        [Description("The order in which the module should be displayed on the TermBar.")]
        [DefaultIntNumber(OrderDefault)]
        [MinimumInt(int.MinValue)]
        [MaximumInt(int.MaxValue)]
        public int Order { get; set; } = OrderDefault;

        [Description("Whether the module should expand to take up as much space as possible.")]
        [DefaultBoolean(ExpandDefault)]
        public bool Expand { get; set; } = ExpandDefault;

        [Description("The Catppuccin color to use for the icon.")]
        [DefaultString(AccentColorDefaultAsString)]
        public ColorEnum AccentColor { get; set; } = AccentColorDefault;

        [Description("The text to use as the icon.")]
        [DefaultString(IconDefault)]
        public string Icon { get; set; } = IconDefault;

        [Description("The static text to display.")]
        [DefaultString(TextDefault)]
        public string Text { get; set; } = TextDefault;
    }
}