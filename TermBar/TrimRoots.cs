using System.Diagnostics.CodeAnalysis;
using TermBar.ViewModels.Modules.Clock;
using TermBar.ViewModels.Modules.Cpu;
using TermBar.ViewModels.Modules.Launcher;
using TermBar.ViewModels.Modules.Memory;
using TermBar.ViewModels.Modules.Terminal;
using TermBar.ViewModels.Modules.Volume;
using TermBar.ViewModels.Modules.WindowBar;
using TermBar.ViewModels.Modules.WindowDropdown;
using TermBar.Views.Modules.Clock;
using TermBar.Views.Modules.Cpu;
using TermBar.Views.Modules.Launcher;
using TermBar.Views.Modules.Memory;
using TermBar.Views.Modules.StaticText;
using TermBar.Views.Modules.Terminal;
using TermBar.Views.Modules.Volume;
using TermBar.Views.Modules.WindowBar;
using TermBar.Views.Modules.WindowDropdown;

namespace TermBar {
  /// <summary>
  /// Prevents trimming of classes that are never instantiated with new().
  /// </summary>
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
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicConstructors, typeof(LauncherView))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(LauncherViewModel))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicConstructors, typeof(MemoryView))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(MemoryViewModel))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicConstructors, typeof(StaticTextView))]
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
    public static void PreserveTrimmableClasses() { }
  }
}
