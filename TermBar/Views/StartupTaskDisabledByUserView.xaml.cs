using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Spakov.Catppuccin;
using Spakov.TermBar.Styles;
using System.Diagnostics;

namespace Spakov.TermBar.Views {
  /// <summary>
  /// The TermBar startup task disabled by user view.
  /// </summary>
  internal sealed partial class StartupTaskDisabledByUserView : ModuleView {
    private const string explorer = "explorer";
    private const string msSettingsStartupApps = "ms-settings:startupapps";

    private readonly Configuration.Json.TermBar config;

    /// <summary>
    /// The owning window.
    /// </summary>
    internal WindowManagement.Windows.DialogWindow? Owner { get; set; }

    /// <summary>
    /// Initializes a <see cref="StartupTaskDisabledByUserView"/>.
    /// </summary>
    /// <param name="config"><inheritdoc cref="ModuleView.ModuleView"
    /// path="/param[@name='config']"/></param>
    internal StartupTaskDisabledByUserView(Configuration.Json.TermBar config) : base(config) {
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
    /// Applies computed styles to <c>LayoutGrid</c>.
    /// </summary>
    private void ApplyComputedGridStyle() {
      Style gridStyle = new(typeof(Grid));

      StylesHelper.MergeWithAncestor(gridStyle, (Border) Content, typeof(Grid));

      gridStyle.Setters.Add(new Setter(Grid.PaddingProperty, config.Padding.ToString()));
      gridStyle.Setters.Add(new Setter(Grid.RowSpacingProperty, config.Padding));

      Resources["LayoutGrid"] = gridStyle;
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
    /// Invoked when the user clicks the "open Windows startup settings"
    /// button.
    /// </summary>
    /// <param name="sender"><inheritdoc cref="RoutedEventHandler"
    /// path="/param[@name='sender']"/></param>
    /// <param name="e"><inheritdoc cref="RoutedEventHandler"
    /// path="/param[@name='e']"/></param>
    private void OpenSettings_Click(object sender, RoutedEventArgs e) => Process.Start(explorer, msSettingsStartupApps);

    /// <summary>
    /// Invoked when the user clicks the Close button.
    /// </summary>
    /// <param name="sender"><inheritdoc cref="RoutedEventHandler"
    /// path="/param[@name='sender']"/></param>
    /// <param name="e"><inheritdoc cref="RoutedEventHandler"
    /// path="/param[@name='e']"/></param>
    private void Close_Click(object sender, RoutedEventArgs e) => Owner?.Close();
  }
}