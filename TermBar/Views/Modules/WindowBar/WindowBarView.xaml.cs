using Microsoft.UI.Xaml.Controls;
using TermBar.ViewModels.Modules.WindowBar;

namespace TermBar.Views.Modules.WindowBar {
  /// <summary>
  /// The TermBar window bar.
  /// </summary>
  internal sealed partial class WindowBarView : ModuleView {
    private WindowBarViewModel? viewModel;

    /// <summary>
    /// The viewmodel.
    /// </summary>
    private WindowBarViewModel? ViewModel {
      get => viewModel;
      set {
        viewModel = value;
        DataContext = viewModel;
      }
    }

    /// <summary>
    /// Initializes a <see cref="WindowBarView"/>.
    /// </summary>
    /// <param name="config"><inheritdoc cref="ModuleView.ModuleView"
    /// path="/param[@name='config']"/></param>
    /// <param name="moduleConfig">The <see
    /// cref="Configuration.Json.Modules.WindowBar"/> for this <see
    /// cref="WindowBarView"/>.</param>
    internal WindowBarView(Configuration.Json.TermBar config, Configuration.Json.Modules.WindowBar moduleConfig) : base(config, moduleConfig, skipColor: true) {
      ViewModel = new WindowBarViewModel(this, config, moduleConfig);

      InitializeComponent();

      ((ListView) Content).ItemClick += WindowBarView_ItemClick;
    }

    /// <summary>
    /// Sets the selected window index.
    /// </summary>
    /// <param name="index">The selected window index.</param>
    internal void SetSelectedWindowIndex(int index) => ((ListView) Content).SelectedIndex = index;

    /// <summary>
    /// Invoked when the user clicks an item.
    /// </summary>
    /// <remarks>Note that foregrounding is handled separately via
    /// <c>SelectedItem</c>.</remarks>
    /// <param name="sender"><inheritdoc cref="ItemClickEventHandler"
    /// path="/param[@name='sender']"/></param>
    /// <param name="e"><inheritdoc cref="ItemClickEventHandler"
    /// path="/param[@name='e']"/></param>
    private void WindowBarView_ItemClick(object sender, ItemClickEventArgs e) {
      if (e.ClickedItem.Equals(ViewModel!.ForegroundedWindow)) {
        WindowBarViewModel.Iconify();
      }
    }
  }
}
