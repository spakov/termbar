using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Spakov.TermBar.ViewModels.Modules.Clock;

namespace Spakov.TermBar.Views.Modules.Clock
{
    /// <summary>
    /// The TermBar clock view.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1812:Class is apparently never instantiated", Justification = "Instantiated with Activator.CreateInstance()")]
    internal sealed partial class ClockView : ModuleView
    {
        private readonly Configuration.Json.TermBar _config;
        private readonly Configuration.Json.Modules.Clock _moduleConfig;

        private ClockViewModel? _viewModel;

        /// <summary>
        /// The viewmodel.
        /// </summary>
        private ClockViewModel? ViewModel
        {
            get => _viewModel;

            set
            {
                _viewModel = value;
                DataContext = _viewModel;
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
        internal ClockView(Configuration.Json.TermBar config, Configuration.Json.Modules.Clock moduleConfig) : base(config, moduleConfig)
        {
            _config = config;
            _moduleConfig = moduleConfig;

            ViewModel = new ClockViewModel(moduleConfig);

            InitializeComponent();

            if (moduleConfig.Calendar is not null)
            {
                ((StackPanel)Content).PointerPressed += StackPanel_PointerPressed;
            }
        }

        /// <summary>
        /// Displays the calendar ephemeral window.
        /// </summary>
        /// <param name="sender"><inheritdoc cref="PointerEventHandler"
        /// path="/param[@name='sender']"/></param>
        /// <param name="e"><inheritdoc cref="PointerEventHandler"
        /// path="/param[@name='e']"/></param>
        private void StackPanel_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (!e.GetCurrentPoint((StackPanel)sender).Properties.IsLeftButtonPressed)
            {
                return;
            }

            if (WindowManagement.Windows.EphemeralWindow.Debounce())
            {
                return;
            }

            PointerPoint pointerPoint = e.GetCurrentPoint(null);

            WindowManagement.Windows.EphemeralWindow ephemeralWindow = new(
                _config,
                (int)pointerPoint.Position.X,
                (int)pointerPoint.Position.Y,
                new ClockCalendarView(_config, _moduleConfig)
            );

            ephemeralWindow.Display();
        }
    }
}