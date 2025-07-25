using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using Windows.Win32;

namespace Spakov.TermBar.WindowManagement.Windows
{
    /// <summary>
    /// A TermBar dialog window.
    /// </summary>
    /// <remarks>Dialog windows are styled as topmost and are draggable. They
    /// have no window chrome.</remarks>
    internal class DialogWindow : Window
    {
        private readonly Views.Windows.DialogWindow _dialogWindow;

        /// <inheritdoc cref="Window.Config"/>
        protected new Configuration.Json.TermBar Config { get; init; }

        private readonly int _margin;
        private readonly int _padding;

        private bool _dragging;
        private Rectangle _draggingWindowRectangle;
        private Point _draggingCursorPosition;

        /// <inheritdoc cref="Window.Margin"/>
        protected new int Margin => Config.DpiAware ? ScaleY(_margin) : _margin;

        /// <inheritdoc cref="Window.Padding"/>
        protected new int Padding => Config.DpiAware ? ScaleY(_padding) : _padding;

        /// <summary>
        /// The minimum width of the dialog window.
        /// </summary>
        private static int MinimumWidth => 300;

        /// <summary>
        /// The maximum width of the dialog window.
        /// </summary>
        private int MaximumWidth => (Config.DpiAware ? ScaleX(display.Width) : display.Width) - (Padding * 2);

        private int _width;

        protected override int Width
        {
            get => _width;

            set
            {
                if (_width != value)
                {
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
        private static int MinimumHeight => 150;

        /// <summary>
        /// The maximum height of the dialog window.
        /// </summary>
        private int MaximumHeight => (Config.DpiAware ? ScaleX(display.Height) : display.Height) - (Padding * 2);

        private int _height;

        protected override int Height
        {
            get => _height;

            set
            {
                if (_height != value)
                {
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
        internal DialogWindow(Configuration.Json.TermBar config, UIElement content) : base(config.Display!, config)
        {
            Config = base.Config!;

            _dialogWindow = new(Config, content);

            _margin = (int)base.Margin!;
            _padding = (int)base.Padding!;
            _width = Config.DpiAware ? ScaleX(MinimumWidth) : MinimumWidth;
            _height = Config.DpiAware ? ScaleY(MinimumHeight) : MinimumHeight;

            content.PointerPressed += Content_PointerPressed;
            content.PointerMoved += Content_PointerMoved;
            content.PointerReleased += Content_PointerReleased;
        }

        /// <summary>
        /// Displays the dialog window.
        /// </summary>
        internal void Display()
        {
            Display(
                _dialogWindow,
                WindowLeft(),
                WindowTop(),
                Width,
                Height,
                DialogWindow_RequestResize,
                isDialog: true,
                skipActivation: false
            );
        }

        /// <summary>
        /// Resizes the dialog window based on its contents.
        /// </summary>
        /// <param name="sender"><inheritdoc
        /// cref="PropertyChangedEventHandler"
        /// path="/param[@name='sender']"/></param>
        /// <param name="e"><inheritdoc
        /// cref="PropertyChangedEventHandler"
        /// path="/param[@name='e']"/></param>
        private void DialogWindow_RequestResize(object? sender, PropertyChangedEventArgs e)
        {
            Width = (int)_dialogWindow!.DesiredWidth;
            Height = (int)_dialogWindow!.DesiredHeight;
        }

        /// <summary>
        /// Determines the left location of the dialog window.
        /// </summary>
        /// <returns>The scaled calculated left position of the dialog
        /// window.</returns>
        private int WindowLeft()
        {
            return (Config.DpiAware ? ScaleX(display.Left) : display.Left)
                + ((Config.DpiAware ? ScaleX(display.Width) : display.Width) / 2)
                - (Width / 2);
        }

        /// <summary>
        /// Determines the top location of the dialog window.
        /// </summary>
        /// <returns>The scaled calculated top position of the dialog
        /// window.</returns>
        private int WindowTop()
        {
            return (Config.DpiAware ? ScaleY(display.Top) : display.Top)
                + ((Config.DpiAware ? ScaleY(display.Height) : display.Height) / 2)
                - (Height / 2);
        }

        /// <summary>
        /// Starts a dragging operation.
        /// </summary>
        /// <param name="sender"><inheritdoc cref="RoutedEventHandler"
        /// path="/param[@name='sender']"/></param>
        /// <param name="e"><inheritdoc cref="RoutedEventHandler"
        /// path="/param[@name='e']"/></param>
        /// <exception cref="Win32Exception"></exception>
        private void Content_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.GetCurrentPoint((UIElement)sender).Properties.IsLeftButtonPressed)
            {
                ((UIElement)sender).CapturePointer(e.Pointer);

                Rectangle windowPosition = GetPosition();

                if (!PInvoke.GetCursorPos(out Point cursorPosition))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                _draggingWindowRectangle = windowPosition;
                _draggingCursorPosition = cursorPosition;
                _dragging = true;
            }
        }

        /// <summary>
        /// Continues a dragging operation.
        /// </summary>
        /// <param name="sender"><inheritdoc cref="RoutedEventHandler"
        /// path="/param[@name='sender']"/></param>
        /// <param name="e"><inheritdoc cref="RoutedEventHandler"
        /// path="/param[@name='e']"/></param>
        private void Content_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (_dragging)
            {
                if (!PInvoke.GetCursorPos(out Point cursorPosition))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                Move(
                    _draggingWindowRectangle.X + (cursorPosition.X - _draggingCursorPosition.X),
                    _draggingWindowRectangle.Y + (cursorPosition.Y - _draggingCursorPosition.Y),
                    _draggingWindowRectangle.Width,
                    _draggingWindowRectangle.Height,
                    skipActivation: true
                );
            }
        }

        /// <summary>
        /// Stops a dragging operation.
        /// </summary>
        /// <param name="sender"><inheritdoc cref="RoutedEventHandler"
        /// path="/param[@name='sender']"/></param>
        /// <param name="e"><inheritdoc cref="RoutedEventHandler"
        /// path="/param[@name='e']"/></param>
        private void Content_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            ((UIElement)sender).ReleasePointerCaptures();
            _dragging = false;
        }
    }
}