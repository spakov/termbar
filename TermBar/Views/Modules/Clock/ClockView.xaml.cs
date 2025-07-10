using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Spakov.TermBar.ViewModels.Modules.Clock;

namespace Spakov.TermBar.Views.Modules.Clock {
  /// <summary>
  /// The TermBar clock.
  /// </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1812:Class is apparently never instantiated", Justification = "Instantiated with Activator.CreateInstance()")]
  internal sealed partial class ClockView : ModuleView {
    private readonly Configuration.Json.TermBar config;
    private readonly Configuration.Json.Modules.Clock moduleConfig;

    private ClockViewModel? viewModel;

    /// <summary>
    /// The viewmodel.
    /// </summary>
    private ClockViewModel? ViewModel {
      get => viewModel;
      set {
        viewModel = value;
        DataContext = viewModel;
      }
    }

    /// <summary>
    /// Initializes a <see cref="ClockView"/>.
    /// </summary>
    /// <param name="config"><inheritdoc cref="ModuleView.ModuleView"
    /// path="/param[@name='config']"/></param>
    /// <param name="moduleConfig">The <see
    /// cref="Configuration.Json.Modules.Clock"/> for this <see
    /// cref="ClockView"/>.</param>
    internal ClockView(Configuration.Json.TermBar config, Configuration.Json.Modules.Clock moduleConfig) : base(config, moduleConfig) {
      this.config = config;
      this.moduleConfig = moduleConfig;

      ViewModel = new ClockViewModel(moduleConfig);

      InitializeComponent();

      if (moduleConfig.Calendar is not null) {
        ((StackPanel) Content).PointerPressed += StackPanel_PointerPressed;
      }
    }

    /// <summary>
    /// Invoked when the user clicks the clock.
    /// </summary>
    /// <param name="sender"><inheritdoc cref="PointerEventHandler"
    /// path="/param[@name='sender']"/></param>
    /// <param name="e"><inheritdoc cref="PointerEventHandler"
    /// path="/param[@name='e']"/></param>
    private void StackPanel_PointerPressed(object sender, PointerRoutedEventArgs e) {
      if (WindowManagement.Windows.EphemeralWindow.Debounce()) return;

      PointerPoint pointerPoint = e.GetCurrentPoint(null);

      WindowManagement.Windows.EphemeralWindow ephemeralWindow = new(
        config,
        (int) pointerPoint.Position.X,
        (int) pointerPoint.Position.Y,
        new ClockCalendarView(config, moduleConfig)
      );

      ephemeralWindow.Display();
    }
  }
}