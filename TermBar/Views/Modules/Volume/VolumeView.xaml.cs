using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Spakov.TermBar.ViewModels.Modules.Volume;
using Windows.Win32;

namespace Spakov.TermBar.Views.Modules.Volume
{
    /// <summary>
    /// The TermBar volume monitor view.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1812:Class is apparently never instantiated", Justification = "Instantiated with Activator.CreateInstance()")]
    internal sealed partial class VolumeView : ModuleView
    {
        private readonly Configuration.Json.Modules.Volume _moduleConfig;

        private VolumeViewModel? _viewModel;

        /// <summary>
        /// The viewmodel.
        /// </summary>
        public VolumeViewModel? ViewModel
        {
            get => _viewModel;

            set
            {
                _viewModel = value;
                DataContext = _viewModel;
            }
        }

        /// <summary>
        /// Initializes a <see cref="VolumeView"/>.
        /// </summary>
        /// <param name="config"><inheritdoc cref="ModuleView.ModuleView"
        /// path="/param[@name='config']"/></param>
        /// <param name="moduleConfig">The <see
        /// cref="Configuration.Json.Modules.Volume"/> for this <see
        /// cref="VolumeView"/>.</param>
        internal VolumeView(Configuration.Json.TermBar config, Configuration.Json.Modules.Volume moduleConfig) : base(config, moduleConfig)
        {
            _moduleConfig = moduleConfig;

            ViewModel = new VolumeViewModel(moduleConfig);

            InitializeComponent();

            if (moduleConfig.VolumeInteraction is not null)
            {
                if (moduleConfig.VolumeInteraction.ClickToToggleMute)
                {
                    ((StackPanel)Content).PointerPressed += StackPanel_PointerPressed;
                }

                if (moduleConfig.VolumeInteraction.ScrollToAdjust)
                {
                    ((StackPanel)Content).PointerWheelChanged += VolumeView_PointerWheelChanged;
                }
            }
        }

        /// <summary>
        /// Toggles the mute state.
        /// </summary>
        /// <param name="sender"><inheritdoc cref="PointerEventHandler"
        /// path="/param[@name='sender']"/></param>
        /// <param name="e"><inheritdoc cref="PointerEventHandler"
        /// path="/param[@name='e']"/></param>
        private void StackPanel_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (WindowManagement.Windows.EphemeralWindow.Debounce())
            {
                return;
            }

            VolumeViewModel.VolumeMuted = !VolumeViewModel.VolumeMuted;
        }

        /// <summary>
        /// Changes the volume level.
        /// </summary>
        /// <param name="sender"><inheritdoc cref="PointerEventHandler"
        /// path="/param[@name='sender']"/></param>
        /// <param name="e"><inheritdoc cref="PointerEventHandler"
        /// path="/param[@name='e']"/></param>
        private void VolumeView_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint pointerPoint = e.GetCurrentPoint(null);

            float newVolume = VolumeViewModel.VolumePercent
                + (
                    (float)(pointerPoint.Properties.MouseWheelDelta / PInvoke.WHEEL_DELTA)
                    * _moduleConfig.VolumeInteraction!.PercentPerScroll
                );

            VolumeViewModel.VolumePercent = newVolume;
        }
    }
}