using Microsoft.UI.Xaml.Media;

namespace Spakov.TermBar.ViewModels.Modules.Launcher
{
    /// <summary>
    /// A <see cref="LauncherViewModel"/> launcher entry viewmodel.
    /// </summary>
    internal class LauncherLauncherEntryViewModel
    {
        /// <summary>
        /// The name of the launcher entry.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The command to run when launching, as a <see
        /// cref="System.ValueTuple{T1, T2}"/> of the command and its
        /// arguments.
        /// </summary>
        public (string?, string[]?) Command { get; set; }

        /// <summary>
        /// Whether to display the launcher entry's name.
        /// </summary>
        public required bool DisplayName { get; set; }

        /// <summary>
        /// The launcher entry's icon color.
        /// </summary>
        public SolidColorBrush? IconColor { get; set; }

        /// <summary>
        /// The launcher entry's icon.
        /// </summary>
        public string? Icon { get; set; }
    }
}