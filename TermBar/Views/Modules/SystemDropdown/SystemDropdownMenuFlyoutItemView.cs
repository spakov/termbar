using Microsoft.UI.Xaml.Controls;
using Spakov.TermBar.ViewModels.Modules.SystemDropdown;

namespace Spakov.TermBar.Views.Modules.SystemDropdown {
  internal sealed partial class SystemDropdownMenuFlyoutItemView : MenuFlyoutItem {
    private SystemDropdownItemViewModel? viewModel;

    private SystemDropdownItemViewModel? ViewModel {
      get => viewModel;

      set {
        viewModel = value;
        DataContext = viewModel;
      }
    }

    /// <summary>
    /// The item's <see
    /// cref="Configuration.Json.Modules.SystemDropdown.SystemDropdownFeatures"
    /// />.
    /// </summary>
    internal Configuration.Json.Modules.SystemDropdown.SystemDropdownFeatures Feature { get; private set; }

    /// <summary>
    /// Initializes a <see cref="SystemDropdownMenuFlyoutItemView"/>.
    /// </summary>
    /// <param name="config"><inheritdoc cref="ModuleView.ModuleView"
    /// path="/param[@name='config']"/></param>
    /// <param name="moduleConfig">The <see
    /// cref="Configuration.Json.Modules.SystemDropdown"/> for this
    /// <see cref="SystemDropdownMenuFlyoutItemView"/>.</param>
    /// <param name="feature"><inheritdoc cref="Feature"
    /// path="/summary"/></param>
    internal SystemDropdownMenuFlyoutItemView(Configuration.Json.TermBar config, Configuration.Json.Modules.SystemDropdown moduleConfig, Configuration.Json.Modules.SystemDropdown.SystemDropdownFeatures feature) {
      Feature = feature;

      ViewModel = new SystemDropdownItemViewModel(config, moduleConfig, feature);
    }
  }
}