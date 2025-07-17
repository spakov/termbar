using Microsoft.UI.Xaml;

namespace Spakov.TermBar.Views.Windows {
  /// <summary>
  /// A dialog "window".
  /// </summary>
  internal sealed partial class DialogWindow : Window {
    /// <summary>
    /// Initializes a <see cref="DialogWindow"/>.
    /// </summary>
    /// <param name="config">A <see
    /// cref="Configuration.Json.TermBar"/>.</param>
    /// <param name="content">The content to present in the dialog
    /// window.</param>
    internal DialogWindow(Configuration.Json.TermBar config, UIElement content) : base(config, content) {
      InitializeComponent();
      ApplyComputedStyles();
    }
  }
}