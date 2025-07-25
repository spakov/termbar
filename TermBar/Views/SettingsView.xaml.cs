using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using Spakov.Catppuccin;
using Spakov.TermBar.Configuration;
using Spakov.TermBar.Styles;
using Spakov.TermBar.WindowManagement;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;

namespace Spakov.TermBar.Views
{
    /// <summary>
    /// The TermBar settings view.
    /// </summary>
    /// <remarks>This is used as the child element of the settings
    /// window.</remarks>
    internal sealed partial class SettingsView : ModuleView
    {
        private const string TermBar = "TermBar";
        private const string Explorer = "explorer";
        private const string ExplorerSelect = "/select,\"{0}\"";

        private readonly Configuration.Json.TermBar _config;

        private readonly string _runtimeConfig;
        private readonly JsonFormatSyntaxColors _jsonFormatSyntaxColors;

        private readonly DataPackage _dataPackage = new();

        /// <summary>
        /// The owning window.
        /// </summary>
        internal WindowManagement.Windows.DialogWindow? Owner { get; set; }

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CA1822 // Mark members as static

        /// <inheritdoc cref="ConfigHelper.ConfigPath"/>
        private string ConfigPath => ConfigHelper.ConfigPath;

        /// <summary>
        /// The list of detected displays.
        /// </summary>
        private string DisplayList { get; set; }

#pragma warning restore CA1822 // Mark members as static
#pragma warning restore IDE0079 // Remove unnecessary suppression

        /// <summary>
        /// Initializes a <see cref="SettingsView"/>.
        /// </summary>
        /// <param name="config"><inheritdoc cref="ModuleView.ModuleView"
        /// path="/param[@name='config']"/></param>
        internal SettingsView(Configuration.Json.TermBar config) : base(config)
        {
            _config = config;

            StringBuilder displayList = new();

            foreach (KeyValuePair<string, Display> display in DisplayHelper.GetDisplays())
            {
                displayList.AppendLine(display.Key);
            }

            DisplayList = displayList.ToString();

            ApplyComputedStyles();
            InitializeComponent();

            _runtimeConfig = JsonSerializer.Serialize(App.Config, Configuration.Json.ConfigContext.Default.Config);
            _jsonFormatSyntaxColors = new(
                defaultColor: Palette.Instance[config.Flavor].Colors[ColorEnum.Lavender].WUIColor,
                objectColor: Palette.Instance[config.Flavor].Colors[ColorEnum.Text].WUIColor,
                stringColor: Palette.Instance[config.Flavor].Colors[ColorEnum.Sky].WUIColor,
                numberColor: Palette.Instance[config.Flavor].Colors[ColorEnum.Maroon].WUIColor,
                booleanColor: Palette.Instance[config.Flavor].Colors[ColorEnum.Blue].WUIColor,
                nullColor: Palette.Instance[config.Flavor].Colors[ColorEnum.Mauve].WUIColor
            );

            RuntimeConfig.ActualThemeChanged += RuntimeConfig_ActualThemeChanged;

            JsonFormatter.FormatJson(
                RuntimeConfig,
                _runtimeConfig,
                _jsonFormatSyntaxColors
            );
        }

        /// <summary>
        /// Updates formatted JSON when the system theme changes.
        /// </summary>
        /// <param name="sender"><inheritdoc
        /// cref="TypedEventHandler{TSender, TResult}"
        /// path="/param[@name='sender']"/></param>
        /// <param name="args"><inheritdoc
        /// cref="TypedEventHandler{TSender, TResult}"
        /// path="/param[@name='args']"/></param>
        private void RuntimeConfig_ActualThemeChanged(FrameworkElement sender, object args)
        {
            // This property actually matters, even programmatically
            RuntimeConfig.IsReadOnly = false;

            JsonFormatter.FormatJson(
                RuntimeConfig,
                _runtimeConfig,
                _jsonFormatSyntaxColors
            );

            RuntimeConfig.IsReadOnly = true;
        }

