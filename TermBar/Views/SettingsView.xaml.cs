using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using System.Diagnostics;
using TermBar.Catppuccin;
using TermBar.Configuration;
using TermBar.Styles;
using Windows.ApplicationModel.DataTransfer;

namespace TermBar.Views {
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

    /// <inheritdoc cref="ConfigHelper.ConfigPath"/>
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CA1822 // Mark members as static
    private string ConfigPath => ConfigHelper.ConfigPath;
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

      ApplyComputedStyles();
      InitializeComponent();
    }

    /// <summary>
    /// Applies computed styles to the window.
    /// </summary>
    private void ApplyComputedStyles() {
      ApplyComputedGridStyle();
      ApplyComputedStackPanelStyle();
      ApplyComputedProseStyle();
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
      textBlockStyle.Setters.Add(new Setter(TextBlock.PaddingProperty, $"0,{config.Padding / 2}"));

      Resources["Prose"] = textBlockStyle;
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
            ? PaletteHelper.Palette[config.Flavor].Colors[(ColorEnum) config.AccentBackground].SolidColorBrush
            : Application.Current.Resources["SystemAccentColorLight3"]
        )
      );

      accentedTextBlockStyle.Setters.Add(
        new Setter(
          TextBlock.ForegroundProperty,
          config.AccentColor is not null
            ? PaletteHelper.Palette[config.Flavor].Colors[(ColorEnum) config.AccentColor].SolidColorBrush
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
    /// Invoked when the user clicks the OK button.
    /// </summary>
    /// <param name="sender"><inheritdoc cref="RoutedEventHandler"
    /// path="/param[@name='sender']"/></param>
    /// <param name="e"><inheritdoc cref="RoutedEventHandler"
    /// path="/param[@name='e']"/></param>
    private void OK_Click(object sender, RoutedEventArgs e) => Owner?.Close();

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
