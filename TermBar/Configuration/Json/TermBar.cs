using Spakov.Catppuccin;
using Spakov.TermBar.Configuration.Json.SchemaAttributes;
using Spakov.TermBar.WindowManagement;
using System.Collections.Generic;
using Windows.Win32.Foundation;

namespace Spakov.TermBar.Configuration.Json
{
    [Description("The TermBar configuration for a display.")]
    internal class TermBar
    {
        private const FlavorEnum FlavorDefault = FlavorEnum.Mocha;
        private const string FlavorDefaultAsString = "Mocha";
        private const ColorEnum BackgroundDefault = ColorEnum.Crust;
        private const string BackgroundDefaultAsString = "Crust";
        private const int CornerRadiusDefault = 12;
        private const string FontFamilyDefault = "0xProto Nerd Font Propo";
        private const double FontSizeDefault = 14.0;
        private const ColorEnum TextColorDefault = ColorEnum.Text;
        private const string TextColorDefaultAsString = "Text";
        private const ColorEnum AccentBackgroundDefault = ColorEnum.Mantle;
        private const string AccentBackgroundDefaultAsString = "Mantle";
        private const ColorEnum AccentColorDefault = ColorEnum.Mauve;
        private const string AccentColorDefaultAsString = "Mauve";
        private const ColorEnum SelectedBackgroundDefault = ColorEnum.Surface0;
        private const string SelectedBackgroundDefaultAsString = "Surface0";
        private const ColorEnum SelectedColorDefault = ColorEnum.Mauve;
        private const string SelectedColorDefaultAsString = "Mauve";
        private const ColorEnum ClickedBackgroundDefault = ColorEnum.Surface1;
        private const string ClickedBackgroundDefaultAsString = "Surface1";
        private const ColorEnum ClickedColorDefault = ColorEnum.Subtext1;
        private const string ClickedColorDefaultAsString = "Subtext1";
        private const Location LocationDefault = Location.Top;
        private const string LocationDefaultAsString = "Top";
        private const bool DpiAwareDefault = true;
        private const int MarginDefault = 16;
        private const int PaddingDefault = 16;
        private const double MinimumWidthPercentageDefault = 0.25;
        private const int HeightDefault = 50;

        /// <summary>
        /// The <see cref="WindowManagement.Display"/> this <see
        /// cref="TermBar"/> is associated with.
        /// </summary>
        /// <remarks>This does not come from JSON—<see cref="ConfigHelper"/>
        /// sets it.</remarks>
        internal WindowManagement.Display? Display { get; set; }

        /// <summary>
        /// The <see cref="HWND"/> with which this <see cref="TermBar"/> is
        /// associated.
        /// </summary>
        /// <remarks>This does not come from JSON—<see
        /// cref="WindowManagement.Windows.TermBarWindow.Display"/> sets
        /// it.</remarks>
        internal HWND? HWnd { get; set; }

        [Description("The Catppuccin flavor to use.")]
        [DefaultString(FlavorDefaultAsString)]
        public FlavorEnum Flavor { get; set; } = FlavorDefault;

        [Description("The Catppuccin color to use for the background.")]
        [DefaultString(BackgroundDefaultAsString)]
        public ColorEnum Background { get; set; } = BackgroundDefault;

        [Description("The corner radius to apply to windows.")]
        [DefaultIntNumber(CornerRadiusDefault)]
        [MinimumInt(0)]
        public int CornerRadius { get; set; } = CornerRadiusDefault;

        [Description("The font family to use.")]
        [DefaultString(FontFamilyDefault)]
        public string FontFamily { get; set; } = FontFamilyDefault;

        [Description("The font size to use.")]
        [MinimumDouble(1.0)]
        [DefaultDoubleNumber(FontSizeDefault)]
        public double FontSize { get; set; } = FontSizeDefault;

        [Description("The Catppuccin color to use for text.")]
        [DefaultString(TextColorDefaultAsString)]
        public ColorEnum TextColor { get; set; } = TextColorDefault;

        [Description("The Catppuccin color to use for the background of accent items. Set to null to use the default system behavior.")]
        [DefaultString(AccentBackgroundDefaultAsString)]
        public ColorEnum? AccentBackground { get; set; } = AccentBackgroundDefault;

        [Description("The Catppuccin color to use as an accent. Set to null to use the default system behavior.")]
        [DefaultString(AccentColorDefaultAsString)]
        public ColorEnum? AccentColor { get; set; } = AccentColorDefault;

        [Description("The Catppuccin color to use for the background of selected items. Set to null to use the default system behavior.")]
        [DefaultString(SelectedBackgroundDefaultAsString)]
        public ColorEnum? SelectedBackground { get; set; } = SelectedBackgroundDefault;

        [Description("The Catppuccin color to use for selected items. Set to null to use the default system behavior.")]
        [DefaultString(SelectedColorDefaultAsString)]
        public ColorEnum? SelectedColor { get; set; } = SelectedColorDefault;

        [Description("The Catppuccin color to use for the background of clicked items. Set to null to use the default system behavior.")]
        [DefaultString(ClickedBackgroundDefaultAsString)]
        public ColorEnum? ClickedBackground { get; set; } = ClickedBackgroundDefault;

        [Description("The Catppuccin color to use for clicked items. Set to null to use the default system behavior.")]
        [DefaultString(ClickedColorDefaultAsString)]
        public ColorEnum? ClickedColor { get; set; } = ClickedColorDefault;

        [Description("The window list configuration.")]
        public WindowList WindowList { get; set; } = new();

        [Description("The window location.")]
        [DefaultString(LocationDefaultAsString)]
        public Location Location { get; set; } = LocationDefault;

        [Description("Whether to consider display DPI.")]
        [DefaultBoolean(DpiAwareDefault)]
        public bool DpiAware { get; set; } = DpiAwareDefault;

        [Description("The margin, in pixels, between the top or bottom screen edge and the TermBar window.")]
        [DefaultIntNumber(MarginDefault)]
        [MinimumInt(0)]
        public int Margin { get; set; } = MarginDefault;

        [Description("The padding, in pixels, between components of the TermBar.")]
        [DefaultIntNumber(PaddingDefault)]
        [MinimumInt(0)]
        public int Padding { get; set; } = PaddingDefault;

        [Description("The minimum width, as a percentage of the total screen width, of the TermBar window. Must be between 0.0 and 1.0.")]
        [DefaultDoubleNumber(MinimumWidthPercentageDefault)]
        [MinimumDouble(0.0)]
        [MaximumDouble(1.0)]
        public double MinimumWidthPercentage { get; set; } = MinimumWidthPercentageDefault;

        [Description("The TermBar window height, in pixels.")]
        [DefaultIntNumber(HeightDefault)]
        [MinimumInt(1)]
        public int Height { get; set; } = HeightDefault;

        [Description("TermBar modules.")]
        public List<IModule>? Modules { get; set; } =
        [
            new Modules.SystemDropdown(),
            new Modules.Launcher(),
            new Modules.Terminal(),
            new Modules.WindowBar(),
            new Modules.WindowDropdown(),
            new Modules.Memory(),
            new Modules.Gpu(),
            new Modules.Cpu(),
            new Modules.Volume(),
            new Modules.Clock()
        ];
    }
}