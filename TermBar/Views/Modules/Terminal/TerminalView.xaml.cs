using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Spakov.Catppuccin;
using Spakov.TermBar.Styles;
using Spakov.TermBar.ViewModels.Modules.Terminal;
using Spakov.Terminal;
using System;

namespace Spakov.TermBar.Views.Modules.Terminal
{
    /// <summary>
    /// The TermBar terminal view.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1812:Class is apparently never instantiated", Justification = "Instantiated with Activator.CreateInstance()")]
    internal sealed partial class TerminalView : ModuleView
    {
        private readonly Configuration.Json.TermBar _config;
        private readonly Configuration.Json.Modules.Terminal _moduleConfig;

        private readonly DispatcherQueueTimer _visualBellTimer;

        private TerminalViewModel? _viewModel;

        /// <summary>
        /// The viewmodel.
        /// </summary>
        private TerminalViewModel? ViewModel
        {
            get => _viewModel;

            set
            {
                _viewModel = value;
                DataContext = _viewModel;
            }
        }

        /// <summary>
        /// Initializes a <see cref="TerminalView"/>.
        /// </summary>
        /// <param name="config"><inheritdoc cref="ModuleView.ModuleView"
        /// path="/param[@name='config']"/></param>
        /// <param name="moduleConfig">The <see
        /// cref="Configuration.Json.Modules.Terminal"/> for this <see
        /// cref="TerminalView"/>.</param>
        internal TerminalView(Configuration.Json.TermBar config, Configuration.Json.Modules.Terminal moduleConfig) : base(config, moduleConfig)
        {
            _config = config;
            _moduleConfig = moduleConfig;
            _visualBellTimer = DispatcherQueue.CreateTimer();
            _visualBellTimer.Interval = TimeSpan.FromMilliseconds(moduleConfig.VisualBellDisplayTime);
            _visualBellTimer.Tick += VisualBellTimer_Tick;

            ViewModel = new TerminalViewModel(this, config, moduleConfig, GetPalette());

            InitializeComponent();
            ApplyComputedGridStyle();
            ApplyComputedTextBoxStyle(config);

            TerminalControl.WindowTitleChanged += TerminalControl_WindowTitleChanged;
            TerminalControl.VisualBellRinging += TerminalControl_VisualBellRinging;
        }

        /// <summary>
        /// Returns a <see cref="AnsiProcessor.AnsiColors.Palette"/>
        /// representing <see cref="_moduleConfig"/>'s colors.
        /// </summary>
        /// <returns>A <see
        /// cref="AnsiProcessor.AnsiColors.Palette"/>.</returns>
        internal AnsiProcessor.AnsiColors.Palette GetPalette()
        {
            return new()
            {
                DefaultBackgroundColor = Palette.Instance[_config.Flavor].AnsiColors[_moduleConfig.Colors.DefaultColors.DefaultBackgroundColor].SDColor,
                DefaultForegroundColor = Palette.Instance[_config.Flavor].AnsiColors[_moduleConfig.Colors.DefaultColors.DefaultForegroundColor].SDColor,
                DefaultUnderlineColor = Palette.Instance[_config.Flavor].AnsiColors[_moduleConfig.Colors.DefaultColors.DefaultUnderlineColor].SDColor,

                Black = Palette.Instance[_config.Flavor].AnsiColors[_moduleConfig.Colors.StandardColors.Black].SDColor,
                Red = Palette.Instance[_config.Flavor].AnsiColors[_moduleConfig.Colors.StandardColors.Red].SDColor,
                Green = Palette.Instance[_config.Flavor].AnsiColors[_moduleConfig.Colors.StandardColors.Green].SDColor,
                Yellow = Palette.Instance[_config.Flavor].AnsiColors[_moduleConfig.Colors.StandardColors.Yellow].SDColor,
                Blue = Palette.Instance[_config.Flavor].AnsiColors[_moduleConfig.Colors.StandardColors.Blue].SDColor,
                Magenta = Palette.Instance[_config.Flavor].AnsiColors[_moduleConfig.Colors.StandardColors.Magenta].SDColor,
                Cyan = Palette.Instance[_config.Flavor].AnsiColors[_moduleConfig.Colors.StandardColors.Cyan].SDColor,
                White = Palette.Instance[_config.Flavor].AnsiColors[_moduleConfig.Colors.StandardColors.White].SDColor,

                BrightBlack = Palette.Instance[_config.Flavor].AnsiColors[_moduleConfig.Colors.BrightColors.BrightBlack].SDColor,
                BrightRed = Palette.Instance[_config.Flavor].AnsiColors[_moduleConfig.Colors.BrightColors.BrightRed].SDColor,
                BrightGreen = Palette.Instance[_config.Flavor].AnsiColors[_moduleConfig.Colors.BrightColors.BrightGreen].SDColor,
                BrightYellow = Palette.Instance[_config.Flavor].AnsiColors[_moduleConfig.Colors.BrightColors.BrightYellow].SDColor,
                BrightBlue = Palette.Instance[_config.Flavor].AnsiColors[_moduleConfig.Colors.BrightColors.BrightBlue].SDColor,
                BrightMagenta = Palette.Instance[_config.Flavor].AnsiColors[_moduleConfig.Colors.BrightColors.BrightMagenta].SDColor,
                BrightCyan = Palette.Instance[_config.Flavor].AnsiColors[_moduleConfig.Colors.BrightColors.BrightCyan].SDColor,
                BrightWhite = Palette.Instance[_config.Flavor].AnsiColors[_moduleConfig.Colors.BrightColors.BrightWhite].SDColor
            };
        }

