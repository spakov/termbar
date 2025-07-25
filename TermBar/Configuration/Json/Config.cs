using Spakov.TermBar.Configuration.Json.SchemaAttributes;
using System.Collections.Generic;

namespace Spakov.TermBar.Configuration.Json
{
    [Description("The TermBar configuration.")]
    internal class Config
    {
        private const string StartDirectoryDefault = "%USERPROFILE%";

        [Description("A list of displays.")]
        public required List<Display> Displays { get; set; }

        [Description("The directory to change to when starting. Set to null to keep the directory inherited from the parent process.")]
        [DefaultString(StartDirectoryDefault)]
        public string? StartDirectory { get; set; } = StartDirectoryDefault;

        [Description("Whether TermBar should start up at boot time.")]
        [DefaultBoolean(false)]
        public bool? StartupAtBoot { get; set; } = false;
    }
}