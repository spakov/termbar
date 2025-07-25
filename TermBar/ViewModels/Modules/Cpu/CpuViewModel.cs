using Microsoft.UI.Xaml;
using Spakov.TermBar.Models;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Spakov.TermBar.ViewModels.Modules.Cpu
{
    /// <summary>
    /// The CPU usage monitor viewmodel.
    /// </summary>
    internal partial class CpuViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly Configuration.Json.Modules.Cpu _config;

        private readonly DispatcherTimer _dispatcherTimer;

        private string? _icon;
        private string? _cpu;

        /// <summary>
        /// The CPU icon.
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
        /// The CPU usage.
        /// </summary>
        public string? Cpu
        {
            get => _cpu;

            set
            {
                if (_cpu != value)
                {
                    _cpu = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Initializes a <see cref="CpuViewModel"/>.
        /// </summary>
        /// <param name="config">A <see cref="Configuration.Json.Modules.Cpu"/>
        /// configuration.</param>
        public CpuViewModel(Configuration.Json.Modules.Cpu config)
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
        /// Updates the CPU usage.
        /// </summary>
        /// <param name="sender"><inheritdoc cref="EventHandler"
        /// path="/param[@name='sender']"/></param>
        /// <param name="e"><inheritdoc cref="EventHandler"
        /// path="/param[@name='e']"/></param>
        private void Tick(object? sender, object e) => Cpu = string.Format(_config.Format, _config.Round ? Math.Round(Performance.CpuPercent is not null ? (float)Performance.CpuPercent : -1.0f, 0, MidpointRounding.AwayFromZero) : Performance.CpuPercent);

        private void OnPropertyChanged([CallerMemberName] string? callerMemberName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerMemberName));
    }
}