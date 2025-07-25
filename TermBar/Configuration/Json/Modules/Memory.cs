using Spakov.Catppuccin;
using Spakov.TermBar.Configuration.Json.SchemaAttributes;

namespace Spakov.TermBar.Configuration.Json.Modules
{
    [Description("A TermBar memory monitor configuration.")]
    internal class Memory : IModule
    {
        private const int OrderDefault = int.MaxValue - 4;
        private const bool ExpandDefault = false;
        private const ColorEnum AccentColorDefault = ColorEnum.Maroon;
        private const string AccentColorDefaultAsString = "Maroon";
        private const string IconDefault = "";
        private const string FormatDefault = "{0:N0}%";
        private const bool RoundDefault = true;
        private const int UpdateIntervalDefault = 5000;

        [Description("The order in which the module should be displayed on the TermBar.")]
        [DefaultIntNumber(OrderDefault)]
        [MinimumInt(int.MinValue)]
        [MaximumInt(int.MaxValue)]
        public int Order { get; set; } = OrderDefault;

        [Description("Whether the module should expand to take up as much space as possible.")]
        [DefaultBoolean(ExpandDefault)]
        public bool Expand { get; set; } = ExpandDefault;

        [Description("The Catppuccin color to use for the memory icon.")]
        [DefaultString(AccentColorDefaultAsString)]
        public ColorEnum AccentColor { get; set; } = AccentColorDefault;

        [Description("The text to use as the memory icon.")]
        [DefaultString(IconDefault)]
        public string Icon { get; set; } = IconDefault;

        [Description("The numeric format to use for the memory percentage. These are documented in https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings.")]
        [DefaultString(FormatDefault)]
        public string Format { get; set; } = FormatDefault;

        [Description("Whether to round the memory percentage before formatting.")]
        [DefaultBoolean(RoundDefault)]
        public bool Round { get; set; } = RoundDefault;

        [Description("The memory usage update interval, in milliseconds.")]
        [DefaultIntNumber(UpdateIntervalDefault)]
        [MinimumInt(1)]
        public int UpdateInterval { get; set; } = UpdateIntervalDefault;
    }
}