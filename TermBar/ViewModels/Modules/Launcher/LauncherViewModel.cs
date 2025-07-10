using Spakov.Catppuccin;
using Spakov.TermBar.Configuration.Json;
using Spakov.TermBar.Configuration.Json.Modules;
using System.Collections.Generic;

namespace Spakov.TermBar.ViewModels.Modules.Launcher {
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
        LauncherLauncherEntryViewModel launcherLauncherEntryViewModel = new() {
          Name = launcherEntry.Name,
          Command = (launcherEntry.Command, launcherEntry.CommandArguments),
          DisplayName = launcherEntry.DisplayName
        };

        if (launcherEntry.Icon is null) {
          if (config.WindowList.ProcessIconMap is not null && launcherEntry.Command is not null) {
            if (config.WindowList.ProcessIconMap.TryGetValue(launcherEntry.Command, out ProcessIconMapping? processIconMapping)) {
              launcherLauncherEntryViewModel.Icon = processIconMapping.Icon;
            }
          }

          launcherLauncherEntryViewModel.Icon ??= moduleConfig.Icon;
        }

        launcherLauncherEntryViewModel.Icon ??= launcherEntry.Icon;

        if (launcherEntry.IconColor is null) {
          if (config.WindowList.ProcessIconMap is not null && launcherEntry.Command is not null) {
            if (config.WindowList.ProcessIconMap.TryGetValue(launcherEntry.Command, out ProcessIconMapping? processIconMapping)) {
              if (processIconMapping.IconColor is not null) {
                launcherLauncherEntryViewModel.IconColor = Palette.Instance[config.Flavor].Colors[(ColorEnum) processIconMapping.IconColor].SolidColorBrush;
              }
            }
          }
        }

        if (launcherEntry.IconColor is not null) {
          launcherLauncherEntryViewModel.IconColor ??= Palette.Instance[config.Flavor].Colors[(ColorEnum) launcherEntry.IconColor].SolidColorBrush;
        }

        launcherLauncherEntryViewModel.IconColor ??= Palette.Instance[config.Flavor].Colors[moduleConfig.AccentColor].SolidColorBrush;

        _launcherEntries.Add(launcherLauncherEntryViewModel);
      }
    }
  }
}