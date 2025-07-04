using System;

namespace TermBar.WindowManagement.Windows {
  /// <summary>
  /// A TermBar window.
  /// </summary>
  internal class TermBarWindow : Window {
    private readonly Views.Windows.TermBarWindow termBarWindow;

    /// <inheritdoc cref="Window.Config"/>
    protected new Configuration.Json.TermBar Config { get; init; }

    private readonly uint _margin;
    private readonly uint _padding;

    /// <inheritdoc cref="Window.Margin"/>
    protected new uint Margin => Config.DpiAware ? ScaleY(_margin) : _margin;

    /// <inheritdoc cref="Window.Padding"/>
    protected new uint Padding => Config.DpiAware ? ScaleY(_padding) : _padding;

    /// <summary>
    /// The minimum width of the TermBar window.
    /// </summary>
    private static uint MinimumWidth => 128;

    /// <summary>
    /// The maximum width of the TermBar window.
    /// </summary>
    private uint MaximumWidth => (Config.DpiAware ? ScaleX(display.Width) : display.Width) - (Padding * 2);

    private uint _width;

    protected override uint Width {
      get => _width;
      set {
        if (_width != value) {
          _width = Math.Min(MaximumWidth, Config.DpiAware ? ScaleX(value) : value);
          _width = Math.Max(MinimumWidth, _width);

          Move(
            WindowLeft(),
            WindowTop(),
            _width,
            Height
          );
        }
      }
    }

    private readonly uint _height;

    protected override uint Height => Config.DpiAware ? ScaleY(_height) : _height;

    /// <summary>
    /// Initializes a <see cref="TermBarWindow"/>.
    /// </summary>
    /// <param name="config"><inheritdoc cref="Window.Window"
    /// path="/param[@name='config']"/></param>
    /// <param name="content">The content to present in the window.</param>
    internal TermBarWindow(Configuration.Json.TermBar config) : base(config.Display!, config) {
      Config = base.Config!;

      termBarWindow = new(Config);

      _margin = (uint) base.Margin!;
      _padding = (uint) base.Padding!;
      _width = InitialWindowWidth();
      _height = config.Height;
    }

    /// <summary>
    /// Displays the TermBar window.
    /// </summary>
    internal void Display() {
      Display(
        termBarWindow,
        WindowLeft(),
        WindowTop(),
        Width,
        Height,
        TermBarWindow_RequestResize
      );

      if (Config is not null) Config.HWnd = hWnd;
    }

    /// <summary>
    /// Called when the TermBar window requests a resize.
    /// </summary>
    /// <param name="sender"><inheritdoc
    /// cref="System.ComponentModel.PropertyChangedEventHandler"
    /// path="/param[@name='sender']"/></param>
    /// <param name="e"><inheritdoc
    /// cref="System.ComponentModel.PropertyChangedEventHandler"
    /// path="/param[@name='e']"/></param>
    private void TermBarWindow_RequestResize(object? sender, System.ComponentModel.PropertyChangedEventArgs e) => Width = (uint) termBarWindow!.DesiredWidth;

    /// <summary>
    /// Determines the left location of the TermBar window.
    /// </summary>
    /// <returns>The scaled calculated left position of the TermBar
    /// window.</returns>
    private uint WindowLeft() {
      return (Config.DpiAware ? ScaleX(display.Left) : display.Left)
        + ((Config.DpiAware ? ScaleX(display.Width) : display.Width) / 2)
        - (Width / 2);
    }

    /// <summary>
    /// Determines the top location of the TermBar window.
    /// </summary>
    /// <returns>The scaled calculated top position of the TermBar
    /// window.</returns>
    /// <exception cref="ArgumentException"></exception>
    private uint WindowTop() {
      if (Config.Location.Equals(Location.Top)) {
        return (Config.DpiAware ? ScaleY(display.Top) : display.Top)
          + Margin;
      } else if (Config.Location.Equals(Location.Bottom)) {
        return (Config.DpiAware ? ScaleY(display.Top) : display.Top)
          + (Config.DpiAware ? ScaleY(display.Height) : display.Height)
          - Height
          - Margin;
      }

      throw new ArgumentException("Invalid Location", "Location");
    }

    /// <summary>
    /// Determines the starting width of the TermBar window.
    /// </summary>
    /// <returns>The scaled calculated width of the TermBar window.</returns>
    private uint InitialWindowWidth() {
      uint windowWidth = (uint) ((Config.DpiAware ? ScaleX(display.Width) : display.Width) * Config.MinimumWidthPercentage);

      if (windowWidth < MinimumWidth) windowWidth = Config.DpiAware ? ScaleX(MinimumWidth) : MinimumWidth;
      if (windowWidth > (Config.DpiAware ? ScaleX(display.Width) : display.Width)) windowWidth = Config.DpiAware ? ScaleX(display.Width) : display.Width;

      return windowWidth;
    }
  }
}
