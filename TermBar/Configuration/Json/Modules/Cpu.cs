using System.ComponentModel;
using TermBar.Catppuccin;

namespace TermBar.Configuration.Json.Modules {
  [Description("A TermBar CPU monitor configuration.")]
  internal class Cpu : IModule {
    [Description("The order in which the module should be displayed on the TermBar.")]
    public int Order { get; set; } = int.MaxValue - 2;

    [Description("Whether the module should expand to take up as much space as possible.")]
    public bool Expand { get; set; } = false;

    [Description("The Catppuccin color to use for the CPU icon.")]
    public ColorEnum AccentColor { get; set; } = ColorEnum.Pink;

    [Description("The text to use as the CPU icon.")]
    public string Icon { get; set; } = "";

    [Description("The numeric format to use for the CPU percentage. These are documented in https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings.")]
    public string Format { get; set; } = "{0:N0}%";

    [Description("Whether to round the CPU percentage before formatting.")]
    public bool Round { get; set; } = true;

    [Description("The CPU usage update interval, in milliseconds.")]
    public uint UpdateInterval { get; set; } = 5000;
  }
}
