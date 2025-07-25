using Spakov.TermBar.ViewModels.Modules.Gpu;

namespace Spakov.TermBar.Views.Modules.Gpu
{
    /// <summary>
    /// The TermBar GPU usage monitor view.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1812:Class is apparently never instantiated", Justification = "Instantiated with Activator.CreateInstance()")]
    internal sealed partial class GpuView : ModuleView
    {
        /// <summary>
        /// Initializes a <see cref="GpuView"/>.
        /// </summary>
        /// <param name="config"><inheritdoc cref="ModuleView.ModuleView"
        /// path="/param[@name='config']"/></param>
        /// <param name="moduleConfig">The <see
        /// cref="Configuration.Json.Modules.Gpu"/> for this <see
        /// cref="GpuView"/>.</param>
        internal GpuView(Configuration.Json.TermBar config, Configuration.Json.Modules.Gpu moduleConfig) : base(config, moduleConfig)
        {
            DataContext = new GpuViewModel(moduleConfig);

            InitializeComponent();
        }
    }
}