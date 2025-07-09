using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.Diagnostics.CodeAnalysis;
using TermBar.Catppuccin;
using TermBar.Configuration.Json;
using TermBar.Styles;

namespace TermBar.Views {
  /// <summary>
  /// A TermBar module view.
  /// </summary>
  [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicConstructors)]
  internal partial class ModuleView : UserControl {
    /// <summary>
    /// Initializes a <see cref="ModuleView"/>.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>Applies styles to <see cref="TextBlock"/>s.</item>
    /// <item>If <paramref name="moduleConfig"/> is not <c>null</c>, sets up
    /// the <c>AccentBrush</c> resource with the accent color as a <see
    /// cref="SolidColorBrush"/>.</item>
    /// </list>
    /// </remarks>
    /// <param name="config">The <see cref="Configuration.Json.TermBar"/>
    /// configuration.</param>
    /// <param name="moduleConfig">The <see cref="IModule"/> configuration for
    /// this module.</param>
    /// <param name="skipColor">Whether to skip explicitly applying
    /// foreground color styles.</param>
    internal ModuleView(Configuration.Json.TermBar config, IModule? moduleConfig = null, bool skipColor = false) {
      if (moduleConfig is not null) {
        Resources["AccentBrush"] = PaletteHelper.Palette[config.Flavor].Colors[moduleConfig.AccentColor].SolidColorBrush;
      }

      ApplyComputedStyles(config, skipColor);
    }

    /// <summary>
    /// Applies computed styles to the module view.
    /// </summary>
    /// <param name="config"><inheritdoc cref="ModuleView.ModuleView"
    /// path="/param[@name='config']"/></param>
    /// <param name="skipColor"><inheritdoc cref="ModuleView.ModuleView"
    /// path="/param[@name='skipColor']"/></param>
    private void ApplyComputedStyles(Configuration.Json.TermBar config, bool skipColor = false) {
      Style textBlockStyle = new(typeof(TextBlock));

      StylesHelper.MergeWithAncestor(textBlockStyle, this, typeof(TextBlock));

      textBlockStyle.Setters.Add(new Setter(TextBlock.FontFamilyProperty, new FontFamily(config.FontFamily)));
      textBlockStyle.Setters.Add(new Setter(TextBlock.FontSizeProperty, config.FontSize));

      if (!skipColor) {
        textBlockStyle.Setters.Add(
          new Setter(
            TextBlock.ForegroundProperty,
            PaletteHelper.Palette[config.Flavor].Colors[config.TextColor].SolidColorBrush
          )
        );
      }

      Resources[typeof(TextBlock)] = textBlockStyle;
    }
  }
}
