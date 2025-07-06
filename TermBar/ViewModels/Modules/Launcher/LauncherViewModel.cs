using System.Collections.Generic;
using TermBar.Catppuccin;
using TermBar.Configuration.Json.Modules;

namespace TermBar.ViewModels.Modules.Launcher {
  /// <summary>
  /// The launcher viewmodel.
  /// </summary>
  internal partial class LauncherViewModel {
    private readonly List<LauncherLauncherEntryViewModel> _launcherEntries;

    /// <summary>
    /// Launcher entries.
    /// </summary>
    public List<LauncherLauncherEntryViewModel> LauncherEntries => _launcherEntries;

    /// <summary>
    /// Initializes a <see cref="LauncherViewModel"/>.
    /// </summary>
    public LauncherViewModel(Configuration.Json.TermBar config, Configuration.Json.Modules.Launcher moduleConfig) {
      _launcherEntries = [];

      foreach (LauncherEntry launcherEntry in moduleConfig.LauncherEntries) {
        _launcherEntries.Add(
          new() {
            Name = launcherEntry.Name,
            Command = (launcherEntry.Command, launcherEntry.CommandArguments),
            DisplayName = launcherEntry.DisplayName,
            Icon = launcherEntry.Icon,
            IconColor = launcherEntry.IconColor is not null
              ? PaletteHelper.Palette[config.Flavor].Colors[(ColorEnum) launcherEntry.IconColor].SolidColorBrush
              : PaletteHelper.Palette[config.Flavor].Colors[moduleConfig.AccentColor].SolidColorBrush
          }
        );
      }
    }
  }
}
