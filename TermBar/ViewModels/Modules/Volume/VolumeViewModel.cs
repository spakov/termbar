using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Spakov.TermBar.ViewModels.Modules.Volume
{
    /// <summary>
    /// The volume viewmodel.
    /// </summary>
    internal partial class VolumeViewModel : INotifyPropertyChanged
    {
        private readonly ILogger? _logger;

        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly Configuration.Json.Modules.Volume _config;

        private string? _icon;
        private string? _volume;

        /// <summary>
        /// The volume icon.
        /// </summary>
        public string? Icon
        {
            get => _icon;

            set
            {
                if (_icon != value)
                {
                    _icon = value;
                    _logger?.LogTrace("Icon changed => {icon}", _icon);

                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The volume level.
        /// </summary>
        public string? Volume
        {
            get => _volume;

            set
            {
                if (_volume != value)
                {
                    _volume = value;
                    _logger?.LogTrace("Volume changed => {volume}", _volume);

                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The volume percent.
        /// </summary>
        public static float VolumePercent
        {
            get => Models.Volume.Instance!.VolumePercent;
            set => Models.Volume.Instance!.VolumePercent = value;
        }

        /// <summary>
        /// Whether the volume is muted.
        /// </summary>
        public static bool VolumeMuted
        {
            get => Models.Volume.Instance!.VolumeMuted;
            set => Models.Volume.Instance!.VolumeMuted = value;
        }

        /// <summary>
        /// Initializes a <see cref="VolumeViewModel"/>.
        /// </summary>
        /// <param name="config">A <see
        /// cref="Configuration.Json.Modules.Volume"/>
        /// configuration.</param>
        public VolumeViewModel(Configuration.Json.Modules.Volume config)
        {
            _logger = LoggerHelper.CreateLogger<VolumeViewModel>();

            _config = config;

            Models.Volume.Instance!.VolumeChanged += Volume_VolumeChanged;
            Volume_VolumeChanged();
        }

        /// <summary>
        /// Updates the volume level and mute state presentation.
        /// </summary>
        private void Volume_VolumeChanged()
        {
            Icon = Models.Volume.Instance!.VolumeMuted ? _config.MutedIcon : _config.Icon;

            if (_config.Muted is not null)
            {
                if (Models.Volume.Instance!.VolumeMuted)
                {
                    Volume = _config.Muted;

                    return;
                }
            }

            Volume = string.Format(_config.Format, Math.Round(Models.Volume.Instance!.VolumePercent, 0, MidpointRounding.AwayFromZero));
        }

        private void OnPropertyChanged([CallerMemberName] string? callerMemberName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerMemberName));
    }
}