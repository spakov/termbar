using System.ComponentModel;
using TermBar.Catppuccin;

namespace TermBar.Configuration.Json.Modules {
  [Description("A TermBar window bar configuration.")]
  internal class WindowBar : IModule {
    [Description("The order in which the module should be displayed on the TermBar.")]
    public int Order { get; set; } = 100;

    [Description("Whether the module should expand to take up as much space as possible.")]
    public bool Expand { get; set; } = true;

    [Description("The Catppuccin color to use for window icon colors by default.")]
    public ColorEnum AccentColor { get; set; } = ColorEnum.Mauve;

    [Description("The text to use as window icons by default.")]
    public string Icon { get; set; } = "•";

    [Description("Windows configuration.")]
    public Windows? Windows { get; set; } = new();
  }

  [Description("A TermBar window bar windows configuration.")]
  internal class Windows {
    [Description("Whether to display windows as a fixed size.")]
    public uint? FixedSize { get; set; } = null;

    [Description("The maximum length of a window, in pixels.")]
    public uint? MaxLength { get; set; } = 300;

    [Description("Whether the window name should be truncated with … if it's too long.")]
    public bool NiceTruncation { get; set; } = true;
  }
}
