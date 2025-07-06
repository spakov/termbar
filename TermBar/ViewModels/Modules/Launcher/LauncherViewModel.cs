using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TermBar.Catppuccin;
using TermBar.Configuration.Json.Modules;

namespace TermBar.ViewModels.Modules.Launcher {
  /// <summary>
  /// The launcher viewmodel.
  /// </summary>
  internal partial class LauncherViewModel : INotifyPropertyChanged {
    public event PropertyChangedEventHandler? PropertyChanged;

    private string? _icon;
    private readonly List<LauncherLauncherEntryViewModel> _launcherEntries;

    /// <summary>
    /// The launcher icon.
    /// </summary>
    public string? Icon {
      get => _icon;

      set {
        if (_icon != value) {
          _icon = value;
          OnPropertyChanged();
        }
      }
    }

    /// <summary>
    /// Launcher entries.
    /// </summary>
    public List<LauncherLauncherEntryViewModel> LauncherEntries => _launcherEntries;

    /// <summary>
    /// Initializes a <see cref="LauncherViewModel"/>.
    /// </summary>
    public LauncherViewModel(Configuration.Json.TermBar config, Configuration.Json.Modules.Launcher moduleConfig) {
      Icon = moduleConfig.Icon;

      _launcherEntries = [];

      foreach (LauncherEntry launcherEntry in moduleConfig.LauncherEntries) {
        _launcherEntries.Add(
          new() {
            Name = launcherEntry.Name,
            Command = launcherEntry.Command,
            DisplayName = launcherEntry.DisplayName,
            Icon = launcherEntry.Icon,
            IconColor = launcherEntry.IconColor is not null
              ? PaletteHelper.Palette[config.Flavor].Colors[(ColorEnum) launcherEntry.IconColor].SolidColorBrush
              : PaletteHelper.Palette[config.Flavor].Colors[moduleConfig.AccentColor].SolidColorBrush
          }
        );
      }
    }

    private void OnPropertyChanged([CallerMemberName] string? callerMemberName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerMemberName));
  }
}
