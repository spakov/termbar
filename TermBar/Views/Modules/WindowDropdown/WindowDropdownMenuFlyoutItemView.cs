using Microsoft.UI.Xaml.Controls;
using Spakov.TermBar.Models;
using Spakov.TermBar.ViewModels.Modules.WindowDropdown;
using Windows.Win32.Foundation;

namespace Spakov.TermBar.Views.Modules.WindowDropdown
{
    /// <summary>
    /// A window dropdown menu item view.
    /// </summary>
    internal sealed partial class WindowDropdownMenuFlyoutItemView : MenuFlyoutItem, IWindowListWindow
    {
        private WindowDropdownWindowViewModel? _viewModel;

        private readonly uint _windowProcessId;

        /// <summary>
        /// The viewmodel.
        /// </summary>
        private WindowDropdownWindowViewModel? ViewModel
        {
            get => _viewModel;

            set
            {
                _viewModel = value;
                DataContext = _viewModel;
            }
        }

        public uint WindowProcessId => _windowProcessId;

        /// <summary>
        /// The window's name.
        /// </summary>
        public string? WindowName
        {
            get => _viewModel?.Name;

            set
            {
                if (_viewModel is not null)
                {
                    _viewModel.Name = value;
                }
            }
        }

        /// <summary>
        /// The window's <see cref="HWND"/>.
        /// </summary>
        internal HWND HWnd { get; private set; }

        /// <summary>
        /// Initializes a <see cref="WindowDropdownMenuFlyoutItemView"/>.
        /// </summary>
        /// <param name="config"><inheritdoc cref="ModuleView.ModuleView"
        /// path="/param[@name='config']"/></param>
        /// <param name="moduleConfig">The <see
        /// cref="Configuration.Json.Modules.WindowDropdown"/> for this
        /// <see cref="WindowDropdownMenuFlyoutItemView"/>.</param>
        /// <param name="hWnd"><inheritdoc cref="HWnd"
        /// path="/summary"/></param>
        /// <param name="processId">The window's owning process ID.</param>
        /// <param name="name">The window's name.</param>
        internal WindowDropdownMenuFlyoutItemView(Configuration.Json.TermBar config, Configuration.Json.Modules.WindowDropdown moduleConfig, HWND hWnd, uint processId, string name)
        {
            _windowProcessId = processId;
            HWnd = hWnd;

            ViewModel = new WindowDropdownWindowViewModel(config, moduleConfig, processId, name);
            DefaultStyleKey = typeof(WindowDropdownMenuFlyoutItemView);
        }
    }
}