        /// <summary>
        /// Writes <paramref name="message"/> to the terminal.
        /// </summary>
        /// <param name="message">The message to write.</param>
        internal void Write(string message) => TerminalControl.Write(message);

        /// <summary>
        /// Clears the visual bell.
        /// </summary>
        /// <remarks>Hides the visual bell.</remarks>
        /// <param name="sender"><inheritdoc
        /// cref="TypedEventHandler{TSender, TResult}"
        /// path="/param[@name='sender']"/></param>
        /// <param name="args"><inheritdoc
        /// cref="TypedEventHandler{TSender, TResult}"
        /// path="/param[@name='args']"/></param>
        private void VisualBellTimer_Tick(DispatcherQueueTimer sender, object args)
        {
            Icon.Text = _moduleConfig.Icon;
            TerminalControl.VisualBell = false;
        }

        /// <summary>
        /// Updates the terminal window title.
        /// </summary>
        private void TerminalControl_WindowTitleChanged() => _viewModel!.WindowTitle = TerminalControl.WindowTitle;

        /// <summary>
        /// Displays the visual bell.
        /// </summary>
        private void TerminalControl_VisualBellRinging()
        {
            Icon.Text = _moduleConfig.VisualBellIcon;
            _visualBellTimer.Start();
        }

        /// <summary>
        /// Applies computed styles to <see cref="Grid"/>.
        /// </summary>
        private void ApplyComputedGridStyle()
        {
            Style gridStyle = new(typeof(Grid));

            StylesHelper.MergeWithAncestor(gridStyle, (Grid)Content, typeof(Grid));

            gridStyle.Setters.Add(new Setter(Grid.ColumnSpacingProperty, _config.Padding));

            Resources[typeof(Grid)] = gridStyle;
        }

        /// <summary>
        /// Applies computed styles to <see cref="TextBox"/>.
        /// </summary>
        /// <param name="config"><inheritdoc cref="ModuleView.ModuleView"
        /// path="/param[@name='config']"/></param>
        private void ApplyComputedTextBoxStyle(Configuration.Json.TermBar config)
        {
            Style textBoxStyle = new(typeof(TextBox));

            StylesHelper.MergeWithAncestor(textBoxStyle, this, typeof(TextBox));

            textBoxStyle.Setters.Add(new Setter(TextBox.FontFamilyProperty, new FontFamily(config.FontFamily)));
            textBoxStyle.Setters.Add(new Setter(TextBox.FontSizeProperty, config.FontSize));
            textBoxStyle.Setters.Add(new Setter(TextBox.TextWrappingProperty, TextWrapping.Wrap));
            textBoxStyle.Setters.Add(new Setter(TextBox.IsSpellCheckEnabledProperty, false));

            textBoxStyle.Setters.Add(
                new Setter(
                    TextBox.ForegroundProperty,
                    Palette.Instance[config.Flavor].Colors[config.TextColor].SolidColorBrush
                )
            );

            Resources[typeof(TextBox)] = textBoxStyle;
        }

        /// <summary>
        /// Binds the the configuration to the terminal's <see
        /// cref="DependencyProperty"/>s.
        /// </summary>
        /// <param name="sender"><inheritdoc cref="RoutedEventHandler"
        /// path="/param[@name='sender']"/></param>
        /// <param name="e"><inheritdoc cref="RoutedEventHandler"
        /// path="/param[@name='e']"/></param>
        private void TerminalControl_Loaded(object sender, RoutedEventArgs e) => ViewModel!.BindSettings(TerminalControl);
    }
}