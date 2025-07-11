using Microsoft.UI.Xaml.Controls;
using Spakov.TermBar.ViewModels.Modules.WindowBar;
using System;

namespace Spakov.TermBar.Views.Modules.WindowBar {
  /// <summary>
  /// The TermBar window bar.
  /// </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1812:Class is apparently never instantiated", Justification = "Instantiated with Activator.CreateInstance()")]
  internal sealed partial class WindowBarView : ModuleView {
    private WindowBarViewModel? viewModel;

    private readonly Configuration.Json.Modules.WindowBar config;

    private int? requestedSelectedIndex;

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
      this.config = moduleConfig;

      ViewModel = new WindowBarViewModel(this, config, moduleConfig);

      requestedSelectedIndex = null;

      InitializeComponent();

      ((ListView) Content).ItemClick += WindowBarView_ItemClick;

      if (moduleConfig.Windows is not null && moduleConfig.Windows.ScrollIntoView) {
        ((ListView) Content).SelectionChanged += ListView_SelectionChanged;
      }
    }

    /// <summary>
    /// Sets the selected window index.
    /// </summary>
    /// <param name="index">The selected window index.</param>
    internal void SetSelectedWindowIndex(int index) => requestedSelectedIndex = index;

    /// <summary>
    /// Invoked when the user clicks a window.
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

    /// <summary>
    /// Invoked when the <see cref="ListView"/> updates its layout.
    /// </summary>
    /// <param name="sender"><inheritdoc cref="EventHandler"
    /// path="/param[@name='sender']"/></param>
    /// <param name="e"><inheritdoc cref="EventHandler"
    /// path="/param[@name='e']"/></param>
    private void ListView_LayoutUpdated(object sender, object e) {
      if (requestedSelectedIndex is not null) {
        ((ListView) Content).SelectedIndex = (int) requestedSelectedIndex;
        requestedSelectedIndex = null;
      }
    }

    /// <summary>
    /// Invoked when the <see cref="ListView"/>'s selection has finished
    /// changing.
    /// </summary>
    /// <param name="sender"><inheritdoc cref="EventHandler"
    /// path="/param[@name='sender']"/></param>
    /// <param name="e"><inheritdoc cref="EventHandler"
    /// path="/param[@name='e']"/></param>
    private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      if (config.Windows is not null && config.Windows.ScrollIntoView) {
        if (((ListView) Content).SelectedItem is not null) {
          if (((ListView) Content).SelectedIndex < ((ListView) Content).Items.Count) {
            ((ListView) Content).ScrollIntoView(((ListView) Content).Items[((ListView) Content).SelectedIndex]);
          }
        }
      }
    }
  }
}