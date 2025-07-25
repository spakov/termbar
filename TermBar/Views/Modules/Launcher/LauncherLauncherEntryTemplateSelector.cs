using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Spakov.TermBar.ViewModels.Modules.Launcher;

namespace Spakov.TermBar.Views.Modules.Launcher
{
    /// <summary>
    /// Selects the appropriate <see cref="DataTemplate"/> based on a <see
    /// cref="LauncherLauncherEntryViewModel"/>.
    /// </summary>
    public partial class LauncherLauncherEntryTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? LauncherEntriesEntryTemplate { get; set; }
        public DataTemplate? LauncherEntriesNamedEntryTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            return item is LauncherLauncherEntryViewModel launcherEntry
                ? !launcherEntry.DisplayName
                    ? LauncherEntriesEntryTemplate!
                    : LauncherEntriesNamedEntryTemplate!
                : base.SelectTemplateCore(item);
        }
    }
}