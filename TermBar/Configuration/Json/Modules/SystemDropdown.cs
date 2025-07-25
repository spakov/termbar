using Spakov.Catppuccin;
using Spakov.TermBar.Configuration.Json.SchemaAttributes;
using System.Collections.Generic;

namespace Spakov.TermBar.Configuration.Json.Modules
{
    [Description("A TermBar system dropdown configuration.")]
    internal class SystemDropdown : IModule
    {
        private const int OrderDefault = int.MinValue;
        private const bool ExpandDefault = false;
        private const ColorEnum AccentColorDefault = ColorEnum.Overlay1;
        private const string AccentColorDefaultAsString = "Overlay1";
        private const string IconDefault = "";

        [Description("The order in which the module should be displayed on the TermBar.")]
        [DefaultIntNumber(OrderDefault)]
        [MinimumInt(int.MinValue)]
        [MaximumInt(int.MaxValue)]
        public int Order { get; set; } = OrderDefault;

        [Description("Whether the module should expand to take up as much space as possible.")]
        [DefaultBoolean(ExpandDefault)]
        public bool Expand { get; set; } = ExpandDefault;

        [Description("The Catppuccin color to use for the dropdown icon color.")]
        [DefaultString(AccentColorDefaultAsString)]
        public ColorEnum AccentColor { get; set; } = AccentColorDefault;

        [Description("The text to use as the dropdown icon.")]
        [DefaultString(IconDefault)]
        public string Icon { get; set; } = IconDefault;

        [Description("System menu features to display in the dropdown.")]
        public List<SystemDropdownFeature> Features { get; set; } =
        [
            SystemDropdownFeature.SystemSettings,
            SystemDropdownFeature.SignOut,
            SystemDropdownFeature.Lock,
            SystemDropdownFeature.ShutDown,
            SystemDropdownFeature.Restart
        ];

        /// <summary>
        /// Available system dropdown features.
        /// </summary>
        internal enum SystemDropdownFeature
        {
            SystemSettings,
            SignOut,
            Lock,
            Sleep,
            ShutDown,
            Restart
        }
    }
}