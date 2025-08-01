﻿using Microsoft.UI.Dispatching;
using System;
using System.Runtime.InteropServices;

namespace Spakov.TermBar.Models
{
    /// <summary>
    /// The volume model.
    /// </summary>
    /// <remarks>Interfaces with <c>EndpointVolumeInterop</c> to allow managing
    /// the system default audio device volume, something that can only be done
    /// via COM.</remarks>
    internal partial class Volume : IDisposable
    {
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

        private readonly VolumeChangedCallback _volumeChangedCallback;
        private readonly bool _isInitialized;

        /// <summary>
        /// The singleton instance.
        /// </summary>
        /// <remarks><b>Important</b>: since we're dealing with COM, this must
        /// be set on the UI thread!</remarks>
        public static Volume? Instance { get; set; }

        private readonly DispatcherQueue _dispatcherQueue;

        /// <summary>
        /// The volume percent.
        /// </summary>
        /// <exception cref="COMException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        internal float VolumePercent
        {
            get
            {
                if (!_isInitialized)
                {
                    throw new InvalidOperationException(App.ResourceLoader.GetString("MustCallInitializeAudioMonitor"));
                }

                int hr = GetMasterVolume(out float level);

                return hr == 0
                    ? level * 100
                    : throw new COMException(App.ResourceLoader.GetString("FailedToGetVolumeLevel"), Marshal.GetHRForLastWin32Error());
            }

            set
            {
                if (!_isInitialized)
                {
                    throw new InvalidOperationException(App.ResourceLoader.GetString("MustCallInitializeAudioMonitor"));
                }

                int hr = SetMasterVolume(value / 100);

                if (hr != 0)
                {
                    throw new COMException(App.ResourceLoader.GetString("FailedToSetVolumeLevel"), hr);
                }
            }
        }

        /// <summary>
        /// Whether the volume is muted.
        /// </summary>
        /// <exception cref="COMException"></exception>
        internal bool VolumeMuted
        {
            get
            {
                if (!_isInitialized)
                {
                    throw new InvalidOperationException(App.ResourceLoader.GetString("MustCallInitializeAudioMonitor"));
                }

                int hr = GetMute(out bool mute);

                return hr == 0
                    ? mute
                    : throw new COMException(App.ResourceLoader.GetString("FailedToGetMuteState"), Marshal.GetHRForLastWin32Error());
            }

            set
            {
                if (!_isInitialized)
                {
                    throw new InvalidOperationException(App.ResourceLoader.GetString("MustCallInitializeAudioMonitor"));
                }

                int hr = SetMute(value);

                if (hr != 0)
                {
                    throw new COMException(App.ResourceLoader.GetString("FailedToSetMuteState"), hr);
                }
            }
        }

        /// <summary>
        /// Invoked when the volume changes.
        /// </summary>
        /// <param name="_">Unused.</param>
        internal void OnVolumeChanged(float _) => _dispatcherQueue.TryEnqueue(() => VolumeChanged?.Invoke());

        /// <summary>
        /// Initializes a <see cref="Volume"/>.
        /// </summary>
        /// <exception cref="COMException"></exception>
        internal Volume(DispatcherQueue dispatcherQueue)
        {
            _dispatcherQueue = dispatcherQueue;

            int hr = InitializeAudioMonitor();

            if (hr != 0)
            {
                throw new COMException(App.ResourceLoader.GetString("FailedToInitializeAudioMonitor"), hr);
            }

            _volumeChangedCallback = OnVolumeChanged;
            SetVolumeChangedCallback(Marshal.GetFunctionPointerForDelegate(_volumeChangedCallback));

            _isInitialized = true;
        }

        public void Dispose() => CleanupAudioMonitor();
    }
}