﻿using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using System;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Accessibility;

namespace Spakov.TermBar.WindowManagement.Windows
{
    /// <summary>
    /// A TermBar ephemeral window.
    /// </summary>
    /// <remarks>Ephemeral windows are debounced so they cannot "flap" closed
    /// and immediately back open. They are intended for temporary display and
    /// have no window chrome.</remarks>
    internal class EphemeralWindow : Window
    {
        private readonly ILogger? _logger;

        private readonly WINEVENTPROC _winForegroundEventProc;

        private readonly Views.Windows.EphemeralWindow _ephemeralWindow;
        private bool _ignoreFirstForegroundEvent;

        private readonly int _requestedLeft;
        private readonly int _requestedTop;

        /// <inheritdoc cref="Window.Config"/>
        protected new Configuration.Json.TermBar Config { get; init; }

        /// <summary>
        /// The last time an ephemeral window closed.
        /// </summary>
        internal static DateTime LastCloseTime { get; set; }

        /// <summary>
        /// Debounces display of an ephemeral window.
        /// </summary>
        /// <remarks>
        /// Before displaying an ephemeral window, include something like
        /// the following:
        /// <code>if (WindowManagement.EphemeralWindow.Debounce()) return;</code>
        /// </remarks>
        /// <returns>Returns <c>true</c> if the debounce is in effect or
        /// <c>false</c> if the window can be displayed.</returns>
        internal static bool Debounce() => (DateTime.Now - LastCloseTime).Milliseconds < 100;

        private readonly int _margin;
        private readonly int _padding;

        /// <inheritdoc cref="Window.Margin"/>
        protected new int Margin => Config.DpiAware ? ScaleY(_margin) : _margin;

        /// <inheritdoc cref="Window.Padding"/>
        protected new int Padding => Config.DpiAware ? ScaleY(_padding) : _padding;

        /// <summary>
        /// The minimum width of the ephemeral window.
        /// </summary>
        private static int MinimumWidth => 300;

        /// <summary>
        /// The maximum width of the ephemeral window.
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
        /// The minimum height of the ephemeral window.
        /// </summary>
        private static int MinimumHeight => 300;

        /// <summary>
        /// The maximum height of the ephemeral window.
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
        /// Initializes an <see cref="EphemeralWindow"/>.
        /// </summary>
        /// <param name="config"><inheritdoc cref="Window.Window"
        /// path="/param[@name='config']"/></param>
        /// <param name="requestedLeft">The requested left position of the
        /// window.</param>
        /// <param name="requestedTop">The requested top position of the
        /// window.</param>
        /// <param name="content">The content to present in the window.</param>
        internal EphemeralWindow(Configuration.Json.TermBar config, int requestedLeft, int requestedTop, UIElement content) : base(config.Display!, config)
        {
            _logger = LoggerHelper.CreateLogger<EphemeralWindow>();

            Config = base.Config!;

            _requestedLeft = requestedLeft;
            _requestedTop = requestedTop;

            _ephemeralWindow = new(Config, content);
            _winForegroundEventProc = new(WinForegroundEventProc);

            _margin = (int)base.Margin!;
            _padding = (int)base.Padding!;
            _width = Config.DpiAware ? ScaleX(MinimumWidth) : MinimumWidth;
            _height = Config.DpiAware ? ScaleY(MinimumHeight) : MinimumHeight;
        }

        /// <summary>
        /// Displays the ephemeral window.
        /// </summary>
        internal void Display()
        {
            Display(
                _ephemeralWindow,
                WindowLeft(),
                WindowTop(),
                Width,
                Height,
                EphemeralWindow_RequestResize,
                skipActivation: false
            );

            _ignoreFirstForegroundEvent = true;

            _ = PInvoke.SetWinEventHook(
                PInvoke.EVENT_SYSTEM_FOREGROUND,
                PInvoke.EVENT_SYSTEM_FOREGROUND,
                (HMODULE)(nint)0,
                _winForegroundEventProc,
                0,
                0,
                PInvoke.WINEVENT_OUTOFCONTEXT
            );
        }

        /// <summary>
        /// Resizes the ephemeral window based on its contents.
        /// </summary>
        /// <param name="sender"><inheritdoc
        /// cref="System.ComponentModel.PropertyChangedEventHandler"
        /// path="/param[@name='sender']"/></param>
        /// <param name="e"><inheritdoc
        /// cref="System.ComponentModel.PropertyChangedEventHandler"
        /// path="/param[@name='e']"/></param>
        private void EphemeralWindow_RequestResize(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Width = (int)_ephemeralWindow!.DesiredWidth;
            Height = (int)_ephemeralWindow!.DesiredHeight;
        }

