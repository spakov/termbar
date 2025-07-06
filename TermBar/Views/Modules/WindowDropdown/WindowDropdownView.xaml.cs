using System;
using TermBar.ViewModels.Modules.WindowDropdown;

namespace TermBar.Views.Modules.WindowDropdown {
  /// <summary>
  /// The TermBar window dropdown.
  /// </summary>
  internal sealed partial class WindowDropdownView : ModuleView {
    private WindowDropdownViewModel? viewModel;

    /// <summary>
    /// The viewmodel.
    /// </summary>
    private WindowDropdownViewModel? ViewModel {
      get => viewModel;
      set {
        viewModel = value;
        DataContext = viewModel;
      }
    }

    /// <summary>
    /// <inheritdoc cref="ModuleView.ModuleView"
    /// path="/param[@name='config']"/>
    /// </summary>
    private Configuration.Json.TermBar? Config { get; init; }

    /// <summary>
    /// The <see cref="Action{T}"/> to invoke when the user clicks a menu item.
    /// </summary>
    private Action<WindowDropdownMenuFlyoutItemView>? ClickAction { get; init; }

    /// <summary>
    /// Initializes a <see cref="WindowDropdownView"/>.
    /// </summary>
    /// <param name="config"><inheritdoc cref="Config"
    /// path="/summary"/></param>
    /// <param name="moduleConfig">The <see
    /// cref="Configuration.Json.Modules.WindowBar"/> for this <see
    /// cref="WindowBarView"/>.</param>
    internal WindowDropdownView(Configuration.Json.TermBar config, Configuration.Json.Modules.WindowDropdown moduleConfig) : base(config, moduleConfig, skipColor: true) {
      Config = config;
      ClickAction = ItemClicked;
      ViewModel = new WindowDropdownViewModel(config, moduleConfig);

      InitializeComponent();
    }

    /// <summary>
    /// Invoked when the user clicks a menu item.
    /// </summary>
    /// <param name="window">The window that was clicked.</param>
    private void ItemClicked(WindowDropdownMenuFlyoutItemView window) {
      if (ViewModel is not null) ViewModel.ForegroundedWindow = window;
    }
  }
}
