using Microsoft.UI.Xaml;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Spakov.TermBar.ViewModels.Modules.Clock
{
    /// <summary>
    /// The clock viewmodel.
    /// </summary>
    internal partial class ClockViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly Configuration.Json.Modules.Clock _config;

        private readonly DispatcherTimer _dispatcherTimer;

        private string? _icon;
        private string? _time;

        /// <summary>
        /// The clock icon.
        /// </summary>
        public string? Icon
        {
            get => _icon;

            set
            {
                if (_icon != value)
                {
                    _icon = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The time.
        /// </summary>
        public string? Time
        {
            get => _time;

            set
            {
                if (_time != value)
                {
                    _time = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Initializes a <see cref="ClockViewModel"/>.
        /// </summary>
        /// <param name="config">A <see
        /// cref="Configuration.Json.Modules.Clock"/> configuration.</param>
        public ClockViewModel(Configuration.Json.Modules.Clock config)
        {
            _config = config;

            Icon = config.Icon;

            _dispatcherTimer = new()
            {
                Interval = TimeSpan.FromMilliseconds(config.UpdateInterval)
            };

            _dispatcherTimer.Tick += Tick;
            _dispatcherTimer.Start();

            Tick(this, new());
        }

        /// <summary>
        /// Updates the time.
        /// </summary>
        /// <param name="sender"><inheritdoc cref="EventHandler"
        /// path="/param[@name='sender']"/></param>
        /// <param name="e"><inheritdoc cref="EventHandler"
        /// path="/param[@name='e']"/></param>
        private void Tick(object? sender, object e) => Time = DateTime.Now.ToString(_config.TimeFormat);

        private void OnPropertyChanged([CallerMemberName] string? callerMemberName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerMemberName));
    }
}