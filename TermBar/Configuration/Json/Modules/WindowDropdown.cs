using Spakov.Catppuccin;
using Spakov.TermBar.Configuration.Json.SchemaAttributes;

namespace Spakov.TermBar.Configuration.Json.Modules {
  [Description("A TermBar window dropdown configuration.")]
  internal class WindowDropdown : IModule {
    private const int orderDefault = 101;
    private const bool expandDefault = false;
    private const ColorEnum accentColorDefault = ColorEnum.Mauve;
    private const string accentColorDefaultAsString = "Mauve";
    private const string dropdownIconDefault = "";
    private const string iconDefault = "•";

    [Description("The order in which the module should be displayed on the TermBar.")]
    [DefaultIntNumber(orderDefault)]
    [MinimumInt(int.MinValue)]
    [MaximumInt(int.MaxValue)]
    public int Order { get; set; } = orderDefault;

    [Description("Whether the module should expand to take up as much space as possible.")]
    [DefaultBoolean(expandDefault)]
    public bool Expand { get; set; } = expandDefault;

    [Description("The Catppuccin color to use for the dropdown icon color.")]
    [DefaultString(accentColorDefaultAsString)]
    public ColorEnum AccentColor { get; set; } = accentColorDefault;

    [Description("The text to use as the dropdown icon.")]
    [DefaultString(dropdownIconDefault)]
    public string DropdownIcon { get; set; } = dropdownIconDefault;

    [Description("The text to use as window icons by default.")]
    [DefaultString(iconDefault)]
    public string Icon { get; set; } = iconDefault;
  }
}