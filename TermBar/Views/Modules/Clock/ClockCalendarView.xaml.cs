using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Spakov.TermBar.Styles;
using Spakov.TermBar.ViewModels.Modules.Clock;

namespace Spakov.TermBar.Views.Modules.Clock
{
    /// <summary>
    /// The TermBar clock calendar view.
    /// </summary>
    internal sealed partial class ClockCalendarView : ModuleView
    {
        private readonly Configuration.Json.TermBar _config;

        private ClockCalendarViewModel? _viewModel;

        /// <summary>
        /// The viewmodel.
        /// </summary>
        private ClockCalendarViewModel? ViewModel
        {
            get => _viewModel;

            set
            {
                _viewModel = value;
                DataContext = _viewModel;
            }
        }

        /// <summary>
        /// Initializes a <see cref="ClockCalendarView"/>.
        /// </summary>
        /// <param name="config"><inheritdoc cref="ModuleView.ModuleView"
        /// path="/param[@name='config']"/></param>
        /// <param name="moduleConfig">The <see
        /// cref="Configuration.Json.Modules.Clock"/> for this <see
        /// cref="ClockCalendarView"/>.</param>
        internal ClockCalendarView(Configuration.Json.TermBar config, Configuration.Json.Modules.Clock moduleConfig) : base(config, moduleConfig)
        {
            _config = config;

            ViewModel = new ClockCalendarViewModel(config, moduleConfig);

            ApplyComputedStyles();
            InitializeComponent();
        }

        /// <summary>
        /// Applies computed styles to the window.
        /// </summary>
        private void ApplyComputedStyles()
        {
            Style gridStyle = new(typeof(Grid));

            StylesHelper.MergeWithAncestor(gridStyle, (Border)Content, typeof(Grid));

            gridStyle.Setters.Add(new Setter(Grid.PaddingProperty, $"{_config.Padding},{_config.Padding / 2}"));

            Resources[typeof(Grid)] = gridStyle;
        }

        /// <summary>
        /// Displays the last month.
        /// </summary>
        /// <param name="sender"><inheritdoc cref="RoutedEventHandler"
        /// path="/param[@name='sender']"/></param>
        /// <param name="e"><inheritdoc cref="RoutedEventHandler"
        /// path="/param[@name='e']"/></param>
        private void LastMonth_Click(object sender, RoutedEventArgs e) => ViewModel?.GoToLastMonth();

        /// <summary>
        /// Displays the next month.
        /// </summary>
        /// <param name="sender"><inheritdoc cref="RoutedEventHandler"
        /// path="/param[@name='sender']"/></param>
        /// <param name="e"><inheritdoc cref="RoutedEventHandler"
        /// path="/param[@name='e']"/></param>
        private void NextMonth_Click(object sender, RoutedEventArgs e) => ViewModel?.GoToNextMonth();

    }
}