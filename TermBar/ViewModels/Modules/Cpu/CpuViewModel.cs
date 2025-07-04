using Microsoft.UI.Xaml;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TermBar.Configuration.Json.Modules;
using TermBar.Models;

namespace TermBar.ViewModels.Modules {
  /// <summary>
  /// The CPU usage monitor viewmodel.
  /// </summary>
  internal partial class CpuViewModel : INotifyPropertyChanged {
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly Cpu config;

    private readonly DispatcherTimer dispatcherTimer;

    private string? _icon;
    private string? _cpu;

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
    public CpuViewModel(Cpu config) {
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
    private void Tick(object? sender, object e) => Cpu = string.Format(config.Format, config.Round ? Math.Round(Performance.CpuPercent, 0, MidpointRounding.AwayFromZero) : Performance.CpuPercent);

    private void OnPropertyChanged([CallerMemberName] string? callerMemberName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerMemberName));
  }
}
