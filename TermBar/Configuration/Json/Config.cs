using System.Collections.Generic;
using TermBar.Configuration.Json.SchemaAttributes;

namespace TermBar.Configuration.Json {
  [Description("The TermBar configuration.")]
  internal class Config {
    private const string startDirectoryDefault = "%USERPROFILE%";

    [Description("A list of displays.")]
    public required List<Display> Displays { get; set; }

    [Description("The directory to change to when starting. Set to null to keep the directory inherited from the parent process.")]
    [DefaultString(startDirectoryDefault)]
    public string? StartDirectory { get; set; } = startDirectoryDefault;
  }
}
