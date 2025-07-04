using Microsoft.UI.Xaml.Controls;
using TermBar.ViewModels.Modules;
using Windows.Win32.Foundation;

namespace TermBar.Views.Modules.WindowDropdown {
  internal sealed partial class WindowDropdownMenuFlyoutItemView : MenuFlyoutItem {
    private WindowDropdownWindowViewModel? viewModel;

    private WindowDropdownWindowViewModel? ViewModel {
      get => viewModel;
      set {
        viewModel = value;
        DataContext = viewModel;
      }
    }

    /// <summary>
    /// The window's <see cref="HWND"/>.
    /// </summary>
    internal HWND HWnd { get; private set; }

    /// <summary>
    /// Allows updating the window's name.
    /// </summary>
    internal string? WindowName {
      set {
        if (viewModel is not null) {
          viewModel.Name = value;
        }
      }
    }

    /// <summary>
    /// Initializes a <see cref="WindowDropdownMenuFlyoutItemView"/>.
    /// </summary>
    /// <param name="config"><inheritdoc cref="ModuleView.ModuleView"
    /// path="/param[@name='config']"/></param>
    /// <param name="moduleConfig">The <see
    /// cref="Configuration.Json.Modules.WindowDropdown"/> for this
    /// <see cref="WindowDropdownMenuFlyoutItemView"/>.</param>
    /// <param name="hWnd"><inheritdoc cref="HWnd" path="/summary"/></param>
    /// <param name="processId">The window's owning process ID.</param>
    /// <param name="name">The window's name.</param>
    internal WindowDropdownMenuFlyoutItemView(Configuration.Json.TermBar config, Configuration.Json.Modules.WindowDropdown moduleConfig, HWND hWnd, uint processId, string name) {
      HWnd = hWnd;

      ViewModel = new WindowDropdownWindowViewModel(config, moduleConfig, processId, name);
      DefaultStyleKey = typeof(WindowDropdownMenuFlyoutItemView);
    }
  }
}
