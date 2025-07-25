using Microsoft.UI.Xaml.Media;
using Spakov.Catppuccin;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Spakov.TermBar.ViewModels.Modules.WindowDropdown
{
    /// <summary>
    /// The window dropdown window viewmodel.
    /// </summary>
    internal partial class WindowDropdownWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly Configuration.Json.TermBar _config;
        private readonly Process? _process;

        private string? _icon;
        private SolidColorBrush? _iconForegroundBrush;
        private string? _name;

        /// <summary>
        /// The task icon.
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
        /// The task icon foreground <see cref="SolidColorBrush"/>.
        /// </summary>
        public SolidColorBrush? IconForegroundBrush
        {
            get => _iconForegroundBrush;

            set
            {
                if (_iconForegroundBrush != value)
                {
                    _iconForegroundBrush = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The task name.
        /// </summary>
        public string? Name
        {
            get => _name;

            set
            {
                if (_name != value)
                {
                    _name = value;

                    UpdateIcon();
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Initializes a <see cref="WindowDropdownWindowViewModel"/>.
        /// </summary>
        /// <param name="config"><inheritdoc
        /// cref="Views.Modules.WindowDropdown.WindowDropdownMenuFlyoutItemView.WindowDropdownMenuFlyoutItemView"
        /// path="/param[@name='config']"/></param>
        /// <param name="moduleConfig"><inheritdoc
        /// cref="Views.Modules.WindowDropdown.WindowDropdownMenuFlyoutItemView.WindowDropdownMenuFlyoutItemView"
        /// path="/param[@name='moduleConfig']"/></param>
        /// <param name="processId"><inheritdoc
        /// cref="Views.Modules.WindowDropdown.WindowDropdownMenuFlyoutItemView.WindowDropdownMenuFlyoutItemView"
        /// path="/param[@name='processId']"/></param>
        /// <param name="name"><inheritdoc
        /// cref="Views.Modules.WindowDropdown.WindowDropdownMenuFlyoutItemView.WindowDropdownMenuFlyoutItemView"
        /// path="/param[@name='name']"/></param>
        public WindowDropdownWindowViewModel(Configuration.Json.TermBar config, Configuration.Json.Modules.WindowDropdown moduleConfig, uint processId, string name)
        {
            _config = config;

            if (config.WindowList.ProcessIconMap is not null)
            {
                _process = Process.GetProcessById((int)processId);
            }

            Icon = moduleConfig.Icon;
            IconForegroundBrush = Palette.Instance[config.Flavor].Colors[moduleConfig.AccentColor].SolidColorBrush;
            Name = name;
        }

        /// <summary>
        /// Updates <see cref="Icon"/> and <see cref="IconForegroundBrush"/>
        /// based on the current state of the window.
        /// </summary>
        private void UpdateIcon()
        {
            if (_config.WindowList.ProcessIconMap is not null && _process is not null)
            {
                if (_config.WindowList.ProcessIconMap.TryGetValue(_process.ProcessName, out Configuration.Json.ProcessIconMapping? processIconMapping))
                {
                    if (processIconMapping is not null)
                    {
                        if (processIconMapping.Icon is not null)
                        {
                            Icon = processIconMapping.Icon;
                        }

                        if (processIconMapping.IconColor is not null)
                        {
                            IconForegroundBrush = Palette.Instance[_config.Flavor].Colors[(ColorEnum)processIconMapping.IconColor].SolidColorBrush;
                        }

                        if (processIconMapping.WindowNameIconMap is not null)
                        {
                            foreach (KeyValuePair<string, Configuration.Json.WindowNameIconMapping> windowNameIconMapping in processIconMapping.WindowNameIconMap)
                            {
                                if (Regex.IsMatch(Name!, windowNameIconMapping.Key))
                                {
                                    if (windowNameIconMapping.Value.Icon is not null)
                                    {
                                        Icon = windowNameIconMapping.Value.Icon;
                                    }

                                    if (windowNameIconMapping.Value.IconColor is not null)
                                    {
                                        IconForegroundBrush = Palette.Instance[_config.Flavor].Colors[(ColorEnum)windowNameIconMapping.Value.IconColor].SolidColorBrush;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? callerMemberName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerMemberName));
    }
}