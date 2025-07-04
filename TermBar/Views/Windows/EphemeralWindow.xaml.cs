using Microsoft.UI.Xaml;

namespace TermBar.Views.Windows {
  /// <summary>
  /// An ephemeral "window".
  /// </summary>
  internal sealed partial class EphemeralWindow : Window {
    /// <summary>
    /// Initializes an <see cref="EphemeralWindow"/>.
    /// </summary>
    /// <param name="config">A <see
    /// cref="Configuration.Json.TermBar"/>.</param>
    /// <param name="content">The content to present in the ephemeral
    /// window.</param>
    internal EphemeralWindow(Configuration.Json.TermBar config, UIElement content) : base(config, content) {
      InitializeComponent();
      ApplyComputedStyles();
    }
  }
}
