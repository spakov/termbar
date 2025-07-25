using Microsoft.UI.Xaml.Media;
using Spakov.Catppuccin;
using Spakov.TermBar.Views.Modules.SystemDropdown;
using System.Collections.ObjectModel;

namespace Spakov.TermBar.ViewModels.Modules.SystemDropdown
{
    /// <summary>
    /// The system dropdown viewmodel.
    /// </summary>
    internal class SystemDropdownViewModel
    {
        private readonly Configuration.Json.TermBar _config;
        private readonly Configuration.Json.Modules.SystemDropdown _moduleConfig;

        private readonly ObservableCollection<SystemDropdownMenuFlyoutItemView> _views;

        /// <summary>
        /// The dropdown icon.
        /// </summary>
        internal string? Icon => _moduleConfig.Icon;

        /// <summary>
        /// The dropdown icon color.
        /// </summary>
        internal SolidColorBrush? IconColor => Palette.Instance[_config.Flavor].Colors[_moduleConfig.AccentColor].SolidColorBrush;

        /// <summary>
        /// The list of <see cref="SystemDropdownMenuFlyoutItemView"/>s to be
        /// presented to the user.
        /// </summary>
        internal ObservableCollection<SystemDropdownMenuFlyoutItemView> Items => _views;

        /// <summary>
        /// Initializes a <see cref="SystemDropdownViewModel"/>.
        /// </summary>
        /// <param name="config"><inheritdoc
        /// cref="SystemDropdownView.SystemDropdownView"
        /// path="/param[@name='config']"/></param>
        /// <param name="moduleConfig"><inheritdoc
        /// cref="SystemDropdownView.SystemDropdownView"
        /// path="/param[@name='moduleConfig']"/></param>
        internal SystemDropdownViewModel(Configuration.Json.TermBar config, Configuration.Json.Modules.SystemDropdown moduleConfig)
        {
            _config = config;
            _moduleConfig = moduleConfig;

            _views = [];

            foreach (Configuration.Json.Modules.SystemDropdown.SystemDropdownFeature feature in moduleConfig.Features)
            {
                _views.Add(new(feature));
            }
        }
    }
}