        /// <summary>
        /// Applies computed styles to the window.
        /// </summary>
        private void ApplyComputedStyles()
        {
            ApplyComputedGridStyle();
            ApplyComputedStackPanelStyle();
            ApplyComputedProseStyle();
            ApplyComputedRichEditBoxStyle();
            ApplyComputedTextBoxStyle();
            ApplyComputedAccentStyles();
        }

        /// <summary>
        /// Applies computed styles to <c>LayoutGrid</c>.
        /// </summary>
        private void ApplyComputedGridStyle()
        {
            Style gridStyle = new(typeof(Grid));

            StylesHelper.MergeWithAncestor(gridStyle, (Border)Content, typeof(Grid));

            gridStyle.Setters.Add(new Setter(Grid.PaddingProperty, _config.Padding.ToString()));
            gridStyle.Setters.Add(new Setter(Grid.RowSpacingProperty, _config.Padding));

            Resources["LayoutGrid"] = gridStyle;
        }

        /// <summary>
        /// Applies computed styles to <see cref="StackPanel"/>.
        /// </summary>
        private void ApplyComputedStackPanelStyle()
        {
            Style stackPanelStyle = new(typeof(StackPanel));

            StylesHelper.MergeWithAncestor(stackPanelStyle, (Border)Content, typeof(StackPanel));

            stackPanelStyle.Setters.Add(new Setter(StackPanel.SpacingProperty, _config.Padding.ToString()));

            Resources[typeof(StackPanel)] = stackPanelStyle;
        }

        /// <summary>
        /// Applies computed styles to <c>Prose</c>.
        /// </summary>
        private void ApplyComputedProseStyle()
        {
            Style textBlockStyle = new(typeof(TextBlock));

            StylesHelper.MergeWithAncestor(textBlockStyle, this, typeof(TextBlock));

            textBlockStyle.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Left));
            textBlockStyle.Setters.Add(new Setter(TextBlock.TextWrappingProperty, TextWrapping.WrapWholeWords));

