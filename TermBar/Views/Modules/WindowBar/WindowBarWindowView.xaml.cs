using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TermBar.Styles;
using TermBar.ViewModels.Modules.WindowBar;
using Windows.Win32.Foundation;

namespace TermBar.Views.Modules.WindowBar {
  /// <summary>
  /// The TermBar clock.
  /// </summary>
  internal sealed partial class WindowBarWindowView : ModuleView {
    private WindowBarWindowViewModel? viewModel;

    /// <summary>
    /// The viewmodel.
    /// </summary>
    private WindowBarWindowViewModel? ViewModel {
      get => viewModel;
      set {
        viewModel = value;
        DataContext = viewModel;
      }
    }

    /// <summary>
    /// The window's <see cref="HWND"/>.
    /// </summary>
    internal HWND HWnd { get; private set; }

    /// <summary>
    /// Allows updating the window's name.
    /// </summary>
    internal string? WindowName {
      set {
        if (viewModel is not null) {
          viewModel.Name = value;
        }
      }
    }

    /// <summary>
    /// Initializes a <see cref="WindowBarWindowView"/>.
    /// </summary>
    /// <param name="config"><inheritdoc cref="ModuleView.ModuleView"
    /// path="/param[@name='config']"/></param>
    /// <param name="moduleConfig">The <see
    /// cref="Configuration.Json.Modules.WindowBar"/> for this <see
    /// cref="WindowBarWindowView"/>.</param>
    /// <param name="hWnd"><inheritdoc cref="HWnd" path="/summary"/></param>
    /// <param name="processId">The window's owning process ID.</param>
    /// <param name="name">The window's name.</param>
    internal WindowBarWindowView(Configuration.Json.TermBar config, Configuration.Json.Modules.WindowBar moduleConfig, HWND hWnd, uint processId, string name) : base(config, moduleConfig, skipColor: true) {
      HWnd = hWnd;

      ViewModel = new WindowBarWindowViewModel(config, moduleConfig, processId, name);

      InitializeComponent();
      ApplyComputedStyles(moduleConfig);
    }

    /// <summary>
    /// Applies computed styles to the <see cref="Grid"/>.
    /// </summary>
    /// <param name="moduleConfig"><inheritdoc
    /// cref="WindowBarView.WindowBarView"
    /// path="/param[@name='config']"/></param>
    private void ApplyComputedStyles(Configuration.Json.Modules.WindowBar moduleConfig) {
      if (moduleConfig.Windows is null) return;

      Style gridStyle = new(typeof(Grid));

      if (moduleConfig.Windows.FixedSize is not null) {
        gridStyle.Setters.Add(new Setter(Grid.WidthProperty, moduleConfig.Windows.FixedSize));
      }

      if (moduleConfig.Windows.MaxLength is not null) {
        gridStyle.Setters.Add(new Setter(Grid.MaxWidthProperty, moduleConfig.Windows.MaxLength));
      }

      ((Grid) Content).Style = gridStyle;

      if (moduleConfig.Windows.NiceTruncation) {
        Style textBlockStyle = new(typeof(TextBlock));

        StylesHelper.MergeWithAncestor(textBlockStyle, this, typeof(TextBlock));

        textBlockStyle.Setters.Add(new Setter(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis));
        textBlockStyle.Setters.Add(new Setter(TextBlock.TextWrappingProperty, TextWrapping.NoWrap));

        Resources[typeof(TextBlock)] = textBlockStyle;
      }
    }
  }
}
