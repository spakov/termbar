using TermBar.Catppuccin;
using TermBar.Configuration.Json.SchemaAttributes;

namespace TermBar.Configuration.Json.Modules {
  [Description("A TermBar window bar configuration.")]
  internal class WindowBar : IModule {
    private const int orderDefault = 100;
    private const bool expandDefault = true;
    private const ColorEnum accentColorDefault = ColorEnum.Mauve;
    private const string accentColorDefaultAsString = "Mauve";
    private const string iconDefault = "•";

    [Description("The order in which the module should be displayed on the TermBar.")]
    [DefaultIntNumber(orderDefault)]
    [MinimumInt(int.MinValue)]
    [MaximumInt(int.MaxValue)]
    public int Order { get; set; } = orderDefault;

    [Description("Whether the module should expand to take up as much space as possible.")]
    [DefaultBoolean(expandDefault)]
    public bool Expand { get; set; } = expandDefault;

    [Description("The Catppuccin color to use for window icon colors by default.")]
    [DefaultString(accentColorDefaultAsString)]
    public ColorEnum AccentColor { get; set; } = accentColorDefault;

    [Description("The text to use as window icons by default.")]
    [DefaultString(iconDefault)]
    public string Icon { get; set; } = iconDefault;

    [Description("Windows configuration.")]
    public Windows? Windows { get; set; } = new();
  }

  [Description("A TermBar window bar windows configuration.")]
  internal class Windows {
    private const int maxLengthDefault = 300;
    private const bool niceTruncationDefault = true;

    [Description("The fixed length of windows, in pixels. Set to null for variable size.")]
    [DefaultNull]
    [MinimumInt(1)]
    public int? FixedLength { get; set; } = null;

    [Description("The maximum length of a window, in pixels. Set to null for no maximum length.")]
    [DefaultIntNumber(maxLengthDefault)]
    [MinimumInt(1)]
    public int? MaxLength { get; set; } = maxLengthDefault;

    [Description("Whether the window name should be truncated with … if it’s too long.")]
    [DefaultBoolean(niceTruncationDefault)]
    public bool NiceTruncation { get; set; } = niceTruncationDefault;
  }
}