            Resources["Prose"] = textBlockStyle;
        }

        /// <summary>
        /// Applies computed styles to <see cref="RichEditBox"/>.
        /// </summary>
        private void ApplyComputedRichEditBoxStyle()
        {
            Style richEditBoxStyle = new(typeof(RichEditBox));

            StylesHelper.MergeWithAncestor(richEditBoxStyle, (Border)Content, typeof(RichEditBox));

            richEditBoxStyle.Setters.Add(new Setter(RichEditBox.FontFamilyProperty, new FontFamily(_config.FontFamily)));
            richEditBoxStyle.Setters.Add(new Setter(RichEditBox.FontSizeProperty, _config.FontSize));
            richEditBoxStyle.Setters.Add(new Setter(RichEditBox.IsReadOnlyProperty, true));
            richEditBoxStyle.Setters.Add(new Setter(RichEditBox.AcceptsReturnProperty, true));
            richEditBoxStyle.Setters.Add(new Setter(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Auto));
            richEditBoxStyle.Setters.Add(new Setter(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Hidden));

            Resources[typeof(RichEditBox)] = richEditBoxStyle;
        }

        /// <summary>
        /// Applies computed styles to <see cref="TextBox"/>.
        /// </summary>
        private void ApplyComputedTextBoxStyle()
        {
            Style textBoxStyle = new(typeof(TextBox));

            StylesHelper.MergeWithAncestor(textBoxStyle, (Border)Content, typeof(TextBox));

            textBoxStyle.Setters.Add(new Setter(TextBox.FontFamilyProperty, new FontFamily(_config.FontFamily)));
            textBoxStyle.Setters.Add(new Setter(TextBox.FontSizeProperty, _config.FontSize));
            textBoxStyle.Setters.Add(new Setter(TextBox.ForegroundProperty, Palette.Instance[_config.Flavor].Colors[ColorEnum.Text].SolidColorBrush));
            textBoxStyle.Setters.Add(new Setter(TextBox.IsReadOnlyProperty, true));
            textBoxStyle.Setters.Add(new Setter(TextBox.AcceptsReturnProperty, true));
            textBoxStyle.Setters.Add(new Setter(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Auto));
            textBoxStyle.Setters.Add(new Setter(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Auto));

            Resources[typeof(TextBox)] = textBoxStyle;
        }

        /// <summary>
        /// Applies computed styles to <c>AccentedButton</c> and
        /// <c>AccentedTextBlock</c>.
        /// </summary>
        private void ApplyComputedAccentStyles()
        {
            Style accentedButtonStyle = new(typeof(Button));
            Style accentedTextBlockStyle = new(typeof(TextBlock));

            StylesHelper.MergeWithAncestor(accentedButtonStyle, this, typeof(Button));
            StylesHelper.MergeWithAncestor(accentedTextBlockStyle, this, typeof(TextBlock));

            accentedButtonStyle.Setters.Add(
                new Setter(
                    Button.BackgroundProperty,
                    _config.AccentBackground is not null
                        ? Palette.Instance[_config.Flavor].Colors[(ColorEnum)_config.AccentBackground].SolidColorBrush
                        : Application.Current.Resources["SystemAccentColorLight3"]
                )
            );

            accentedTextBlockStyle.Setters.Add(
                new Setter(
                    TextBlock.ForegroundProperty,
                    _config.AccentColor is not null
                        ? Palette.Instance[_config.Flavor].Colors[(ColorEnum)_config.AccentColor].SolidColorBrush
                        : Application.Current.Resources["SystemAccentColor"]
                )
            );

            Resources["AccentedButton"] = accentedButtonStyle;
            Resources["AccentedTextBlock"] = accentedTextBlockStyle;
        }

        /// <summary>
        /// Copies the config path to the clipboard.
        /// </summary>
        /// <param name="sender"><inheritdoc cref="RoutedEventHandler"
        /// path="/param[@name='sender']"/></param>
        /// <param name="e"><inheritdoc cref="RoutedEventHandler"
        /// path="/param[@name='e']"/></param>
        private void CopyPath_Click(object sender, RoutedEventArgs e)
        {
            _dataPackage.RequestedOperation = DataPackageOperation.Copy;
            _dataPackage.SetText(ConfigPath);
            Clipboard.SetContent(_dataPackage);

            AppNotificationManager.Default.Show(
                new AppNotificationBuilder()
                    .AddText(App.ResourceLoader.GetString("PathCopied"))
                    .AddText(App.ResourceLoader.GetString("ConfigurationFilePathCopied"))
                    .BuildNotification()
            );
        }

        /// <summary>
        /// Shows the config file in Explorer.
        /// </summary>
        /// <param name="sender"><inheritdoc cref="RoutedEventHandler"
        /// path="/param[@name='sender']"/></param>
        /// <param name="e"><inheritdoc cref="RoutedEventHandler"
        /// path="/param[@name='e']"/></param>
        private void ShowInExplorer_Click(object sender, RoutedEventArgs e) => Process.Start(Explorer, string.Format(ExplorerSelect, ConfigPath));

        /// <summary>
        /// Closes the settings window.
        /// </summary>
        /// <param name="sender"><inheritdoc cref="RoutedEventHandler"
        /// path="/param[@name='sender']"/></param>
        /// <param name="e"><inheritdoc cref="RoutedEventHandler"
        /// path="/param[@name='e']"/></param>
        private void Close_Click(object sender, RoutedEventArgs e) => Owner?.Close();

        /// <summary>
        /// Exits.
        /// </summary>
        /// <param name="sender"><inheritdoc cref="RoutedEventHandler"
        /// path="/param[@name='sender']"/></param>
        /// <param name="e"><inheritdoc cref="RoutedEventHandler"
        /// path="/param[@name='e']"/></param>
        private void Exit_Click(object sender, RoutedEventArgs e) => Application.Current.Exit();

        /// <summary>
        /// Restarts TermBar.
        /// </summary>
        /// <param name="sender"><inheritdoc cref="RoutedEventHandler"
        /// path="/param[@name='sender']"/></param>
        /// <param name="e"><inheritdoc cref="RoutedEventHandler"
        /// path="/param[@name='e']"/></param>
        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(TermBar);
            Application.Current.Exit();
        }
    }
}