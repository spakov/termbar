using Spakov.Catppuccin;
using Spakov.TermBar.Configuration.Json.SchemaAttributes;
using Spakov.TermBar.WindowManagement;
using System.Collections.Generic;
using Windows.Win32.Foundation;

namespace Spakov.TermBar.Configuration.Json {
  [Description("The TermBar configuration for a display.")]
  internal class TermBar {
    private const FlavorEnum flavorDefault = FlavorEnum.Mocha;
    private const string flavorDefaultAsString = "Mocha";
    private const ColorEnum backgroundDefault = ColorEnum.Crust;
    private const string backgroundDefaultAsString = "Crust";
    private const int cornerRadiusDefault = 12;
    private const string fontFamilyDefault = "0xProto Nerd Font Propo";
    private const double fontSizeDefault = 14.0;
    private const ColorEnum textColorDefault = ColorEnum.Text;
    private const string textColorDefaultAsString = "Text";
    private const ColorEnum accentBackgroundDefault = ColorEnum.Mantle;
    private const string accentBackgroundDefaultAsString = "Mantle";
    private const ColorEnum accentColorDefault = ColorEnum.Mauve;
    private const string accentColorDefaultAsString = "Mauve";
    private const ColorEnum selectedBackgroundDefault = ColorEnum.Surface0;
    private const string selectedBackgroundDefaultAsString = "Surface0";
    private const ColorEnum selectedColorDefault = ColorEnum.Mauve;
    private const string selectedColorDefaultAsString = "Mauve";
    private const ColorEnum clickedBackgroundDefault = ColorEnum.Surface1;
    private const string clickedBackgroundDefaultAsString = "Surface1";
    private const ColorEnum clickedColorDefault = ColorEnum.Subtext1;
    private const string clickedColorDefaultAsString = "Subtext1";
    private const Location locationDefault = Location.Top;
    private const string locationDefaultAsString = "Top";
    private const bool dpiAwareDefault = true;
    private const int marginDefault = 16;
    private const int paddingDefault = 16;
    private const double minimumWidthPercentageDefault = 0.25;
    private const int heightDefault = 50;

    /// <summary>
    /// The <see cref="WindowManagement.Display"/> this <see cref="TermBar"/>
    /// is associated with.
    /// </summary>
    /// <remarks>This does not come from JSON—<see cref="ConfigHelper"/> sets
    /// it.</remarks>
    internal WindowManagement.Display? Display { get; set; }

    /// <summary>
    /// The <see cref="HWND"/> this <see cref="TermBar"/> is associated with.
    /// </summary>
    /// <remarks>This does not come from JSON—<see
    /// cref="TermBar.WindowManagement.Windows.TermBarWindow.Display"/> sets
    /// it.</remarks>
    internal HWND? HWnd { get; set; }

    [Description("The Catppuccin flavor to use.")]
    [DefaultString(flavorDefaultAsString)]
    public FlavorEnum Flavor { get; set; } = flavorDefault;

    [Description("The Catppuccin color to use for the background.")]
    [DefaultString(backgroundDefaultAsString)]
    public ColorEnum Background { get; set; } = backgroundDefault;

    [Description("The corner radius to apply to windows.")]
    [DefaultIntNumber(cornerRadiusDefault)]
    [MinimumInt(0)]
    public int CornerRadius { get; set; } = cornerRadiusDefault;

    [Description("The font family to use.")]
    [DefaultString(fontFamilyDefault)]
    public string FontFamily { get; set; } = fontFamilyDefault;

    [Description("The font size to use.")]
    [MinimumDouble(1.0)]
    [DefaultDoubleNumber(fontSizeDefault)]
    public double FontSize { get; set; } = fontSizeDefault;

    [Description("The Catppuccin color to use for text.")]
    [DefaultString(textColorDefaultAsString)]
    public ColorEnum TextColor { get; set; } = textColorDefault;

    [Description("The Catppuccin color to use for the background of accent items. Set to null to use the default system behavior.")]
    [DefaultString(accentBackgroundDefaultAsString)]
    public ColorEnum? AccentBackground { get; set; } = accentBackgroundDefault;

    [Description("The Catppuccin color to use as an accent. Set to null to use the default system behavior.")]
    [DefaultString(accentColorDefaultAsString)]
    public ColorEnum? AccentColor { get; set; } = accentColorDefault;

    [Description("The Catppuccin color to use for the background of selected items. Set to null to use the default system behavior.")]
    [DefaultString(selectedBackgroundDefaultAsString)]
    public ColorEnum? SelectedBackground { get; set; } = selectedBackgroundDefault;

    [Description("The Catppuccin color to use for selected items. Set to null to use the default system behavior.")]
    [DefaultString(selectedColorDefaultAsString)]
    public ColorEnum? SelectedColor { get; set; } = selectedColorDefault;

    [Description("The Catppuccin color to use for the background of clicked items. Set to null to use the default system behavior.")]
    [DefaultString(clickedBackgroundDefaultAsString)]
    public ColorEnum? ClickedBackground { get; set; } = clickedBackgroundDefault;

    [Description("The Catppuccin color to use for clicked items. Set to null to use the default system behavior.")]
    [DefaultString(clickedColorDefaultAsString)]
    public ColorEnum? ClickedColor { get; set; } = clickedColorDefault;

    [Description("The window list configuration.")]
    public WindowList WindowList { get; set; } = new();

    [Description("The window location.")]
    [DefaultString(locationDefaultAsString)]
    public Location Location { get; set; } = locationDefault;

    [Description("Whether to consider display DPI.")]
    [DefaultBoolean(dpiAwareDefault)]
    public bool DpiAware { get; set; } = dpiAwareDefault;

    [Description("The margin, in pixels, between the top or bottom screen edge and the TermBar window.")]
    [DefaultIntNumber(marginDefault)]
    [MinimumInt(0)]
    public int Margin { get; set; } = marginDefault;

    [Description("The padding, in pixels, between components of the TermBar.")]
    [DefaultIntNumber(paddingDefault)]
    [MinimumInt(0)]
    public int Padding { get; set; } = paddingDefault;

    [Description("The minimum width, as a percentage of the total screen width, of the TermBar window. Must be between 0.0 and 1.0.")]
    [DefaultDoubleNumber(minimumWidthPercentageDefault)]
    [MinimumDouble(0.0)]
    [MaximumDouble(1.0)]
    public double MinimumWidthPercentage { get; set; } = minimumWidthPercentageDefault;

    [Description("The TermBar window height, in pixels.")]
    [DefaultIntNumber(heightDefault)]
    [MinimumInt(1)]
    public int Height { get; set; } = heightDefault;

    [Description("TermBar modules.")]
    public List<IModule>? Modules { get; set; } = [
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