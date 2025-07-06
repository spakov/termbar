using Microsoft.UI.Xaml;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TermBar.Models;

namespace TermBar.ViewModels.Modules.Memory {
  /// <summary>
  /// The memory usage monitor viewmodel.
  /// </summary>
  internal partial class MemoryViewModel : INotifyPropertyChanged {
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly Configuration.Json.Modules.Memory config;

    private readonly DispatcherTimer dispatcherTimer;

    private string? _icon;
    private string? _memory;

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
    /// The memory usage.
    /// </summary>
    public string? Memory {
      get => _memory;

      set {
        if (_memory != value) {
          _memory = value;
          OnPropertyChanged();
        }
      }
    }

    /// <summary>
    /// Initializes a <see cref="MemoryViewModel"/>.
    /// </summary>
    public MemoryViewModel(Configuration.Json.Modules.Memory config) {
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
    /// Updates the memory usage.
    /// </summary>
    /// <param name="sender"><inheritdoc cref="EventHandler"
    /// path="/param[@name='sender']"/></param>
    /// <param name="e"><inheritdoc cref="EventHandler"
    /// path="/param[@name='e']"/></param>
    private void Tick(object? sender, object e) => Memory = string.Format(config.Format, config.Round ? Math.Round(Performance.MemoryPercent, 0, MidpointRounding.AwayFromZero) : Performance.MemoryPercent);

    private void OnPropertyChanged([CallerMemberName] string? callerMemberName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerMemberName));
  }
}
