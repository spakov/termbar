using Spakov.Catppuccin;
using Spakov.TermBar.Configuration.Json.SchemaAttributes;
using System.Collections.Generic;

namespace Spakov.TermBar.Configuration.Json.Modules
{
    [Description("A TermBar launcher configuration.")]
    internal class Launcher : IModule
    {
        private const int OrderDefault = -1;
        private const bool ExpandDefault = false;
        private const ColorEnum AccentColorDefault = ColorEnum.Rosewater;
        private const string AccentColorDefaultAsString = "Rosewater";
        private const string IconDefault = "•";

        [Description("The order in which the module should be displayed on the TermBar.")]
        [DefaultIntNumber(OrderDefault)]
        [MinimumInt(int.MinValue)]
        [MaximumInt(int.MaxValue)]
        public int Order { get; set; } = OrderDefault;

        [Description("Whether the module should expand to take up as much space as possible.")]
        [DefaultBoolean(ExpandDefault)]
        public bool Expand { get; set; } = ExpandDefault;

        [Description("The Catppuccin color to use for launcher icons by default.")]
        [DefaultString(AccentColorDefaultAsString)]
        public ColorEnum AccentColor { get; set; } = AccentColorDefault;

        [Description("The text to use as launcher icons by default.")]
        [DefaultString(IconDefault)]
        public string Icon { get; set; } = IconDefault;

        [Description("Launcher entries.")]
        public List<LauncherEntry> LauncherEntries { get; set; } =
        [
            new()
            {
                Name = "Windows Terminal",
                Command = "wt",
                IconColor = ColorEnum.Overlay0,
                Icon = ""
            },
            new()
            {
                Name = "File Explorer",
                Command = "explorer",
                CommandArguments =
                [
                    "%USERPROFILE%"
                ]
            }
        ];
    }

    [Description("A TermBar launcher entry.")]
    internal class LauncherEntry
    {
        private const bool DisplayNameDefault = false;

        [Description("The name of the launcher entry. This is always displayed as a tooltip and is optionally displayed in the launcher button if DisplayName is true.")]
        public required string Name { get; set; }

        [Description("The command to shell execute.")]
        public required string Command { get; set; }

        [Description("Arguments to the command to shell execute. Set to null for no arguments.")]
        [DefaultNull]
        public string[]? CommandArguments { get; set; } = null;

        [Description("Whether to display Name in the launcher button.")]
        [DefaultBoolean(DisplayNameDefault)]
        public bool DisplayName { get; set; } = DisplayNameDefault;

        [Description("The Catppuccin color to use for the launcher icon. Set to null to use matches in ProcessIconMap and fall back to AccentColor.")]
        [DefaultNull]
        public ColorEnum? IconColor { get; set; } = null;

        [Description("The text to use as the launcher icon. Set to null to use matches in ProcessIconMap and fall back to Icon.")]
        [DefaultNull]
        public string? Icon { get; set; } = null;
    }
}