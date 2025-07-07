using System.ComponentModel;
using TermBar.Catppuccin;

namespace TermBar.Configuration.Json.Modules {
  [Description("A TermBar static text display configuration.")]
  internal class StaticText : IModule {
    [Description("The order in which the module should be displayed on the TermBar.")]
    public int Order { get; set; } = 0;

    [Description("Whether the module should expand to take up as much space as possible.")]
    public bool Expand { get; set; } = false;

    [Description("The Catppuccin color to use for the icon.")]
    public ColorEnum AccentColor { get; set; } = ColorEnum.Green;

    [Description("The text to use as the icon.")]
    public string Icon { get; set; } = "";

    [Description("The static text to display.")]
    public string Text { get; set; } = "Hello, world!";
  }
}
