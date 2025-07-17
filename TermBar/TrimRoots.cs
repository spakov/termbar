using Spakov.TermBar.ViewModels.Modules.Clock;
using Spakov.TermBar.ViewModels.Modules.Cpu;
using Spakov.TermBar.ViewModels.Modules.Gpu;
using Spakov.TermBar.ViewModels.Modules.Launcher;
using Spakov.TermBar.ViewModels.Modules.Memory;
using Spakov.TermBar.ViewModels.Modules.SystemDropdown;
using Spakov.TermBar.ViewModels.Modules.Terminal;
using Spakov.TermBar.ViewModels.Modules.Volume;
using Spakov.TermBar.ViewModels.Modules.WindowBar;
using Spakov.TermBar.ViewModels.Modules.WindowDropdown;
using Spakov.TermBar.Views.Modules.Clock;
using Spakov.TermBar.Views.Modules.Cpu;
using Spakov.TermBar.Views.Modules.Gpu;
using Spakov.TermBar.Views.Modules.Launcher;
using Spakov.TermBar.Views.Modules.Memory;
using Spakov.TermBar.Views.Modules.StaticText;
using Spakov.TermBar.Views.Modules.SystemDropdown;
using Spakov.TermBar.Views.Modules.Terminal;
using Spakov.TermBar.Views.Modules.Volume;
using Spakov.TermBar.Views.Modules.WindowBar;
using Spakov.TermBar.Views.Modules.WindowDropdown;
using System.Diagnostics.CodeAnalysis;

namespace Spakov.TermBar {
  /// <summary>
  /// Prevents trimming of classes that are never instantiated with new().
  /// </summary>
#pragma warning disable IDE0079 // Remove unnecessary suppression
  [SuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Types are preserved")]
#pragma warning restore IDE0079 // Remove unnecessary suppression
  internal static class TrimRoots {
    /// <summary>
    /// This method, which does nothing at runtime, must be invoked to ensure
    /// the classes marked with <see cref="DynamicDependencyAttribute"/> are
    /// preserved by the trimmer.
    /// </summary>
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicConstructors, typeof(ClockView))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ClockViewModel))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicConstructors, typeof(ClockCalendarView))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ClockCalendarViewModel))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicConstructors, typeof(CpuView))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(CpuViewModel))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicConstructors, typeof(GpuView))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(GpuViewModel))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicConstructors, typeof(LauncherView))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(LauncherViewModel))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicConstructors, typeof(MemoryView))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(MemoryViewModel))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicConstructors, typeof(StaticTextView))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicConstructors, typeof(SystemDropdownView))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SystemDropdownViewModel))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SystemDropdownItemViewModel))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicConstructors, typeof(TerminalView))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(TerminalViewModel))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicConstructors, typeof(VolumeView))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(VolumeViewModel))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicConstructors, typeof(WindowBarView))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(WindowBarViewModel))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicConstructors, typeof(WindowBarWindowView))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(WindowBarWindowViewModel))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicConstructors, typeof(WindowDropdownView))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(WindowDropdownViewModel))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(WindowDropdownWindowViewModel))]
    public static void PreserveTrimmableClasses() { }
  }
}