using Microsoft.UI.Xaml.Controls;
using Spakov.TermBar.ViewModels.Modules.WindowBar;
using System;

namespace Spakov.TermBar.Views.Modules.WindowBar
{
    /// <summary>
    /// The TermBar window bar view.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1812:Class is apparently never instantiated", Justification = "Instantiated with Activator.CreateInstance()")]
    internal sealed partial class WindowBarView : ModuleView
    {
        private WindowBarViewModel? _viewModel;

        private readonly Configuration.Json.Modules.WindowBar _config;

        /// <summary>
        /// The viewmodel.
        /// </summary>
        private WindowBarViewModel? ViewModel
        {
            get => _viewModel;

            set
            {
                _viewModel = value;
                DataContext = _viewModel;
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
        internal WindowBarView(Configuration.Json.TermBar config, Configuration.Json.Modules.WindowBar moduleConfig) : base(config, moduleConfig, skipColor: true)
        {
            _config = moduleConfig;

            ViewModel = new WindowBarViewModel(this, config, moduleConfig);
            InitializeComponent();

            ((ListView)Content).ItemClick += WindowBarView_ItemClick;

            if (moduleConfig.Windows is not null && moduleConfig.Windows.ScrollIntoView)
            {
                ((ListView)Content).SelectionChanged += ListView_SelectionChanged;
            }
        }

        /// <summary>
        /// Foregrounds or iconifies the clicked window, as appropriate.
        /// </summary>
        /// <param name="sender"><inheritdoc cref="ItemClickEventHandler"
        /// path="/param[@name='sender']"/></param>
        /// <param name="e"><inheritdoc cref="ItemClickEventHandler"
        /// path="/param[@name='e']"/></param>
        private void WindowBarView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (ReferenceEquals(e.ClickedItem, ViewModel!.LastForegroundWindow))
            {
                ViewModel!.Iconify();
            }
            else if (!ReferenceEquals(e.ClickedItem, ViewModel!.ForegroundWindow))
            {
                ViewModel!.Foreground((WindowBarWindowView)e.ClickedItem);
            }
        }

        /// <summary>
        /// Ensures the foregrounded window is selected, optionally scrolling
        /// it into view.
        /// </summary>
        /// <param name="sender"><inheritdoc cref="SelectionChangedEventHandler"
        /// path="/param[@name='sender']"/></param>
        /// <param name="e"><inheritdoc cref="SelectionChangedEventHandler"
        /// path="/param[@name='e']"/></param>
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_config.Windows is not null && _config.Windows.ScrollIntoView)
            {
                if (((ListView)Content).SelectedItem is not null)
                {
                    if (((ListView)Content).SelectedIndex < ((ListView)Content).Items.Count)
                    {
                        ((ListView)Content).ScrollIntoView(((ListView)Content).Items[((ListView)Content).SelectedIndex]);
                    }
                }
            }
        }

        /// <summary>
        /// Implements delayed foregrounding after the <see cref="ListView"/>
        /// has settled.
        /// </summary>
        /// <remarks>Please be sure to see the comment attached to <see
        /// cref="WindowBarViewModel.ForegroundWindow"/>'s setter for an
        /// explanation of what's happening here.</remarks>
        /// <param name="sender"><inheritdoc cref="EventHandler"
        /// path="/param[@name='sender']"/></param>
        /// <param name="e"><inheritdoc cref="EventHandler"
        /// path="/param[@name='e']"/></param>
        private void ListView_LayoutUpdated(object sender, object e)
        {
            if (!ReferenceEquals(((ListView)Content).SelectedItem, ViewModel!.ForegroundWindow))
            {
                ViewModel!.OnPropertyChanged("ForegroundWindow");
            }
        }
    }
}