using System.ComponentModel;
using TermBar.Catppuccin;

namespace TermBar.Configuration.Json.Modules {
  [Description("A TermBar window dropdown configuration.")]
  internal class WindowDropdown : IModule {
    [Description("The order in which the module should be displayed on the TermBar.")]
    public int Order { get; set; } = 101;

    [Description("Whether the module should expand to take up as much space as possible.")]
    public bool Expand { get; set; } = false;

    [Description("The Catppuccin color to use for the dropdown icon color.")]
    public ColorEnum AccentColor { get; set; } = ColorEnum.Mauve;

    [Description("The text to use as the dropdown icon.")]
    public string DropdownIcon { get; set; } = "";

    [Description("The text to use as window icons by default.")]
    public string Icon { get; set; } = "•";
  }
}
