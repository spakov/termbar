#if DEBUG
using Microsoft.Extensions.Logging;
using Spakov.TermBar.Views.Modules.WindowBar;

#endif
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Dwm;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Spakov.TermBar.Models {
  /// <summary>
  /// Helper methods abstracting Win32 for <see cref="WindowList"/>.
  /// </summary>
  internal static class WindowListHelper {
    private static readonly List<string> ignoredClassNames = [
      "Microsoft.UI.Content.PopupWindowSiteBridge",
      "Progman",
      "PseudoConsoleWindow",
      "Tooltips_Class32",
      "WorkerW",
      "Xaml_WindowedPopupClass"
    ];

    private static WNDENUMPROC? wndEnumProc;

    private static readonly List<HWND> hWnds = [];
    private static readonly Dictionary<HWND, CancellationTokenSource> pendingRemovals = [];

    private static int windowNameLength;
    private static readonly char[] windowName = new char[256];

    private static int windowClassNameLength;
    private static readonly char[] windowClassName = new char[256];

#pragma warning disable IDE0044 // Add readonly modifier
    private static uint windowProcessId;
#pragma warning restore IDE0044 // Add readonly modifier

    private static readonly HashSet<HWND> _windows = [];

    /// <summary>
    /// Returns the windows to be managed.
    /// </summary>
    /// <remarks>Call <see cref="UpdateWindow"/> to keep the window list in
    /// sync.</remarks>
#if DEBUG
    /// <param name="logger">An <see cref="ILogger"/>.</param>
#endif
    /// <returns>An <see cref="ObservableCollection{T}"/> of
    /// <see cref="Window"/>s representing the windows to be managed.</returns>
#if DEBUG
    internal static ObservableCollection<Window> EnumerateWindows(ILogger logger) {
#else
    internal static ObservableCollection<Window> EnumerateWindows() {
#endif
      ObservableCollection<Window> windows = [];

      wndEnumProc = new(WndEnumProc);

      PInvoke.EnumWindows(wndEnumProc, 0);

      foreach (HWND hWnd in hWnds) {
#if DEBUG
        bool isInteresting = WindowIsInteresting(logger, hWnd);
#else
        bool isInteresting = WindowIsInteresting(hWnd);
#endif

        string name = new(windowName, 0, windowNameLength);

        _windows.Add(hWnd);
        windows.Add(new(hWnd, windowProcessId, name, isInteresting));

#if DEBUG
        logger.LogDebug("Tracking window {hWnd} \"{name}\" (interesting: {isInteresting})", hWnd, name, isInteresting);
#endif
      }

      return windows;
    }

    /// <summary>
    /// Determines the correct order in which <paramref name="window"/> should
    /// appear in <paramref name="windowList"/> and inserts it at the proper
    /// index.
    /// </summary>
    /// <typeparam name="T">A type representing a window.</typeparam>
    /// <param name="config">A <see cref="Configuration.Json.WindowList"/>
    /// configuration.</param>
    /// <param name="window">The window to be added.</param>
    /// <param name="windowList">The window list.</param>
    /// <param name="processId">The window's process ID.</param>
    /// <param name="name">The window's name.</param>
    internal static void OrderAndInsert<T>(Configuration.Json.WindowList config, T window, ObservableCollection<T> windowList, uint processId, string? name) where T : IWindowListWindow {
      Range highPriorityWindows = new(-1, -1);
      Range normalPriorityWindows = new(-1, -1);
      Range lowPriorityWindows = new(-1, -1);

      // Adjust ranges
      for (int i = 0; i < windowList.Count; i++) {
        using (Process process = Process.GetProcessById((int) windowList[i].WindowProcessId)) {
          if (config.HighPriorityWindows is not null) {
            if (config.HighPriorityWindows.Contains(process.ProcessName)) {
              if (highPriorityWindows.Low < 0) {
                highPriorityWindows.Low = i;
              }
            } else {
              if (normalPriorityWindows.Low < 0) {
                normalPriorityWindows.Low = i;
              }
            }
          } else {
            if (normalPriorityWindows.Low < 0) {
              normalPriorityWindows.Low = i;
            }
          }

          if (config.LowPriorityWindows is not null) {
            if (config.LowPriorityWindows.Contains(process.ProcessName)) {
              if (lowPriorityWindows.Low < 0) {
                lowPriorityWindows.Low = i;
              }
            }
          }
        }
      }

      if (lowPriorityWindows.High < 0 && lowPriorityWindows.Low >= 0) {
        lowPriorityWindows.High = windowList.Count - 1;
      }

      if (normalPriorityWindows.High < 0 && normalPriorityWindows.Low >= 0) {
        normalPriorityWindows.High = lowPriorityWindows.Low >= 0
          ? lowPriorityWindows.Low - 1
          : windowList.Count - 1;
      }

      if (highPriorityWindows.High < 0 && highPriorityWindows.Low >= 0) {
        highPriorityWindows.High = normalPriorityWindows.Low >= 0
          ? normalPriorityWindows.Low - 1
          : windowList.Count - 1;
      }

      Range insertionRange = normalPriorityWindows;

      string processName;
      using (Process process = Process.GetProcessById((int) processId)) {
        processName = process.ProcessName;
      }

      // Determine priority
      if (config.LowPriorityWindows is not null) {
        if (config.LowPriorityWindows.Contains(processName)) {
          insertionRange = lowPriorityWindows;
        }
      }

      if (config.HighPriorityWindows is not null) {
        if (config.HighPriorityWindows.Contains(processName)) {
          insertionRange = highPriorityWindows;
        }
      }

      // If we have no insertion range at this point, our list is empty
      if (insertionRange.Low < 0 && insertionRange.High < 0) {
        windowList.Add(window);

        return;
      }

      int groupLowIndex = -1;

      // Determine grouping
      for (int i = insertionRange.Low; i <= insertionRange.High; i++) {
        using (Process process = Process.GetProcessById((int) windowList[i].WindowProcessId)) {
          if (process.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase)) {
            groupLowIndex = i;
            break;
          }
        }
      }

      if (groupLowIndex >= 0) {
        insertionRange.Low = groupLowIndex;
      }

      int insertionIndex = -1;

      // Check for group alphabetic sorting
      if (config.SortGroupsAlphabetically && groupLowIndex >= 0) {
        for (int i = insertionRange.Low; i <= insertionRange.High; i++) {
          using (Process process = Process.GetProcessById((int) windowList[i].WindowProcessId)) {
            if (!process.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase)) {
              insertionIndex = i;
              break;
            } else {
              if (string.Compare(windowList[i].WindowName, name, StringComparison.OrdinalIgnoreCase) > 0) {
                insertionIndex = i;
                break;
              }
            }
          }
        }
      }

      // Determine the insertion index
      if (insertionIndex < 0) {
        for (int i = insertionRange.Low; i <= insertionRange.High; i++) {
          if (config.HighPriorityWindows is not null) {
            if (config.HighPriorityWindows.Contains(processName)) {
              using (Process process = Process.GetProcessById((int) windowList[i].WindowProcessId)) {
                if (config.HighPriorityWindows.IndexOf(process.ProcessName) > config.HighPriorityWindows.IndexOf(processName)) {
                  insertionIndex = i;
                  break;
                }
              }
            }
          }

          if (config.LowPriorityWindows is not null) {
            if (config.LowPriorityWindows.Contains(processName)) {
              using (Process process = Process.GetProcessById((int) windowList[i].WindowProcessId)) {
                if (config.LowPriorityWindows.IndexOf(process.ProcessName) > config.LowPriorityWindows.IndexOf(processName)) {
                  insertionIndex = i;
                  break;
                }
              }
            }
          }
        }

        if (insertionIndex < 0) {
          insertionIndex = insertionRange.High + 1;
        }
      }

      windowList.Insert(insertionIndex, window);

      return;
    }

    /// <summary>
    /// Determines whether a window has been foregrounded based on <paramref
    /// name="event"/>.
    /// </summary>
#if DEBUG
    /// <param name="logger">An <see cref="ILogger"/>.</param>
#endif
    /// <param name="windows">The list of windows.</param>
    /// <param name="event"><inheritdoc cref="WindowList.WinSystemEventProc"
    /// path="/param[@name='event']"/></param>
    /// <param name="hWnd"><inheritdoc cref="WindowList.WinSystemEventProc"
    /// path="/param[@name='hWnd']"/></param>
#if DEBUG
    internal static Window? IsForegrounded(ILogger logger, ObservableCollection<Window> windows, uint @event, HWND hWnd) {
#else
    internal static Window? IsForegrounded(ObservableCollection<Window> windows, uint @event, HWND hWnd) {
#endif
      if (@event == PInvoke.EVENT_SYSTEM_FOREGROUND) {
        if (_windows.Contains(hWnd)) {
          foreach (Window window in windows.Where(window => window.HWnd.Equals(hWnd))) {
#if DEBUG
            logger.LogDebug("Foregrounded: window {hWnd} \"{name}\" (interesting: {isInteresting})", hWnd, window.Name, window.IsInteresting);
#endif

            return window;
          }
        } else {
#if DEBUG
          logger.LogDebug("Foregrounded: untracked window {hWnd}, will track it", hWnd);
#endif

          string name;

#if DEBUG
          bool isInteresting = WindowIsInteresting(logger, hWnd);
#else
          bool isInteresting = WindowIsInteresting(hWnd);
#endif

          name = new(windowName, 0, windowNameLength);

#if DEBUG
          logger.LogDebug("Tracking window {hWnd} \"{name}\" (interesting: {isInteresting})", hWnd, name, isInteresting);
#endif

          Window window = new(hWnd, windowProcessId, name, isInteresting);

          _windows.Add(hWnd);
          windows.Add(window);

          return window;
        }
      }

#if DEBUG
      logger.LogWarning("Foregrounded: unknown window {hWnd}", hWnd);
#endif

      return null;
    }

    /// <summary>
    /// Updates the list of windows based on <paramref name="event"/>.
    /// </summary>
#if DEBUG
    /// <param name="logger">An <see cref="ILogger"/>.</param>
#endif
    /// <param name="windows">The list of windows.</param>
    /// <param name="event"><inheritdoc
    /// cref="WindowList.WinObjectEventProc"
    /// path="/param[@name='event']"/></param>
    /// <param name="hWnd"><inheritdoc
    /// cref="WindowList.WinObjectEventProc"
    /// path="/param[@name='hWnd']"/></param>
#if DEBUG
    internal static void UpdateWindow(ILogger logger, ObservableCollection<Window> windows, uint @event, HWND hWnd) {
#else
    internal static void UpdateWindow(ObservableCollection<Window> windows, uint @event, HWND hWnd) {
#endif
      if (
        hWnd == HWND.Null
        || (
          @event != PInvoke.EVENT_OBJECT_CREATE
          && @event != PInvoke.EVENT_OBJECT_SHOW
          && @event != PInvoke.EVENT_OBJECT_UNCLOAKED
          && @event != PInvoke.EVENT_OBJECT_DESTROY
          && @event != PInvoke.EVENT_OBJECT_HIDE
          && @event != PInvoke.EVENT_OBJECT_CLOAKED
          && @event != PInvoke.EVENT_OBJECT_NAMECHANGE
        )
      ) {
        return;
      }

      string name;

#if DEBUG
      bool isInteresting = WindowIsInteresting(logger, hWnd);

      logger.LogTrace("WinObjectEventProc for window {hWnd}: event 0x{event:x}", hWnd, @event);
#else
      bool isInteresting = WindowIsInteresting(hWnd);
#endif

      switch (@event) {
        case PInvoke.EVENT_OBJECT_CREATE:
        case PInvoke.EVENT_OBJECT_SHOW:
        case PInvoke.EVENT_OBJECT_UNCLOAKED:
          if (!_windows.Contains(hWnd)) {
            name = new(windowName, 0, windowNameLength);

#if DEBUG
            logger.LogDebug("Tracking window {hWnd} \"{name}\" (interesting: {isInteresting})", hWnd, name, isInteresting);
#endif

            _windows.Add(hWnd);
            windows.Add(new(hWnd, windowProcessId, name, isInteresting));
          } else {
            if (pendingRemovals.TryGetValue(hWnd, out CancellationTokenSource? cts)) {
              if (cts is not null) {
                cts.Cancel();
                pendingRemovals.Remove(hWnd);

#if DEBUG
                logger.LogTrace("Canceled removal of {hWnd}", hWnd);
#endif
              }
            } else {
              foreach (Window window in windows.Where(window => window.HWnd.Equals(hWnd))) {
                if (window.IsInteresting != isInteresting) {
#if DEBUG
                  logger.LogDebug("Window {hWnd} \"{name}\" interesting changed {oldInteresting} -> {newInteresting}", hWnd, window.Name, window.IsInteresting, isInteresting);
#endif

                  window.IsInteresting = isInteresting;
                }
              }
            }
          }

          return;

        case PInvoke.EVENT_OBJECT_DESTROY:
        case PInvoke.EVENT_OBJECT_HIDE:
        case PInvoke.EVENT_OBJECT_CLOAKED:
          ObservableCollection<Window> toRemove = [];

          if (_windows.Contains(hWnd)) {
            if (@event == PInvoke.EVENT_OBJECT_DESTROY) {
              foreach (Window window in windows.Where(window => window.HWnd.Equals(hWnd))) {
#if DEBUG
                logger.LogTrace("Pending teardown check of {hWnd} \"{name}\"", hWnd, window.Name);
#endif

                toRemove.Add(window);
              }

              foreach (Window window in toRemove) {
#if DEBUG
                ScheduleTeardownCheck(logger, windows, window);
#else
                ScheduleTeardownCheck(windows, window);
#endif
              }
            } else {
              foreach (Window window in windows.Where(window => window.HWnd.Equals(hWnd))) {
                if (window.IsInteresting != isInteresting) {
#if DEBUG
                  logger.LogDebug("Window {hWnd} \"{name}\" interesting changed {oldInteresting} -> {newInteresting}", hWnd, window.Name, window.IsInteresting, isInteresting);
#endif

                  window.IsInteresting = isInteresting;
                }
              }
            }
          }

          return;

        case PInvoke.EVENT_OBJECT_NAMECHANGE:
          name = new(windowName, 0, windowNameLength);

          if (_windows.Contains(hWnd)) {
            foreach (Window window in windows.Where(window => window.HWnd.Equals(hWnd))) {
              if (!name.Equals(window.Name)) {
#if DEBUG
                logger.LogDebug("Renaming window {hWnd} \"{oldName}\" -> \"{newName}\"", hWnd, window.Name, name);
#endif

                window.Name = name;
              }
            }
          }

          return;
      }
    }

    /// <summary>
    /// Brings <paramref name="hWnd"/> to the foreground.
    /// </summary>
#if DEBUG
    /// <param name="logger">An <see cref="ILogger"/>.</param>
#endif
    /// <param name="windows">The list of windows.</param>
    /// <param name="hWnd"><inheritdoc
    /// cref="WindowList.WinSystemEventProc"
    /// path="/param[@name='hWnd']"/></param>
#if DEBUG
    internal static void Foreground(ILogger logger, ObservableCollection<Window> windows, HWND hWnd) {
#else
    internal static void Foreground(ObservableCollection<Window> windows, HWND hWnd) {
#endif
      if (_windows.Contains(hWnd)) {
        foreach (Window window in windows.Where(window => window.HWnd.Equals(hWnd))) {
          if (!window.IsInteresting) continue;

          if (PInvoke.IsIconic(hWnd)) {
            PInvoke.ShowWindow(hWnd, SHOW_WINDOW_CMD.SW_RESTORE);
          }

#if DEBUG
          logger.LogDebug("Foregrounding: window {hWnd} \"{name}\"", hWnd, window.Name);
#endif

          PInvoke.SetForegroundWindow(hWnd);
        }
      }
    }

    /// <summary>
    /// Iconifies <paramref name="hWnd"/>.
    /// </summary>
#if DEBUG
    /// <param name="logger">An <see cref="ILogger"/>.</param>
#endif
    /// <param name="windows">The list of windows.</param>
    /// <param name="hWnd"><inheritdoc
    /// cref="WindowList.WinSystemEventProc"
    /// path="/param[@name='hWnd']"/></param>
#if DEBUG
    internal static void Iconify(ILogger logger, ObservableCollection<Window> windows, HWND hWnd) {
#else
    internal static void Iconify(ObservableCollection<Window> windows, HWND hWnd) {
#endif
      if (_windows.Contains(hWnd)) {
        foreach (Window window in windows.Where(window => window.HWnd.Equals(hWnd))) {
          if (!window.IsInteresting) continue;

          if (!PInvoke.IsIconic(hWnd)) {
#if DEBUG
            logger.LogDebug("Iconifying: window {hWnd} \"{name}\"", hWnd, window.Name);
#endif

            PInvoke.ShowWindow(hWnd, SHOW_WINDOW_CMD.SW_MINIMIZE);

            return;
          }
        }
      }
    }

    /// <summary>
    /// Determines whether a window should be included in the window list.
    /// </summary>
    /// <remarks>
    /// It is guaranteed, if the HWND refers to a window, that the HWND's name
    /// is written to <see cref="windowName"/>, the length of its name to <see
    /// cref="windowNameLength"/>, its class name to <see
    /// cref="windowClassName"/>, the length of its class name to <see
    /// cref="windowClassNameLength"/>, and its owning process ID to <see
    /// cref="windowProcessId"/>.
    /// </remarks>
#if DEBUG
    /// <param name="logger">An <see cref="ILogger"/>.</param>
#endif
    /// <param name="hWnd">The window's <see cref="HWND"/>.</param>
    /// <returns><c>true</c> if the window is interesting or <c>false</c>
    /// otherwise.</returns>
#if DEBUG
    private static bool WindowIsInteresting(ILogger logger, HWND hWnd) {
#else
    private static bool WindowIsInteresting(HWND hWnd) {
#endif
      // Skip ghosts
      if (!PInvoke.IsWindow(hWnd)) {
#if DEBUG
        logger.LogTrace("Window {hWnd} not interesting: not a window", hWnd);
#endif

        return false;
      }

      // Get window name and name length
      windowNameLength = PInvoke.GetWindowText(hWnd, windowName);

      // Get window class and class length
      windowClassNameLength = PInvoke.GetClassName(hWnd, windowClassName);

      // Get window process ID
      unsafe {
        fixed (uint* windowProcessIdPtr = &windowProcessId) {
          _ = PInvoke.GetWindowThreadProcessId(hWnd, windowProcessIdPtr);
        }
      }

      // Skip non-visible windows
      if (!PInvoke.IsWindowVisible(hWnd)) {
#if DEBUG
        logger.LogTrace("Window {hWnd} not interesting: not visible", hWnd);
#endif

        return false;
      }

      // Skip windows with no name
      if (windowName[0] == 0) {
#if DEBUG
        logger.LogTrace("Window {hWnd} not interesting: no name", hWnd);
#endif

        return false;
      }

      // Skip ignored window class names
      string name = new(windowClassName, 0, windowClassNameLength);
      if (ignoredClassNames.Contains(name)) {
#if DEBUG
        logger.LogTrace("Window {hWnd} not interesting: ignored window class", hWnd);
#endif

        return false;
      }

      // Extended styles
      long exStyle = PInvoke.GetWindowLongPtr(hWnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE).ToInt64();

      // Keep windows with WS_EX_APPWINDOW
      if ((exStyle & (int) WINDOW_EX_STYLE.WS_EX_APPWINDOW) != 0) {
#if DEBUG
        logger.LogTrace("Window {hWnd} interesting: app window", hWnd);
#endif

        return true;
      }

      // Skip tool windows (like floating palettes, popups)
      if ((exStyle & (long) WINDOW_EX_STYLE.WS_EX_TOOLWINDOW) != 0) {
#if DEBUG
        logger.LogTrace("Window {hWnd} not interesting: tool window", hWnd);
#endif

        return false;
      }

      // Skip non-activatable windows
      if ((exStyle & (long) WINDOW_EX_STYLE.WS_EX_NOACTIVATE) != 0) {
#if DEBUG
        logger.LogTrace("Window {hWnd} not interesting: not activatable", hWnd);
#endif

        return false;
      }

      // Get window styles
      long style = PInvoke.GetWindowLongPtr(hWnd, WINDOW_LONG_PTR_INDEX.GWL_STYLE).ToInt64();

      // Skip frameless popup windows
      if (
        (style & (long) WINDOW_STYLE.WS_POPUP) != 0
        && (style & (long) WINDOW_STYLE.WS_THICKFRAME) == 0
      ) {
#if DEBUG
        logger.LogTrace("Window {hWnd} not interesting: frameless popup", hWnd);
#endif

        return false;
      }

      // Skip zero-size windows (invisible or dummy handles)
      if (!PInvoke.GetWindowRect(hWnd, out RECT rect)) {
#if DEBUG
        logger.LogTrace("Window {hWnd} not interesting: GetWindowRect() failed", hWnd);
#endif

        return false;
      }

      if (rect.right - rect.left == 0 || rect.bottom - rect.top == 0) {
#if DEBUG
        logger.LogTrace("Window {hWnd} not interesting: zero-size rect", hWnd);
#endif

        return false;
      }

      // Skip cloaked windows
      if (IsCloaked(hWnd)) {
#if DEBUG
        logger.LogTrace("Window {hWnd} not interesting: cloaked", hWnd);
#endif

        return false;
      }

      // Watch out for Chromium!
      if (PInvoke.GetAncestor(hWnd, GET_ANCESTOR_FLAGS.GA_ROOT) != hWnd) {
#if DEBUG
        logger.LogTrace("Window {hWnd} not interesting: not self-rooted", hWnd);
#endif

        return false;
      }

#if DEBUG
      logger.LogTrace("{hWnd} interesting: not uninteresting", hWnd);
#endif

      return true;
    }

    /// <summary>
    /// Determines whether the window is cloaked.
    /// </summary>
    /// <param name="hWnd"><inheritdoc cref="WindowIsInteresting"
    /// path="/param[@name='hWnd']"/></param>
    /// <returns><c>true</c> if the window is cloaked or <c>false</c>
    /// otherwise.</returns>
    private static unsafe bool IsCloaked(HWND hWnd) {
      int isCloaked;
      HRESULT result = PInvoke.DwmGetWindowAttribute(hWnd, DWMWINDOWATTRIBUTE.DWMWA_CLOAKED, &isCloaked, sizeof(int));

      return result == 0 && isCloaked != 0;
    }

    /// <summary>An application-defined callback function used with the
    /// <see cref="PInvoke.EnumWindows"/> or EnumDesktopWindows function. It
    /// receives top-level window handles. The <see cref="WNDENUMPROC"/> type
    /// defines a pointer to this callback function.</summary>
    /// <param name="hWnd">A handle to a top-level window.</param>
    /// <param name="lParam">The application-defined value given in
    /// <see cref="PInvoke.EnumWindows"/> or EnumDesktopWindows.</param>
    /// <returns>To continue enumeration, the callback function must return
    /// <c>TRUE</c>; to stop enumeration, it must return
    /// <c>FALSE</c>.</returns>
    private static BOOL WndEnumProc(HWND hWnd, LPARAM lParam) {
      hWnds.Add(hWnd);
      return true;
    }

    /// <summary>
    /// Schedules a teardown check of <paramref name="window"/> from the window
    /// list.
    /// </summary>
    /// <remarks>If the teardown check passes, we schedule removal of the
    /// window.</remarks>
#if DEBUG
    /// <param name="logger">An <see cref="ILogger"/>.</param>
#endif
    /// <param name="window">A <see cref="Window"/>.</param>
#if DEBUG
    private static async void ScheduleTeardownCheck(ILogger logger, ObservableCollection<Window> windows, Window window) {
#else
    private static async void ScheduleTeardownCheck(ObservableCollection<Window> windows, Window window) {
#endif
      CancellationTokenSource cts = new();

      pendingRemovals[window.HWnd] = cts;

      try {
        await Task.Delay(50, cts.Token);

        pendingRemovals.Remove(window.HWnd);

        if (!PInvoke.IsWindow(window.HWnd)) {
#if DEBUG
          logger.LogTrace("Pending removal of {hWnd} \"{name}\"", window.HWnd, window.Name);

          ScheduleRemoval(logger, windows, window);
#else
          ScheduleRemoval(windows, window);
#endif
        } else {
#if DEBUG
          logger.LogTrace("Teardown check failed, keeping {hWnd} \"{name}\"", window.HWnd, window.Name);
#endif
        }
      } catch (TaskCanceledException) { }
    }

    /// <summary>
    /// Schedules removal of <paramref name="window"/> from the window list.
    /// </summary>
#if DEBUG
    /// <param name="logger">An <see cref="ILogger"/>.</param>
#endif
    /// <param name="window">A <see cref="Window"/>.</param>
#if DEBUG
    private static async void ScheduleRemoval(ILogger logger, ObservableCollection<Window> windows, Window window) {
#else
    private static async void ScheduleRemoval(ObservableCollection<Window> windows, Window window) {
#endif
      CancellationTokenSource cts = new();

      pendingRemovals[window.HWnd] = cts;

      try {
        await Task.Delay(50, cts.Token);

        _windows.Remove(window.HWnd);
        windows.Remove(window);
        pendingRemovals.Remove(window.HWnd);

#if DEBUG
        logger.LogDebug("No longer tracking window {hWnd} \"{name}\"", window.HWnd, window.Name);
#endif
      } catch (TaskCanceledException) { }
    }
  }
}