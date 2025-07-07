using TermBar.Catppuccin;
using TermBar.Configuration.Json.SchemaAttributes;

namespace TermBar.Configuration.Json.Modules {
  [Description("A TermBar static text display configuration.")]
  internal class StaticText : IModule {
    private const int orderDefault = int.MinValue;
    private const bool expandDefault = false;
    private const ColorEnum accentColorDefault = ColorEnum.Green;
    private const string accentColorDefaultAsString = "Green";
    private const string iconDefault = "";
    private const string textDefault = "Hello, world!";

    [Description("The order in which the module should be displayed on the TermBar.")]
    [DefaultIntNumber(orderDefault)]
    [MinimumInt(int.MinValue)]
    [MaximumInt(int.MaxValue)]
    public int Order { get; set; } = orderDefault;

    [Description("Whether the module should expand to take up as much space as possible.")]
    [DefaultBoolean(expandDefault)]
    public bool Expand { get; set; } = expandDefault;

    [Description("The Catppuccin color to use for the icon.")]
    [DefaultString(accentColorDefaultAsString)]
    public ColorEnum AccentColor { get; set; } = accentColorDefault;

    [Description("The text to use as the icon.")]
    [DefaultString(iconDefault)]
    public string Icon { get; set; } = iconDefault;

    [Description("The static text to display.")]
    [DefaultString(textDefault)]
    public string Text { get; set; } = textDefault;
  }
}
