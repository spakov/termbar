using TermBar.Catppuccin;
using TermBar.Configuration.Json.SchemaAttributes;

namespace TermBar.Configuration.Json.Modules {
  [Description("A TermBar CPU monitor configuration.")]
  internal class Cpu : IModule {
    private const int orderDefault = int.MaxValue - 2;
    private const bool expandDefault = false;
    private const ColorEnum accentColorDefault = ColorEnum.Pink;
    private const string accentColorDefaultAsString = "Pink";
    private const string iconDefault = "";
    private const string formatDefault = "{0:N0}%";
    private const bool roundDefault = true;
    private const int updateIntervalDefault = 5000;

    [Description("The order in which the module should be displayed on the TermBar.")]
    [DefaultIntNumber(orderDefault)]
    [MinimumInt(int.MinValue)]
    [MaximumInt(int.MaxValue)]
    public int Order { get; set; } = orderDefault;

    [Description("Whether the module should expand to take up as much space as possible.")]
    [DefaultBoolean(expandDefault)]
    public bool Expand { get; set; } = expandDefault;

    [Description("The Catppuccin color to use for the CPU icon.")]
    [DefaultString(accentColorDefaultAsString)]
    public ColorEnum AccentColor { get; set; } = accentColorDefault;

    [Description("The text to use as the CPU icon.")]
    [DefaultString(iconDefault)]
    public string Icon { get; set; } = iconDefault;

    [Description("The numeric format to use for the CPU percentage. These are documented in https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings.")]
    [DefaultString(formatDefault)]
    public string Format { get; set; } = formatDefault;

    [Description("Whether to round the CPU percentage before formatting.")]
    [DefaultBoolean(roundDefault)]
    public bool Round { get; set; } = roundDefault;

    [Description("The CPU usage update interval, in milliseconds.")]
    [DefaultIntNumber(updateIntervalDefault)]
    [MinimumInt(1)]
    public int UpdateInterval { get; set; } = updateIntervalDefault;
  }
}
