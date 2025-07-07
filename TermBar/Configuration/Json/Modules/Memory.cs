using System.ComponentModel;
using TermBar.Catppuccin;

namespace TermBar.Configuration.Json.Modules {
  [Description("A TermBar memory monitor configuration.")]
  internal class Memory : IModule {
    [Description("The order in which the module should be displayed on the TermBar.")]
    public int Order { get; set; } = int.MaxValue - 3;

    [Description("Whether the module should expand to take up as much space as possible.")]
    public bool Expand { get; set; } = false;

    [Description("The Catppuccin color to use for the memory icon.")]
    public ColorEnum AccentColor { get; set; } = ColorEnum.Teal;

    [Description("The text to use as the memory icon.")]
    public string Icon { get; set; } = "";

    [Description("The numeric format to use for the memory percentage. These are documented in https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings.")]
    public string Format { get; set; } = "{0:N0}%";

    [Description("Whether to round the memory percentage before formatting.")]
    public bool Round { get; set; } = true;

    [Description("The memory usage update interval, in milliseconds.")]
    public uint UpdateInterval { get; set; } = 5000;
  }
}
