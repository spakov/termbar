using Microsoft.UI.Xaml.Media;
using Spakov.Catppuccin;
using Spakov.TermBar.Views.Modules.SystemDropdown;
using System.Collections.ObjectModel;

namespace Spakov.TermBar.ViewModels.Modules.SystemDropdown {
  internal class SystemDropdownViewModel {
    private readonly Configuration.Json.TermBar config;
    private readonly Configuration.Json.Modules.SystemDropdown moduleConfig;

    private readonly ObservableCollection<SystemDropdownMenuFlyoutItemView> views;

    /// <summary>
    /// The dropdown icon.
    /// </summary>
    internal string? Icon => moduleConfig.Icon;

    /// <summary>
    /// The dropdown icon color.
    /// </summary>
    internal SolidColorBrush? IconColor => Palette.Instance[config.Flavor].Colors[moduleConfig.AccentColor].SolidColorBrush;

    /// <summary>
    /// The list of <see cref="SystemDropdownMenuFlyoutItemView"/>s to be
    /// presented to the user.
    /// </summary>
    internal ObservableCollection<SystemDropdownMenuFlyoutItemView> Items => views;

    /// <summary>
    /// Initializes a <see cref="SystemDropdownViewModel"/>.
    /// </summary>
    /// <param name="config"><inheritdoc
    /// cref="SystemDropdownView.SystemDropdownView"
    /// path="/param[@name='config']"/></param>
    /// <param name="moduleConfig"><inheritdoc
    /// cref="SystemDropdownView.SystemDropdownView"
    /// path="/param[@name='moduleConfig']"/></param>
    internal SystemDropdownViewModel(Configuration.Json.TermBar config, Configuration.Json.Modules.SystemDropdown moduleConfig) {
      this.config = config;
      this.moduleConfig = moduleConfig;

      views = [];

      foreach (Configuration.Json.Modules.SystemDropdown.SystemDropdownFeatures feature in moduleConfig.Features) {
        views.Add(new(config, moduleConfig, feature));
      }
    }
  }
}