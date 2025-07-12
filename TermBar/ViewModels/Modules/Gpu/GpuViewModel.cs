using Microsoft.UI.Xaml;
using Spakov.TermBar.Models;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Spakov.TermBar.ViewModels.Modules.Gpu {
  /// <summary>
  /// The GPU usage monitor viewmodel.
  /// </summary>
  internal partial class GpuViewModel : INotifyPropertyChanged {
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly Configuration.Json.Modules.Gpu config;

    private readonly DispatcherTimer dispatcherTimer;

    private string? _icon;
    private string? _gpu;

    /// <summary>
    /// The GPU icon.
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
    /// The GPU usage.
    /// </summary>
    public string? Gpu {
      get => _gpu;

      set {
        if (_gpu != value) {
          _gpu = value;
          OnPropertyChanged();
        }
      }
    }

    /// <summary>
    /// Initializes a <see cref="GpuViewModel"/>.
    /// </summary>
    public GpuViewModel(Configuration.Json.Modules.Gpu config) {
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
    /// Updates the GPU usage.
    /// </summary>
    /// <param name="sender"><inheritdoc cref="EventHandler"
    /// path="/param[@name='sender']"/></param>
    /// <param name="e"><inheritdoc cref="EventHandler"
    /// path="/param[@name='e']"/></param>
    private void Tick(object? sender, object e) => Gpu = string.Format(config.Format, config.Round ? Math.Round(Performance.GpuPercent, 0, MidpointRounding.AwayFromZero) : Performance.GpuPercent);

    private void OnPropertyChanged([CallerMemberName] string? callerMemberName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerMemberName));
  }
}