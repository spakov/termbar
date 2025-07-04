using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.Diagnostics;
using TermBar.Catppuccin;
using TermBar.Styles;
using TermBar.ViewModels.Modules.Terminal;

namespace TermBar.Views.Modules.Terminal {
  /// <summary>
  /// The TermBar clock.
  /// </summary>
  internal sealed partial class TerminalView : ModuleView {
    private readonly Configuration.Json.TermBar config;
    private readonly Configuration.Json.Modules.Terminal moduleConfig;

    private TerminalViewModel? viewModel;

    /// <summary>
    /// The viewmodel.
    /// </summary>
    private TerminalViewModel? ViewModel {
      get => viewModel;
      set {
        viewModel = value;
        DataContext = viewModel;
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
    internal TerminalView(Configuration.Json.TermBar config, Configuration.Json.Modules.Terminal moduleConfig) : base(config, moduleConfig) {
      this.config = config;
      this.moduleConfig = moduleConfig;

      ViewModel = new TerminalViewModel(this, moduleConfig);

      InitializeComponent();
      ApplyComputedGridStyle();
      ApplyComputedTextBoxStyle(config);

      TerminalControl.WindowTitleChanged += TerminalControl_WindowTitleChanged;
      TerminalControl.VisualBellRinging += TerminalControl_VisualBellRinging;
    }

    /// <summary>
    /// Writes <paramref name="message"/> to the terminal.
    /// </summary>
    /// <param name="message">The message to write.</param>
    internal void Write(string message) => TerminalControl.Write(message);

    /// <summary>
    /// Invoked when the terminal window title changes.
    /// </summary>
    private void TerminalControl_WindowTitleChanged() => Debug.WriteLine($"Window title: {TerminalControl.WindowTitle}");

    /// <summary>
    /// Invoked when the terminal visual bell is ringing.
    /// </summary>
    private void TerminalControl_VisualBellRinging() {
      Debug.WriteLine("Visual bell ringing");
      TerminalControl.VisualBell = false;
    }

    /// <summary>
    /// Applies computed styles to <see cref="Grid"/>.
    /// </summary>
    private void ApplyComputedGridStyle() {
      Style gridStyle = new(typeof(Grid));

      StylesHelper.MergeWithAncestor(gridStyle, (Grid) Content, typeof(Grid));

      gridStyle.Setters.Add(new Setter(Grid.ColumnSpacingProperty, config.Padding));

      Resources[typeof(Grid)] = gridStyle;
    }

    /// <summary>
    /// Applies computed styles to <see cref="TextBox"/>.
    /// </summary>
    /// <param name="config"><inheritdoc cref="ModuleView.ModuleView"
    /// path="/param[@name='config']"/></param>
    private void ApplyComputedTextBoxStyle(Configuration.Json.TermBar config) {
      Style textBoxStyle = new(typeof(TextBox));

      StylesHelper.MergeWithAncestor(textBoxStyle, this, typeof(TextBox));

      textBoxStyle.Setters.Add(new Setter(TextBox.FontFamilyProperty, new FontFamily(config.FontFamily)));
      textBoxStyle.Setters.Add(new Setter(TextBox.FontSizeProperty, config.FontSize));
      textBoxStyle.Setters.Add(new Setter(TextBox.TextWrappingProperty, TextWrapping.Wrap));
      textBoxStyle.Setters.Add(new Setter(TextBox.IsSpellCheckEnabledProperty, false));

      textBoxStyle.Setters.Add(
        new Setter(
          TextBox.ForegroundProperty,
          PaletteHelper.Palette[config.Flavor].Colors[config.TextColor].SolidColorBrush
        )
      );

      Resources[typeof(TextBox)] = textBoxStyle;
    }
  }
}
