using System.Collections.Generic;
using TermBar.Catppuccin;
using Windows.Win32.Foundation;

namespace TermBar.Configuration.Json {
  /// <summary>
  /// A TermBar configuration for a display.
  /// </summary>
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

    /// <summary>
    /// The Catppuccin flavor to use.
    /// </summary>
    public FlavorEnum Flavor { get; set; } = FlavorEnum.Mocha;

    /// <summary>
    /// The Catppuccin color to use as the background.
    /// </summary>
    public ColorEnum Background { get; set; } = ColorEnum.Crust;

    /// <summary>
    /// The corner radius to apply to the window.
    /// </summary>
    public uint CornerRadius { get; set; } = 12;

    /// <summary>
    /// The font family to use.
    /// </summary>
    public string FontFamily { get; set; } = "0xProto Nerd Font Propo";

    /// <summary>
    /// The font size to use.
    /// </summary>
    public double FontSize { get; set; } = 16.0;

    /// <summary>
    /// The Catppuccin color to use for text.
    /// </summary>
    public ColorEnum TextColor { get; set; } = ColorEnum.Text;

    /// <summary>
    /// The Catppuccin color to use as the background of accent items.
    /// </summary>
    /// <remarks>Set to <c>null</c> to use the default system
    /// behavior.</remarks>
    public ColorEnum? AccentBackground { get; set; } = ColorEnum.Mantle;

    /// <summary>
    /// The Catppuccin color to use as an accent color.
    /// </summary>
    /// <remarks>Set to <c>null</c> to use the default system
    /// behavior.</remarks>
    public ColorEnum? AccentColor { get; set; } = ColorEnum.Mauve;

    /// <summary>
    /// The Catppuccin color to use as the background of selected items.
    /// </summary>
    /// <remarks>Set to <c>null</c> to use the default system
    /// behavior.</remarks>
    public ColorEnum? SelectedBackground { get; set; } = ColorEnum.Surface0;

    /// <summary>
    /// The Catppuccin color to use for selected items.
    /// </summary>
    /// <remarks>Set to <c>null</c> to use the default system
    /// behavior.</remarks>
    public ColorEnum? SelectedColor { get; set; } = ColorEnum.Mauve;

    /// <summary>
    /// The Catppuccin color to use as the background of clicked items.
    /// </summary>
    /// <remarks>Set to <c>null</c> to use the default system
    /// behavior.</remarks>
    public ColorEnum? ClickedBackground { get; set; } = ColorEnum.Surface1;

    /// <summary>
    /// The Catppuccin color to use for clicked items.
    /// </summary>
    /// <remarks>Set to <c>null</c> to use the default system
    /// behavior.</remarks>
    public ColorEnum? ClickedColor { get; set; } = ColorEnum.Subtext1;

    /// <summary>
    /// The configuration for window list modules.
    /// </summary>
    public WindowList WindowList { get; set; } = new();

    /// <inheritdoc cref="WindowManagement.Location"/>
    public WindowManagement.Location Location { get; set; } = WindowManagement.Location.Top;

    /// <summary>
    /// Whether to consider display DPI.
    /// </summary>
    public bool DpiAware { get; set; } = true;

    /// <summary>
    /// The margin, in pixels, between the top or bottom screen edge and the
    /// TermBar window.
    /// </summary>
    public uint Margin { get; set; } = 16;

    /// <summary>
    /// The padding, in pixels, between components of the TermBar.
    /// </summary>
    public uint Padding { get; set; } = 16;

    /// <summary>
    /// The minimum width, as a percentage of the total screen width, of the
    /// TermBar window.
    /// </summary>
    /// <remarks>Must be between 0 and 1.</remarks>
    public double MinimumWidthPercentage { get; set; } = 0.25;

    /// <summary>
    /// The window height, in pixels.
    /// </summary>
    public uint Height { get; set; } = 48;

    /// <summary>
    /// The array of modules.
    /// </summary>
    public List<IModule>? Modules { get; set; } = [
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
