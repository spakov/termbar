using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Spakov.Catppuccin;
using Spakov.TermBar.Themes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace Spakov.TermBar.Views.Windows
{
    /// <summary>
    /// A TermBar "window".
    /// </summary>
    /// <remarks>"Windows" are actually <see cref="UserControl"/>s hosted in
    /// native-managed XAML islands.</remarks>
    internal abstract partial class Window : UserControl, INotifyPropertyChanged
    {
        private readonly ILogger? _logger;

        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly DispatcherTimer _layoutTimer;
        private readonly UIElement? _child;

        private readonly Configuration.Json.TermBar? _config;

        private double _desiredWidth;
        private double _desiredHeight;

        /// <summary>
        /// The window's child element.
        /// </summary>
        internal UIElement Child
        {
            get => ((Border)Content).Child;
            set => ((Border)Content).Child = value;
        }

        /// <summary>
        /// The desired width of the window.
        /// </summary>
        internal double DesiredWidth
        {
            get => _desiredWidth;
            set
            {
                if (Math.Abs(value - _desiredWidth) > 1)
                {
                    _desiredWidth = value;
                    _logger?.LogTrace("Desired width: {desiredWidth}", _desiredWidth);

                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The desired height of the window.
        /// </summary>
        internal double DesiredHeight
        {
            get => _desiredHeight;
            set
            {
                if (Math.Abs(value - _desiredHeight) > 1)
                {
                    _desiredHeight = value;
                    _logger?.LogTrace("Desired height: {desiredHeight}", _desiredHeight);

                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Initializes a <see cref="Window"/>.
        /// </summary>
        /// <remarks>Be sure to invoke <see cref="ApplyComputedStyles"/> to set
        /// styles <em>after</em> setting <see
        /// cref="UserControl.Content"/>.</remarks>
        /// <param name="config">A <see
        /// cref="Configuration.Json.TermBar"/>.</param>
        /// <param name="child">A child element to present in the
        /// window.</param>
        protected Window(Configuration.Json.TermBar? config, UIElement? child = null)
        {
            _logger = LoggerHelper.CreateLogger<Window>();

            _config = config;
            _child = child;

            // Our Win32 window's cursor, at this point, is (probably)
            // IDC_WAIT. It "bleeds through" when we do things like display
            // context menus. Explanation from Raymond Chen:
            // https://devblogs.microsoft.com/oldnewthing/20250424-00/?p=111114
            PInvoke.SetCursor(PInvoke.LoadCursor((HMODULE)(nint)0, PInvoke.IDC_ARROW));

            // Not super happy with this, but haven't found a reliable
            // event-driven way to achieve this yet that doesn't devolve into
            // an endless recursive loop
            _layoutTimer = new()
            {
                Interval = TimeSpan.FromMilliseconds(10)
            };

            _layoutTimer.Tick += LayoutTimer_Tick;
            _layoutTimer.Start();

            if (config is not null)
            {
                ThemeHelper.BuildTheme(Resources, config);
            }

            Content = new Border();
        }

        /// <summary>
        /// Applies computed styles to the window.
        /// </summary>
        protected void ApplyComputedStyles()
        {
            if (_config is not null)
            {
                ((Border)Content).Background = Palette.Instance[_config.Flavor].Colors[_config.Background].SolidColorBrush;
                ((Border)Content).CornerRadius = new(_config.CornerRadius);
            }

            if (_child is not null)
            {
                ((Border)Content).Child = _child;
            }
        }

        /// <summary>
        /// Calculates the desired size of the TermBar window.
        /// </summary>
        /// <param name="sender"><inheritdoc cref="EventHandler"
        /// path="/param[@name='sender']"/></param>
        /// <param name="e"><inheritdoc cref="EventHandler"
        /// path="/param[@name='e']"/></param>
        private void LayoutTimer_Tick(object? sender, object e)
        {
            Measure(new Size(float.PositiveInfinity, float.PositiveInfinity));
            DesiredWidth = DesiredSize.Width;
            DesiredHeight = DesiredSize.Height;
        }

        private void OnPropertyChanged([CallerMemberName] string? callerMemberName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerMemberName));
    }
}