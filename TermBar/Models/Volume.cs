using Microsoft.UI.Dispatching;
using System;
using System.Runtime.InteropServices;

namespace Spakov.TermBar.Models {
  /// <summary>
  /// The volume model.
  /// </summary>
  internal partial class Volume : IDisposable {
    internal event Action? VolumeChanged;

    [LibraryImport("EndpointVolumeInterop.dll")]
    private static partial int InitializeAudioMonitor();

    [LibraryImport("EndpointVolumeInterop.dll")]
    private static partial void SetVolumeChangedCallback(IntPtr cb);

    [LibraryImport("EndpointVolumeInterop.dll")]
    private static partial void CleanupAudioMonitor();

    [LibraryImport("EndpointVolumeInterop.dll")]
    private static partial int SetMasterVolume(float level);

    [LibraryImport("EndpointVolumeInterop.dll")]
    private static partial int GetMasterVolume(out float level);

    [LibraryImport("EndpointVolumeInterop.dll")]
    private static partial int SetMute([MarshalAs(UnmanagedType.Bool)] bool mute);

    [LibraryImport("EndpointVolumeInterop.dll")]
    private static partial int GetMute([MarshalAs(UnmanagedType.Bool)] out bool mute);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VolumeChangedCallback(float newVolume);

    private readonly VolumeChangedCallback volumeChangedCallback;
    private readonly bool isInitialized;

    /// <summary>
    /// The singleton instance.
    /// </summary>
    /// <remarks><b>Important</b>: since we're dealing with COM, this must be
    /// set on the UI thread!</remarks>
    public static Volume? Instance { get; set; }

    private readonly DispatcherQueue dispatcherQueue;

    /// <summary>
    /// The volume percent.
    /// </summary>
    /// <exception cref="COMException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    internal float VolumePercent {
      get {
        if (!isInitialized) {
          throw new InvalidOperationException("Must call InitializeAudioMonitor() first.");
        }

        int hr = GetMasterVolume(out float level);

        return hr == 0
          ? level * 100
          : throw new COMException("Failed to get volume level.", Marshal.GetHRForLastWin32Error());
      }

      set {
        if (!isInitialized) {
          throw new InvalidOperationException("Must call InitializeAudioMonitor() first.");
        }

        int hr = SetMasterVolume(value / 100);

        if (hr != 0) {
          throw new COMException("Failed to set volume level.", hr);
        }
      }
    }

    /// <summary>
    /// Whether the volume is muted.
    /// </summary>
    /// <exception cref="COMException"></exception>
    internal bool VolumeMuted {
      get {
        if (!isInitialized) {
          throw new InvalidOperationException("Must call InitializeAudioMonitor() first.");
        }

        int hr = GetMute(out bool mute);

        return hr == 0
          ? mute
          : throw new COMException("Failed to get volume level.", Marshal.GetHRForLastWin32Error());
      }

      set {
        if (!isInitialized) {
          throw new InvalidOperationException("Must call InitializeAudioMonitor() first.");
        }

        int hr = SetMute(value);

        if (hr != 0) {
          throw new COMException("Failed to set mute state.", hr);
        }
      }
    }

    /// <summary>
    /// Invoked when the volume changes.
    /// </summary>
    /// <param name="_">Unused.</param>
    internal void OnVolumeChanged(float _) => dispatcherQueue.TryEnqueue(() => VolumeChanged?.Invoke());

    /// <summary>
    /// Initializes a <see cref="Volume"/>.
    /// </summary>
    /// <exception cref="COMException"></exception>
    internal Volume(DispatcherQueue dispatcherQueue) {
      this.dispatcherQueue = dispatcherQueue;

      int hr = InitializeAudioMonitor();

      if (hr != 0) {
        throw new COMException("Failed to initialize audio monitor.", hr);
      }

      volumeChangedCallback = OnVolumeChanged;
      SetVolumeChangedCallback(Marshal.GetFunctionPointerForDelegate(volumeChangedCallback));

      isInitialized = true;
    }

    public void Dispose() => CleanupAudioMonitor();
  }
}