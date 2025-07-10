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

namespace Spakov.TermBar.Views {
  /// <summary>
  /// The TermBar settings view.
  /// </summary>
  internal sealed partial class SettingsView : ModuleView {
    private const string termBar = "TermBar.exe";
    private const string explorer = "explorer.exe";

    private readonly Configuration.Json.TermBar config;

    private readonly DataPackage dataPackage = new();

    /// <summary>
    /// The owning window.
    /// </summary>
    internal WindowManagement.Windows.DialogWindow? Owner { get; set; }

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CA1822 // Mark members as static

    /// <inheritdoc cref="ConfigHelper.ConfigPath"/>
    private string ConfigPath => ConfigHelper.ConfigPath;

    /// <summary>
    /// The runtime JSON configuration.
    /// </summary>
    //private string RuntimeConfig { get; set; }

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
    /// <param name="moduleConfig">The <see
    /// cref="Configuration.Json.Modules.Clock"/> for this <see
    /// cref="ClockCalendarView"/>.</param>
    internal SettingsView(Configuration.Json.TermBar config) : base(config) {
      this.config = config;

      StringBuilder displayList = new();

      foreach (KeyValuePair<string, Display> display in DisplayHelper.GetDisplays()) {
        displayList.AppendLine(display.Key);
      }

      DisplayList = displayList.ToString();

      ApplyComputedStyles();
      InitializeComponent();

      JsonFormatter.FormatJson(
        RuntimeConfig,
        JsonSerializer.Serialize(App.Config, Configuration.Json.ConfigContext.Default.Config),
        new(
          defaultColor: Palette.Instance[config.Flavor].Colors[ColorEnum.Lavender].WUIColor,
          objectColor: Palette.Instance[config.Flavor].Colors[ColorEnum.Text].WUIColor,
          stringColor: Palette.Instance[config.Flavor].Colors[ColorEnum.Sky].WUIColor,
          numberColor: Palette.Instance[config.Flavor].Colors[ColorEnum.Maroon].WUIColor,
          booleanColor: Palette.Instance[config.Flavor].Colors[ColorEnum.Blue].WUIColor,
          nullColor: Palette.Instance[config.Flavor].Colors[ColorEnum.Mauve].WUIColor
        )
      );
    }

    /// <summary>
    /// Applies computed styles to the window.
    /// </summary>
    private void ApplyComputedStyles() {
      ApplyComputedGridStyle();
      ApplyComputedStackPanelStyle();
      ApplyComputedProseStyle();
      ApplyComputedRichEditBoxStyle();
      ApplyComputedTextBoxStyle();
      ApplyComputedAccentStyles();
    }

    /// <summary>
    /// Applies computed styles to <see cref="Grid"/>.
    /// </summary>
    private void ApplyComputedGridStyle() {
      Style gridStyle = new(typeof(Grid));

      StylesHelper.MergeWithAncestor(gridStyle, (Border) Content, typeof(Grid));

      gridStyle.Setters.Add(new Setter(Grid.PaddingProperty, config.Padding.ToString()));
      gridStyle.Setters.Add(new Setter(Grid.RowSpacingProperty, config.Padding));

      Resources[typeof(Grid)] = gridStyle;
    }

    /// <summary>
    /// Applies computed styles to <see cref="StackPanel"/>.
    /// </summary>
    private void ApplyComputedStackPanelStyle() {
      Style stackPanelStyle = new(typeof(StackPanel));

      StylesHelper.MergeWithAncestor(stackPanelStyle, (Border) Content, typeof(StackPanel));

      stackPanelStyle.Setters.Add(new Setter(StackPanel.SpacingProperty, config.Padding.ToString()));

      Resources[typeof(StackPanel)] = stackPanelStyle;
    }

    /// <summary>
    /// Applies computed styles to <c>Prose</c>.
    /// </summary>
    private void ApplyComputedProseStyle() {
      Style textBlockStyle = new(typeof(TextBlock));

      StylesHelper.MergeWithAncestor(textBlockStyle, this, typeof(TextBlock));

      textBlockStyle.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Left));
      textBlockStyle.Setters.Add(new Setter(TextBlock.TextWrappingProperty, TextWrapping.WrapWholeWords));

