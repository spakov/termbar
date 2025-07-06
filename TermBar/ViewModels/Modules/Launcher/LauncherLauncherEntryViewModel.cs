using Microsoft.UI.Xaml.Media;

namespace TermBar.ViewModels.Modules.Launcher {
  internal class LauncherLauncherEntryViewModel {
    public string? Name { get; set; }
    public string? Command { get; set; }
    public required bool DisplayName { get; set; }
    public SolidColorBrush? IconColor { get; set; }
    public string? Icon { get; set; }
  }
}
