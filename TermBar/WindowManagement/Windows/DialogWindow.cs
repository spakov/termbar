using Microsoft.UI.Xaml;
using System;

namespace TermBar.WindowManagement.Windows {
  /// <summary>
  /// A TermBar dialog window.
  /// </summary>
  internal class DialogWindow : Window {
    private readonly Views.Windows.DialogWindow dialogWindow;

    /// <inheritdoc cref="Window.Config"/>
    protected new Configuration.Json.TermBar Config { get; init; }

    private readonly uint _margin;
    private readonly uint _padding;

    /// <inheritdoc cref="Window.Margin"/>
    protected new uint Margin => Config.DpiAware ? ScaleY(_margin) : _margin;

    /// <inheritdoc cref="Window.Padding"/>
    protected new uint Padding => Config.DpiAware ? ScaleY(_padding) : _padding;

    /// <summary>
    /// The minimum width of the dialog window.
    /// </summary>
    private static uint MinimumWidth => 300;

    /// <summary>
    /// The maximum width of the dialog window.
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
            Height,
            skipActivation: false
          );
        }
      }
    }

    /// <summary>
    /// The minimum height of the dialog window.
    /// </summary>
    private static uint MinimumHeight => 150;

    /// <summary>
    /// The maximum height of the dialog window.
    /// </summary>
    private uint MaximumHeight => (Config.DpiAware ? ScaleX(display.Height) : display.Height) - (Padding * 2);

    private uint _height;

    protected override uint Height {
      get => _height;
      set {
        if (_height != value) {
          _height = Math.Min(MaximumHeight, Config.DpiAware ? ScaleX(value) : value);
          _height = Math.Max(MinimumHeight, _height);

          Move(
            WindowLeft(),
            WindowTop(),
            Width,
            _height,
            skipActivation: false
          );
        }
      }
    }

    /// <summary>
    /// Initializes a <see cref="DialogWindow"/>.
    /// </summary>
    /// <param name="config"><inheritdoc cref="Window.Window"
    /// path="/param[@name='config']"/></param>
    /// <param name="content">The content to present in the window.</param>
    internal DialogWindow(Configuration.Json.TermBar config, UIElement content) : base(config.Display!, config) {
      Config = base.Config!;

      dialogWindow = new(Config, content);

      _margin = (uint) base.Margin!;
      _padding = (uint) base.Padding!;
      _width = Config.DpiAware ? ScaleX(MinimumWidth) : MinimumWidth;
      _height = Config.DpiAware ? ScaleY(MinimumHeight) : MinimumHeight;
    }

    /// <summary>
    /// Displays the dialog window.
    /// </summary>
    internal void Display() {
      Display(
        dialogWindow,
        WindowLeft(),
        WindowTop(),
        Width,
        Height,
        TermBarWindow_RequestResize,
        skipActivation: false
      );
    }

    /// <summary>
    /// Called when the dialog window requests a resize.
    /// </summary>
    /// <param name="sender"><inheritdoc
    /// cref="System.ComponentModel.PropertyChangedEventHandler"
    /// path="/param[@name='sender']"/></param>
    /// <param name="e"><inheritdoc
    /// cref="System.ComponentModel.PropertyChangedEventHandler"
    /// path="/param[@name='e']"/></param>
    private void TermBarWindow_RequestResize(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {
      Width = (uint) dialogWindow!.DesiredWidth;
      Height = (uint) dialogWindow!.DesiredHeight;
    }

    /// <summary>
    /// Determines the left location of the dialog window.
    /// </summary>
    /// <returns>The scaled calculated left position of the dialog
    /// window.</returns>
    private uint WindowLeft() {
      return (Config.DpiAware ? ScaleX(display.Left) : display.Left)
        + ((Config.DpiAware ? ScaleX(display.Width) : display.Width) / 2)
        - (Width / 2);
    }

    /// <summary>
    /// Determines the top location of the dialog window.
    /// </summary>
    /// <returns>The scaled calculated top position of the dialog
    /// window.</returns>
    private uint WindowTop() {
      return (Config.DpiAware ? ScaleY(display.Top) : display.Top)
        + ((Config.DpiAware ? ScaleY(display.Height) : display.Height) / 2)
        - (Height / 2);
    }
  }
}