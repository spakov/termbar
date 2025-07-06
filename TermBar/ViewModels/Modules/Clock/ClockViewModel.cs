using Microsoft.UI.Xaml;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TermBar.ViewModels.Modules.Clock {
  /// <summary>
  /// The clock viewmodel.
  /// </summary>
  internal partial class ClockViewModel : INotifyPropertyChanged {
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly Configuration.Json.Modules.Clock config;

    private readonly DispatcherTimer dispatcherTimer;

    private string? _icon;
    private string? _time;

    /// <summary>
    /// The clock icon.
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
    /// The time.
    /// </summary>
    public string? Time {
      get => _time;

      set {
        if (_time != value) {
          _time = value;
          OnPropertyChanged();
        }
      }
    }

    /// <summary>
    /// Initializes a <see cref="ClockViewModel"/>.
    /// </summary>
    public ClockViewModel(Configuration.Json.Modules.Clock config) {
      this.config = config;

      Icon = config.Icon;

      dispatcherTimer = new() {
        Interval = TimeSpan.FromMilliseconds(config.UpdateInterval)
      };

      dispatcherTimer.Tick += Tick;
      dispatcherTimer.Start();

      Tick(this, new());
    }

    /// <summary>
    /// Updates the time.
    /// </summary>
    /// <param name="sender"><inheritdoc cref="EventHandler"
    /// path="/param[@name='sender']"/></param>
    /// <param name="e"><inheritdoc cref="EventHandler"
    /// path="/param[@name='e']"/></param>
    private void Tick(object? sender, object e) => Time = DateTime.Now.ToString(config.TimeFormat);

    private void OnPropertyChanged([CallerMemberName] string? callerMemberName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerMemberName));
  }
}
