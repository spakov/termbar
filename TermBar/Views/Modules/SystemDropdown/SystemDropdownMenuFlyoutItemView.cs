using Microsoft.UI.Xaml.Controls;
using Spakov.TermBar.ViewModels.Modules.SystemDropdown;

namespace Spakov.TermBar.Views.Modules.SystemDropdown
{
    /// <summary>
    /// A system dropdown menu item view.
    /// </summary>
    internal sealed partial class SystemDropdownMenuFlyoutItemView : MenuFlyoutItem
    {
        private SystemDropdownItemViewModel? _viewModel;

        /// <summary>
        /// The viewmodel.
        /// </summary>
        private SystemDropdownItemViewModel? ViewModel
        {
            get => _viewModel;

            set
            {
                _viewModel = value;
                DataContext = _viewModel;
            }
        }

        /// <summary>
        /// The item's <see
        /// cref="Configuration.Json.Modules.SystemDropdown.SystemDropdownFeature"
        /// />.
        /// </summary>
        internal Configuration.Json.Modules.SystemDropdown.SystemDropdownFeature Feature { get; private set; }

        /// <summary>
        /// Initializes a <see cref="SystemDropdownMenuFlyoutItemView"/>.
        /// </summary>
        /// <param name="feature"><inheritdoc cref="Feature"
        /// path="/summary"/></param>
        internal SystemDropdownMenuFlyoutItemView(Configuration.Json.Modules.SystemDropdown.SystemDropdownFeature feature)
        {
            Feature = feature;

            ViewModel = new SystemDropdownItemViewModel(feature);
        }
    }
}