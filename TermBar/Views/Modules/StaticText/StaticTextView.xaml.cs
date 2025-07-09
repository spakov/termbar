using System.Diagnostics.CodeAnalysis;

namespace TermBar.Views.Modules.StaticText {
  /// <summary>
  /// The TermBar static text.
  /// </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1812:Class is apparently never instantiated", Justification = "Instantiated with Activator.CreateInstance()")]
  internal sealed partial class StaticTextView : ModuleView {
    /// <summary>
    /// Initializes a <see cref="StaticTextView"/>.
    /// </summary>
    /// <param name="config"><inheritdoc cref="ModuleView.ModuleView"
    /// path="/param[@name='config']"/></param>
    /// <param name="moduleConfig">The <see
    /// cref="Configuration.Json.Modules.StaticText"/> for this <see
    /// cref="StaticTextView"/>.</param>
    internal StaticTextView(Configuration.Json.TermBar config, Configuration.Json.Modules.StaticText moduleConfig) : base(config, moduleConfig) {
      InitializeComponent();

      Icon.Text = moduleConfig.Icon;
      Text.Text = moduleConfig.Text;
    }
  }
}
