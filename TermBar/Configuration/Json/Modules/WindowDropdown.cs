using Spakov.Catppuccin;
using Spakov.TermBar.Configuration.Json.SchemaAttributes;

namespace Spakov.TermBar.Configuration.Json.Modules
{
    [Description("A TermBar window dropdown configuration.")]
    internal class WindowDropdown : IModule
    {
        private const int OrderDefault = 101;
        private const bool ExpandDefault = false;
        private const ColorEnum AccentColorDefault = ColorEnum.Mauve;
        private const string AccentColorDefaultAsString = "Mauve";
        private const string DropdownIconDefault = "";
        private const string IconDefault = "•";

        [Description("The order in which the module should be displayed on the TermBar.")]
        [DefaultIntNumber(OrderDefault)]
        [MinimumInt(int.MinValue)]
        [MaximumInt(int.MaxValue)]
        public int Order { get; set; } = OrderDefault;

        [Description("Whether the module should expand to take up as much space as possible.")]
        [DefaultBoolean(ExpandDefault)]
        public bool Expand { get; set; } = ExpandDefault;

        [Description("The Catppuccin color to use for the dropdown icon color.")]
        [DefaultString(AccentColorDefaultAsString)]
        public ColorEnum AccentColor { get; set; } = AccentColorDefault;

        [Description("The text to use as the dropdown icon.")]
        [DefaultString(DropdownIconDefault)]
        public string DropdownIcon { get; set; } = DropdownIconDefault;

        [Description("The text to use as window icons by default.")]
        [DefaultString(IconDefault)]
        public string Icon { get; set; } = IconDefault;
    }
}