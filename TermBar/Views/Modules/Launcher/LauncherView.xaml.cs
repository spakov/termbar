using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using TermBar.Catppuccin;
using TermBar.ViewModels.Modules.Launcher;

namespace TermBar.Views.Modules.Launcher {
  /// <summary>
  /// The TermBar launcher.
  /// </summary>
  internal sealed partial class LauncherView : ModuleView {
    private LauncherViewModel? viewModel;

    /// <summary>
    /// The viewmodel.
    /// </summary>
    private LauncherViewModel? ViewModel {
      get => viewModel;
      set {
        viewModel = value;
        DataContext = viewModel;
      }
    }

    /// <summary>
    /// Initializes a <see cref="LauncherView"/>.
    /// </summary>
    /// <param name="config"><inheritdoc cref="ModuleView.ModuleView"
    /// path="/param[@name='config']"/></param>
    /// <param name="moduleConfig">The <see
    /// cref="Configuration.Json.Modules.Launcher"/> for this <see
    /// cref="LauncherView"/>.</param>
    internal LauncherView(Configuration.Json.TermBar config, Configuration.Json.Modules.Launcher moduleConfig) : base(config, moduleConfig) {
      ViewModel = new LauncherViewModel(config, moduleConfig);
      DataContext = ViewModel;

      InitializeComponent();
      ApplyComputedStyles(config);
    }

    /// <summary>
    /// Applies computed styles to the <see cref="ListView"/>.
    /// </summary>
    /// <param name="config"><inheritdoc
    /// cref="Configuration.Json.TermBar"
    /// path="/param[@name='config']"/></param>
    private void ApplyComputedStyles(Configuration.Json.TermBar config) {
      Style textBlockStyle = new(typeof(TextBlock));

      textBlockStyle.Setters.Add(new Setter(TextBlock.FontFamilyProperty, new FontFamily(config.FontFamily)));
      textBlockStyle.Setters.Add(new Setter(TextBlock.FontSizeProperty, config.FontSize));

      textBlockStyle.Setters.Add(
        new Setter(
          TextBlock.ForegroundProperty,
          PaletteHelper.Palette[config.Flavor].Colors[config.TextColor].SolidColorBrush
        )
      );

      ListView.Resources[typeof(TextBlock)] = textBlockStyle;
    }
  }
}
