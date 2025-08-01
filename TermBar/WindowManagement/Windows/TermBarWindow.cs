﻿using Spakov.TermBar.Models;
using System;

namespace Spakov.TermBar.WindowManagement.Windows
{
    /// <summary>
    /// A TermBar window.
    /// </summary>
    /// <remarks>The TermBar window "floats" on the desktop. It is intended for
    /// long-term display and has no window chrome.</remarks>
    internal class TermBarWindow : Window
    {
        private readonly Views.Windows.TermBarWindow _termBarWindow;

        /// <inheritdoc cref="Window.Config"/>
        protected new Configuration.Json.TermBar Config { get; init; }

        private readonly int _margin;
        private readonly int _padding;

        /// <inheritdoc cref="Window.Margin"/>
        protected new int Margin => Config.DpiAware ? ScaleY(_margin) : _margin;

        /// <inheritdoc cref="Window.Padding"/>
        protected new int Padding => Config.DpiAware ? ScaleY(_padding) : _padding;

        /// <summary>
        /// The minimum width of the TermBar window.
        /// </summary>
        private static int MinimumWidth => 128;

        /// <summary>
        /// The maximum width of the TermBar window.
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
                        Height
                    );
                }
            }
        }

        private readonly int _height;

        protected override int Height => Config.DpiAware ? ScaleY(_height) : _height;

        /// <summary>
        /// Initializes a <see cref="TermBarWindow"/>.
        /// </summary>
        /// <param name="config"><inheritdoc cref="Window.Window"
        /// path="/param[@name='config']"/></param>
        /// <param name="content">The content to present in the window.</param>
        internal TermBarWindow(Configuration.Json.TermBar config) : base(config.Display!, config)
        {
            Config = base.Config!;

            _termBarWindow = new(Config);

            _margin = (int)base.Margin!;
            _padding = (int)base.Padding!;
            _width = InitialWindowWidth();
            _height = config.Height;
        }

        /// <summary>
        /// Displays the TermBar window.
        /// </summary>
        internal void Display()
        {
            Display(
                _termBarWindow,
                WindowLeft(),
                WindowTop(),
                Width,
                Height,
                TermBarWindow_RequestResize
            );

            if (Config is not null)
            {
                Config.HWnd = hWnd;
            }
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
        private void TermBarWindow_RequestResize(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!WindowList.IsPopupOpen)
            {
                Width = (int)_termBarWindow!.DesiredWidth;
            }
        }

        /// <summary>
        /// Determines the left location of the TermBar window.
        /// </summary>
        /// <returns>The scaled calculated left position of the TermBar
        /// window.</returns>
        private int WindowLeft()
        {
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
        private int WindowTop()
        {
            if (Config.Location.Equals(Location.Top))
            {
                return (Config.DpiAware ? ScaleY(display.Top) : display.Top)
                    + Margin;
            }
            else if (Config.Location.Equals(Location.Bottom))
            {
                return (Config.DpiAware ? ScaleY(display.Top) : display.Top)
                    + (Config.DpiAware ? ScaleY(display.Height) : display.Height)
                    - Height
                    - Margin;
            }

            throw new ArgumentException(App.ResourceLoader.GetString("InvalidLocation"), "Location");
        }

        /// <summary>
        /// Determines the starting width of the TermBar window.
        /// </summary>
        /// <returns>The scaled calculated width of the TermBar
        /// window.</returns>
        private int InitialWindowWidth()
        {
            int windowWidth = (int)((Config.DpiAware ? ScaleX(display.Width) : display.Width) * Config.MinimumWidthPercentage);

            if (windowWidth < MinimumWidth)
            {
                windowWidth = Config.DpiAware ? ScaleX(MinimumWidth) : MinimumWidth;
            }

            if (windowWidth > (Config.DpiAware ? ScaleX(display.Width) : display.Width))
            {
                windowWidth = Config.DpiAware ? ScaleX(display.Width) : display.Width;
            }

            return windowWidth;
        }
    }
}