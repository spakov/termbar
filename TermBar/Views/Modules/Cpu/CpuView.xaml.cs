using Spakov.TermBar.ViewModels.Modules.Cpu;

namespace Spakov.TermBar.Views.Modules.Cpu
{
    /// <summary>
    /// The TermBar CPU usage monitor view.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1812:Class is apparently never instantiated", Justification = "Instantiated with Activator.CreateInstance()")]
    internal sealed partial class CpuView : ModuleView
    {
        /// <summary>
        /// Initializes a <see cref="CpuView"/>.
        /// </summary>
        /// <param name="config"><inheritdoc cref="ModuleView.ModuleView"
        /// path="/param[@name='config']"/></param>
        /// <param name="moduleConfig">The <see
        /// cref="Configuration.Json.Modules.Cpu"/> for this <see
        /// cref="CpuView"/>.</param>
        internal CpuView(Configuration.Json.TermBar config, Configuration.Json.Modules.Cpu moduleConfig) : base(config, moduleConfig)
        {
            DataContext = new CpuViewModel(moduleConfig);

            InitializeComponent();
        }
    }
}