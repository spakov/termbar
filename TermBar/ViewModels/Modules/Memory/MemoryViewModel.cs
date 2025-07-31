using Microsoft.UI.Xaml;
using Spakov.TermBar.Models;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Spakov.TermBar.ViewModels.Modules.Memory
{
    /// <summary>
    /// The memory usage monitor viewmodel.
    /// </summary>
    internal partial class MemoryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly Configuration.Json.Modules.Memory _config;

        private readonly DispatcherTimer _dispatcherTimer;

        private string? _icon;
        private string? _memory;

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
        /// The memory usage.
        /// </summary>
        public string? Memory
        {
            get => _memory;

            set
            {
                if (_memory != value)
                {
                    _memory = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Initializes a <see cref="MemoryViewModel"/>.
        /// </summary>
        /// <param name="config">A <see
        /// cref="Configuration.Json.Modules.Memory"/> configuration.</param>
        public MemoryViewModel(Configuration.Json.Modules.Memory config)
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
        /// Updates the memory usage.
        /// </summary>
        /// <param name="sender"><inheritdoc cref="EventHandler"
        /// path="/param[@name='sender']"/></param>
        /// <param name="e"><inheritdoc cref="EventHandler"
        /// path="/param[@name='e']"/></param>
        private void Tick(object? sender, object e)
        {
            float? memoryPercent = Performance.MemoryPercent;

            Memory = memoryPercent is not null
                ? string.Format(
                    _config.Format,
                    _config.Round
                        ? Math.Round(
                            (float)memoryPercent,
                            0,
                            MidpointRounding.AwayFromZero
                        )
                        : memoryPercent
                )
                : Performance.ErrorIcon;
        }

        private void OnPropertyChanged([CallerMemberName] string? callerMemberName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerMemberName));
    }
}