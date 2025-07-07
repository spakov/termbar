using System.Collections.Generic;
using System.ComponentModel;
using TermBar.Catppuccin;
using Windows.Win32.Foundation;

namespace TermBar.Configuration.Json {
  [Description("The TermBar configuration for a display.")]
  internal class TermBar {
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
    public FlavorEnum Flavor { get; set; } = FlavorEnum.Mocha;

    [Description("The Catppuccin color to use for the background.")]
    public ColorEnum Background { get; set; } = ColorEnum.Crust;

    [Description("The corner radius to apply to windows.")]
    public uint CornerRadius { get; set; } = 12;

    [Description("The font family to use.")]
    public string FontFamily { get; set; } = "0xProto Nerd Font Propo";

    [Description("The font size to use.")]
    public double FontSize { get; set; } = 14.0;

    [Description("The Catppuccin color to use for text.")]
    public ColorEnum TextColor { get; set; } = ColorEnum.Text;

    [Description("The Catppuccin color to use for the background of accent items. Set to null to use the default system behavior.")]
    public ColorEnum? AccentBackground { get; set; } = ColorEnum.Mantle;

    [Description("The Catppuccin color to use as an accent. Set to null to use the default system behavior.")]
    public ColorEnum? AccentColor { get; set; } = ColorEnum.Mauve;

    [Description("The Catppuccin color to use for the background of selected items. Set to null to use the default system behavior.")]
    public ColorEnum? SelectedBackground { get; set; } = ColorEnum.Surface0;

    [Description("The Catppuccin color to use for selected items. Set to null to use the default system behavior.")]
    public ColorEnum? SelectedColor { get; set; } = ColorEnum.Mauve;

    [Description("The Catppuccin color to use for the background of clicked items. Set to null to use the default system behavior.")]
    public ColorEnum? ClickedBackground { get; set; } = ColorEnum.Surface1;

    [Description("The Catppuccin color to use for clicked items. Set to null to use the default system behavior.")]
    public ColorEnum? ClickedColor { get; set; } = ColorEnum.Subtext1;

    [Description("The window list configuration.")]
    public WindowList WindowList { get; set; } = new();

    [Description("The window location.")]
    public WindowManagement.Location Location { get; set; } = WindowManagement.Location.Top;

    [Description("Whether to consider display DPI.")]
    public bool DpiAware { get; set; } = true;

    [Description("The margin, in pixels, between the top or bottom screen edge and the TermBar window.")]
    public uint Margin { get; set; } = 16;

    [Description("The padding, in pixels, between components of the TermBar.")]
    public uint Padding { get; set; } = 16;

    [Description("The minimum width, as a percentage of the total screen width, of the TermBar window. Must be between 0.0 and 1.0.")]
    public double MinimumWidthPercentage { get; set; } = 0.25;

    [Description("The TermBar window height, in pixels.")]
    public uint Height { get; set; } = 50;

    [Description("TermBar modules.")]
    public List<IModule>? Modules { get; set; } = [
      new Modules.Launcher(),
      new Modules.Terminal(),
      new Modules.WindowBar(),
      new Modules.WindowDropdown(),
      new Modules.Memory(),
      new Modules.Cpu(),
      new Modules.Volume(),
      new Modules.Clock()
    ];
  }
}
