#if DEBUG
using Microsoft.Extensions.Logging;
#endif
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TermBar.ViewModels.Modules.Volume {
  /// <summary>
  /// The volume viewmodel.
  /// </summary>
  internal partial class VolumeViewModel : INotifyPropertyChanged {
#if DEBUG
    internal readonly ILogger logger;
    internal static readonly LogLevel logLevel = App.logLevel;
#endif

    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly Configuration.Json.Modules.Volume config;

    private string? _icon;
    private string? _volume;

    /// <summary>
    /// The volume icon.
    /// </summary>
    public string? Icon {
      get => _icon;

      set {
        if (_icon != value) {
          _icon = value;

#if DEBUG
          logger.LogDebug("Icon changed => {icon}", _icon);
#endif

          OnPropertyChanged();
        }
      }
    }

    /// <summary>
    /// The volume level.
    /// </summary>
    public string? Volume {
      get => _volume;

      set {
        if (_volume != value) {
          _volume = value;

#if DEBUG
          logger.LogDebug("Volume changed => {volume}", _volume);
#endif

          OnPropertyChanged();
        }
      }
    }

    /// <summary>
    /// The volume percent.
    /// </summary>
    public static float VolumePercent {
      get => Models.Volume.Instance!.VolumePercent;
      set => Models.Volume.Instance!.VolumePercent = value;
    }

    /// <summary>
    /// Whether the volume is muted.
    /// </summary>
    public static bool VolumeMuted {
      get => Models.Volume.Instance!.VolumeMuted;
      set => Models.Volume.Instance!.VolumeMuted = value;
    }

    /// <summary>
    /// Initializes a <see cref="VolumeViewModel"/>.
    /// </summary>
    public VolumeViewModel(Configuration.Json.Modules.Volume config) {
#if DEBUG
      using ILoggerFactory factory = LoggerFactory.Create(
        builder => {
          builder.AddDebug();
          builder.SetMinimumLevel(logLevel);
        }
      );

      logger = factory.CreateLogger<VolumeViewModel>();
#endif

      this.config = config;

      Models.Volume.Instance!.VolumeChanged += Volume_VolumeChanged;
      Volume_VolumeChanged();
    }

    /// <summary>
    /// Called when the volume changes.
    /// </summary>
    private void Volume_VolumeChanged() {
      Icon = Models.Volume.Instance!.VolumeMuted ? config.MutedIcon : config.Icon;

      if (config.Muted is not null) {
        if (Models.Volume.Instance!.VolumeMuted) {
          Volume = config.Muted;

          return;
        }
      }

      Volume = string.Format(config.Format, Math.Round(Models.Volume.Instance!.VolumePercent, 0, MidpointRounding.AwayFromZero));
    }

    private void OnPropertyChanged([CallerMemberName] string? callerMemberName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerMemberName));
  }
}
