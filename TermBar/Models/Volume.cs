using Microsoft.UI.Dispatching;
using NAudio.CoreAudioApi;
using System;

namespace TermBar.Models {
  /// <summary>
  /// The volume model.
  /// </summary>
  internal partial class Volume : IDisposable {
    internal event Action? VolumeChanged;

    /// <summary>
    /// The singleton instance.
    /// </summary>
    /// <remarks><b>Important</b>: since we're dealing with COM, this must be
    /// set on the UI thread!</remarks>
    public static Volume? Instance { get; set; }

    private readonly DispatcherQueue dispatcherQueue;

    private readonly MMDeviceEnumerator enumerator;
    private readonly MMDevice defaultDevice;
    private readonly AudioEndpointVolume volume;

    /// <summary>
    /// The volume percent.
    /// </summary>
    internal float VolumePercent {
      get => volume.MasterVolumeLevelScalar * 100;

      set {
        if (Math.Abs((volume.MasterVolumeLevelScalar * 100) - value) > 1) {
          volume.MasterVolumeLevelScalar = value / 100;
        }
      }
    }

    /// <summary>
    /// Whether the volume is muted.
    /// </summary>
    internal bool VolumeMuted {
      get => volume.Mute;

      set {
        if (volume.Mute != value) {
          volume.Mute = value;
        }
      }
    }

    /// <summary>
    /// Invoked when the volume changes.
    /// </summary>
    /// <param name="_">Unused.</param>
    internal void OnVolumeChanged(AudioVolumeNotificationData _) => dispatcherQueue.TryEnqueue(() => VolumeChanged?.Invoke());

    /// <summary>
    /// Initializes a <see cref="Volume"/>.
    /// </summary>
    internal Volume(DispatcherQueue dispatcherQueue) {
      this.dispatcherQueue = dispatcherQueue;

      enumerator = new();
      defaultDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
      volume = defaultDevice.AudioEndpointVolume;
      volume.OnVolumeNotification += OnVolumeChanged;
    }

    public void Dispose() {
      volume.OnVolumeNotification -= OnVolumeChanged;
      volume.Dispose();
      defaultDevice.Dispose();
      enumerator.Dispose();
    }
  }
}
