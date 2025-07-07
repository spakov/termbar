using System.ComponentModel;

namespace TermBar.Configuration.Json {
  [Description("A display. A TermBar window will be shown on each configured display.")]
  internal class Display {
    [Description("The display ID. These can be viewed in the TermBar settings.")]
    public required string Id { get; set; }

    [Description("The TermBar configuration for the display.")]
    public required TermBar TermBar { get; set; }
  }
}
