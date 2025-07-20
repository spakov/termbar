using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Spakov.Catppuccin;
using Spakov.TermBar.Styles;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace Spakov.TermBar.Views {
  /// <summary>
  /// The TermBar exception view.
  /// </summary>
  internal sealed partial class ExceptionView : ModuleView {
    private const string termBar = "TermBar";

    private readonly Configuration.Json.TermBar config;

    /// <summary>
    /// The owning window.
    /// </summary>
    internal WindowManagement.Windows.DialogWindow? Owner { get; set; }

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CA1822 // Mark members as static

    /// <summary>
    /// Whether the exception is fatal.
    /// </summary>
    internal bool IsFatal { get; }

    /// <summary>
    /// The likely cause of the exception.
    /// </summary>
    private string LikelyCause { get; set; }

    /// <summary>
    /// The exception message and stack trace.
    /// </summary>
    private string? Exception { get; set; }

#pragma warning restore CA1822 // Mark members as static
#pragma warning restore IDE0079 // Remove unnecessary suppression

    /// <summary>
    /// Initializes an <see cref="ExceptionView"/>.
    /// </summary>
    /// <param name="config"><inheritdoc cref="ModuleView.ModuleView"
    /// path="/param[@name='config']"/></param>
    /// <param name="e">The exception that led to this view being
    /// created.</param>
    internal ExceptionView(Configuration.Json.TermBar config, Exception e) : base(config) {
      this.config = config;

      IsFatal = GetIsFatal(e);
      LikelyCause = GetLikelyCause(e);

      StringBuilder exception = new();
      exception.Append(e.Message);
      exception.Append("\r\n\r\n");
      exception.Append(e.GetType());
      exception.Append("\r\n");
      exception.Append(e.StackTrace);

      Exception = exception.ToString();

      ApplyComputedStyles();
      InitializeComponent();

      if (IsFatal) {
        AProblemHasOccurred.Text = App.ResourceLoader.GetString("AProblemHasOccurred");

        Close.Visibility = Visibility.Collapsed;
      } else {
        AProblemHasOccurred.Text = App.ResourceLoader.GetString("AProblemHasOccurredNonFatal");

        Exit.Visibility = Visibility.Collapsed;
        Restart.Visibility = Visibility.Collapsed;
      }
    }

    /// <summary>
    /// Returns the likely cause of <paramref name="e"/> in human-readable
    /// text.
    /// </summary>
    /// <param name="e">An <see cref="System.Exception"/>.</param>
    /// <returns>The likely cause of <paramref name="e"/>.</returns>
    private static string GetLikelyCause(Exception e) {
      if (e is JsonException) {
        return App.ResourceLoader.GetString("ConfigurationFileError");
      } else if (e is Win32Exception && e.StackTrace is not null && e.StackTrace.Contains("Launcher.LauncherView")) {
        return App.ResourceLoader.GetString("BadLauncherEntry");
      }

      return App.ResourceLoader.GetString("FoundABug");
    }

    /// <summary>
    /// Determines whether or not <paramref name="e"/> is fatal.
    /// </summary>
    /// <param name="e">An <see cref="System.Exception"/>.</param>
    /// <returns>Whether <paramref name="e"/> is fatal.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Maximize readability")]
    private static bool GetIsFatal(Exception e) {
      if (e is Win32Exception && e.StackTrace is not null && e.StackTrace.Contains("Launcher.LauncherView")) {
        return false;
      }

      return true;
    }

    /// <summary>
    /// Applies computed styles to the window.
    /// </summary>
    private void ApplyComputedStyles() {
      ApplyComputedGridStyle();
      ApplyComputedStackPanelStyle();
      ApplyComputedProseStyle();
      ApplyComputedTextBoxStyle();
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
      Style accentedTextBlockStyle = new(typeof(TextBlock));

      StylesHelper.MergeWithAncestor(textBlockStyle, this, typeof(TextBlock));
      StylesHelper.MergeWithAncestor(accentedTextBlockStyle, this, typeof(TextBlock));

      textBlockStyle.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Left));
      textBlockStyle.Setters.Add(new Setter(TextBlock.TextWrappingProperty, TextWrapping.WrapWholeWords));

      Resources["Prose"] = textBlockStyle;

      accentedTextBlockStyle.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Left));
      accentedTextBlockStyle.Setters.Add(new Setter(TextBlock.TextWrappingProperty, TextWrapping.WrapWholeWords));
      accentedTextBlockStyle.Setters.Add(
        new Setter(
          TextBlock.ForegroundProperty,
          config.AccentColor is not null
            ? Palette.Instance[config.Flavor].Colors[(ColorEnum) config.AccentColor].SolidColorBrush
            : Application.Current.Resources["SystemAccentColor"]
        )
      );

      Resources["AccentedProse"] = accentedTextBlockStyle;
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
      textBoxStyle.Setters.Add(new Setter(TextBox.TextWrappingProperty, TextWrapping.Wrap));
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