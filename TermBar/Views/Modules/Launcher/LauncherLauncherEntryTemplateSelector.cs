using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TermBar.ViewModels.Modules.Launcher;

namespace TermBar.Views.Modules.Launcher {
  public partial class LauncherLauncherEntryTemplateSelector : DataTemplateSelector {
    public DataTemplate? LauncherEntriesEntryTemplate { get; set; }
    public DataTemplate? LauncherEntriesNamedEntryTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item) {
      return item is LauncherLauncherEntryViewModel launcherEntry
        ? !launcherEntry.DisplayName
          ? LauncherEntriesEntryTemplate!
          : LauncherEntriesNamedEntryTemplate!
        : base.SelectTemplateCore(item);
    }
  }
}
