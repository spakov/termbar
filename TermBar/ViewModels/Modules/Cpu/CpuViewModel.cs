using Microsoft.UI.Xaml;
using Spakov.TermBar.Models;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Spakov.TermBar.ViewModels.Modules.Cpu {
  /// <summary>
  /// The CPU usage monitor viewmodel.
  /// </summary>
  internal partial class CpuViewModel : INotifyPropertyChanged {
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly Configuration.Json.Modules.Cpu config;

    private readonly DispatcherTimer dispatcherTimer;

    private string? _icon;
    private string? _cpu;

    /// <summary>
    /// The CPU icon.
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
    /// The CPU usage.
    /// </summary>
    public string? Cpu {
      get => _cpu;

      set {
        if (_cpu != value) {
          _cpu = value;
          OnPropertyChanged();
        }
      }
    }

    /// <summary>
    /// Initializes a <see cref="CpuViewModel"/>.
    /// </summary>
    public CpuViewModel(Configuration.Json.Modules.Cpu config) {
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
    /// Updates the CPU usage.
    /// </summary>
    /// <param name="sender"><inheritdoc cref="EventHandler"
    /// path="/param[@name='sender']"/></param>
    /// <param name="e"><inheritdoc cref="EventHandler"
    /// path="/param[@name='e']"/></param>
    private void Tick(object? sender, object e) => Cpu = string.Format(config.Format, config.Round ? Math.Round(Performance.CpuPercent is not null ? (float) Performance.CpuPercent : -1.0f, 0, MidpointRounding.AwayFromZero) : Performance.CpuPercent);

    private void OnPropertyChanged([CallerMemberName] string? callerMemberName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerMemberName));
  }
}