using Spakov.TermBar.ViewModels.Modules.Memory;

namespace Spakov.TermBar.Views.Modules.Memory
{
    /// <summary>
    /// The TermBar memory usage monitor view.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1812:Class is apparently never instantiated", Justification = "Instantiated with Activator.CreateInstance()")]
    internal sealed partial class MemoryView : ModuleView
    {
        /// <summary>
        /// Initializes a <see cref="MemoryView"/>.
        /// </summary>
        /// <param name="config"><inheritdoc cref="ModuleView.ModuleView"
        /// path="/param[@name='config']"/></param>
        /// <param name="moduleConfig">The <see
        /// cref="Configuration.Json.Modules.Memory"/> for this <see
        /// cref="MemoryView"/>.</param>
        internal MemoryView(Configuration.Json.TermBar config, Configuration.Json.Modules.Memory moduleConfig) : base(config, moduleConfig)
        {
            DataContext = new MemoryViewModel(moduleConfig);

            InitializeComponent();
        }
    }
}