        /// <summary>
        /// Determines the left location of the ephemeral window.
        /// </summary>
        /// <returns>The scaled calculated left position of the ephemeral
        /// window.</returns>
        private int WindowLeft()
        {
            return _requestedLeft + Width + Margin > (Config.DpiAware ? ScaleX(display.Width) : display.Width)
                ? (Config.DpiAware ? ScaleX(display.Width) : display.Width) - Width - Margin
                : _requestedLeft;
        }

        /// <summary>
        /// Determines the top location of the ephemeral window.
        /// </summary>
        /// <returns>The scaled calculated top position of the ephemeral
        /// window.</returns>
        /// <exception cref="ArgumentException"></exception>
        private int WindowTop()
        {
            if (Config.Location.Equals(Location.Top))
            {
                return _requestedTop < (Config.DpiAware ? ScaleY(display.Top) : display.Top) + Config.Height + (Margin * 2)
                    ? (Config.DpiAware ? ScaleY(display.Top) : display.Top) + Config.Height + (Margin * 2)
                    : _requestedTop;
            }
            else if (Config.Location.Equals(Location.Bottom))
            {
                return _requestedTop + Margin > (Config.DpiAware ? ScaleY(display.Height) : display.Height) - Config.Height - (Margin * 2)
                    ? (Config.DpiAware ? ScaleY(display.Height) : display.Height) - Config.Height - (Margin * 2)
                    : _requestedTop;
            }

            throw new ArgumentException(App.ResourceLoader.GetString("InvalidLocation"), "Location");
        }

        /// <summary>
        /// Handles event <c>EVENT_SYSTEM_FOREGROUND</c> to close the ephemeral
        /// window on loss of focus.
        /// </summary>
        /// <remarks>
        /// <para>An application-defined callback (or hook) function that the
        /// system calls in response to events generated by an accessible
        /// object. The hook function processes the event notifications as
        /// required. Clients install the hook function and request specific
        /// types of event notifications by calling <see
        /// cref="PInvoke.SetWinEventHook"/>.</para>
        /// <para>The <see cref="WINEVENTPROC"/> type defines a pointer to this
        /// callback function. <c>WinEventProc</c> is a placeholder for the
        /// application-defined function name.</para>
        /// </remarks>
        /// <param name="hWinEventHook">Handle to an event hook function. This
        /// value is returned by <see cref="PInvoke.SetWinEventHook"/> when the
        /// hook function is installed and is specific to each instance of the
        /// hook function.</param>
        /// <param name="event">Specifies the event that occurred. This value
        /// is one of the event constants.</param>
        /// <param name="hWnd">Handle to the window that generates the event,
        /// or <c>null</c> if no window is associated with the event. For
        /// example, the mouse pointer is not associated with a window.</param>
        /// <param name="idObject">Identifies the object associated with the
        /// event. This is one of the object identifiers or a custom object
        /// ID.</param>
        /// <param name="idChild">Identifies whether the event was triggered by
        /// an object or a child element of the object. If this value is
        /// <c>CHILDID_SELF</c>, the event was triggered by the object;
        /// otherwise, this value is the child ID of the element that triggered
        /// the event.</param>
        /// <param name="idEventThread"></param>
        /// <param name="dwmsEventTime">Specifies the time, in milliseconds,
        /// that the event was generated.</param>
        private void WinForegroundEventProc(
            HWINEVENTHOOK hWinEventHook,
            uint @event,
            HWND hWnd,
            int idObject,
            int idChild,
            uint idEventThread,
            uint dwmsEventTime
        )
        {
            _logger?.LogTrace("EVENT_SYSTEM_FOREGROUND for HWND {hWnd}", hWnd);
            _logger?.LogTrace("  TermBar HWND is {Config.HWnd}", Config.HWnd);
            _logger?.LogTrace("  My HWND is {base.hWnd}", base.hWnd);
            _logger?.LogTrace("  XAML Island HWND is {islandHWnd}", islandHWnd);

            if (_ignoreFirstForegroundEvent)
            {
                _logger?.LogTrace("Ephemeral window ignored first EVENT_SYSTEM_FOREGROUND event");
                _ignoreFirstForegroundEvent = false;

                return;
            }

            if (hWnd != base.hWnd)
            {
                PInvoke.UnhookWinEvent(hWinEventHook);
                Close();
                LastCloseTime = DateTime.Now;
            }
        }
    }
}