      Resources["Prose"] = textBlockStyle;
    }

    /// <summary>
    /// Applies computed styles to <see cref="RichEditBox"/>.
    /// </summary>
    private void ApplyComputedRichEditBoxStyle() {
      Style richEditBoxStyle = new(typeof(RichEditBox));

      StylesHelper.MergeWithAncestor(richEditBoxStyle, (Border) Content, typeof(RichEditBox));

      richEditBoxStyle.Setters.Add(new Setter(RichEditBox.FontFamilyProperty, new FontFamily(config.FontFamily)));
      richEditBoxStyle.Setters.Add(new Setter(RichEditBox.FontSizeProperty, config.FontSize));
      richEditBoxStyle.Setters.Add(new Setter(RichEditBox.IsReadOnlyProperty, true));
      richEditBoxStyle.Setters.Add(new Setter(RichEditBox.AcceptsReturnProperty, true));
      richEditBoxStyle.Setters.Add(new Setter(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Auto));
      richEditBoxStyle.Setters.Add(new Setter(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Auto));

      Resources[typeof(RichEditBox)] = richEditBoxStyle;
    }

    /// <summary>
    /// Applies computed styles to <see cref="TextBox"/>.
    /// </summary>
    private void ApplyComputedTextBoxStyle() {
      Style textBoxStyle = new(typeof(TextBox));

      StylesHelper.MergeWithAncestor(textBoxStyle, (Border) Content, typeof(TextBox));

      textBoxStyle.Setters.Add(new Setter(TextBox.FontFamilyProperty, new FontFamily(config.FontFamily)));
      textBoxStyle.Setters.Add(new Setter(TextBox.FontSizeProperty, config.FontSize));
      textBoxStyle.Setters.Add(new Setter(TextBox.ForegroundProperty, Palette.Instance[config.Flavor].Colors[ColorEnum.Text].SolidColorBrush));
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
    private void ApplyComputedAccentStyles() {
      Style accentedButtonStyle = new(typeof(Button));
      Style accentedTextBlockStyle = new(typeof(TextBlock));

      StylesHelper.MergeWithAncestor(accentedButtonStyle, this, typeof(Button));
      StylesHelper.MergeWithAncestor(accentedTextBlockStyle, this, typeof(TextBlock));

      accentedButtonStyle.Setters.Add(
        new Setter(
          Button.BackgroundProperty,
          config.AccentBackground is not null
            ? Palette.Instance[config.Flavor].Colors[(ColorEnum) config.AccentBackground].SolidColorBrush
            : Application.Current.Resources["SystemAccentColorLight3"]
        )
      );

      accentedTextBlockStyle.Setters.Add(
        new Setter(
          TextBlock.ForegroundProperty,
          config.AccentColor is not null
            ? Palette.Instance[config.Flavor].Colors[(ColorEnum) config.AccentColor].SolidColorBrush
            : Application.Current.Resources["SystemAccentColor"]
        )
      );

      Resources["AccentedButton"] = accentedButtonStyle;
      Resources["AccentedTextBlock"] = accentedTextBlockStyle;
    }

    /// <summary>
    /// Invoked when the user clicks on the config path.
    /// </summary>
    /// <param name="sender"><inheritdoc cref="RoutedEventHandler"
    /// path="/param[@name='sender']"/></param>
    /// <param name="e"><inheritdoc cref="RoutedEventHandler"
    /// path="/param[@name='e']"/></param>
    private void CopyPath_Click(object sender, RoutedEventArgs e) {
      dataPackage.RequestedOperation = DataPackageOperation.Copy;
      dataPackage.SetText(ConfigPath);
      Clipboard.SetContent(dataPackage);

      AppNotificationManager.Default.Show(
        new AppNotificationBuilder()
          .AddText("Path copied")
          .AddText("The configuration file path has been copied to the clipboard.")
          .BuildNotification()
      );
    }

    /// <summary>
    /// Invoked when the user clicks the "show in Explorer" button.
    /// </summary>
    /// <param name="sender"><inheritdoc cref="RoutedEventHandler"
    /// path="/param[@name='sender']"/></param>
    /// <param name="e"><inheritdoc cref="RoutedEventHandler"
    /// path="/param[@name='e']"/></param>
    private void ShowInExplorer_Click(object sender, RoutedEventArgs e) => Process.Start(explorer, $"/select,\"{ConfigPath}\"");

    /// <summary>
    /// Invoked when the user clicks the Close button.
    /// </summary>
    /// <param name="sender"><inheritdoc cref="RoutedEventHandler"
    /// path="/param[@name='sender']"/></param>
    /// <param name="e"><inheritdoc cref="RoutedEventHandler"
    /// path="/param[@name='e']"/></param>
    private void Close_Click(object sender, RoutedEventArgs e) => Owner?.Close();

    /// <summary>
    /// Invoked when the user clicks the exit button.
    /// </summary>
    /// <param name="sender"><inheritdoc cref="RoutedEventHandler"
    /// path="/param[@name='sender']"/></param>
    /// <param name="e"><inheritdoc cref="RoutedEventHandler"
    /// path="/param[@name='e']"/></param>
    private void Exit_Click(object sender, RoutedEventArgs e) => Application.Current.Exit();

    /// <summary>
    /// Invoked when the user clicks the restart button.
    /// </summary>
    /// <param name="sender"><inheritdoc cref="RoutedEventHandler"
    /// path="/param[@name='sender']"/></param>
    /// <param name="e"><inheritdoc cref="RoutedEventHandler"
    /// path="/param[@name='e']"/></param>
    private void Restart_Click(object sender, RoutedEventArgs e) {
      Process.Start(termBar);
      Application.Current.Exit();
    }
  }
}