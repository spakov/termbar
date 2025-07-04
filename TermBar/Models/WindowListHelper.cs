using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Dwm;
using Windows.Win32.UI.WindowsAndMessaging;

namespace TermBar.Models {
  /// <summary>
  /// Helper methods abstracting Win32 for <see cref="WindowList"/>.
  /// </summary>
  internal static class WindowListHelper {
    internal const uint EVENT_SYSTEM_FOREGROUND = 0x0003;

    internal const uint EVENT_OBJECT_CREATE = 0x8000;
    internal const uint EVENT_OBJECT_DESTROY = 0x8001;
    internal const uint EVENT_OBJECT_SHOW = 0x8002;
    internal const uint EVENT_OBJECT_HIDE = 0x8003;
    internal const uint EVENT_OBJECT_NAMECHANGE = 0x800C;

    internal const uint WINEVENT_OUTOFCONTEXT = 0x0000;
    internal const uint WINEVENT_SKIPOWNTHREAD = 0x0001;
    internal const uint WINEVENT_SKIPOWNPROCESS = 0x0002;
    internal const uint WINEVENT_INCONTEXT = 0x0004;

    private static WNDENUMPROC? wndEnumProc;

    private static readonly List<HWND> hWnds = [];

    private static int windowNameLength;
    private static readonly char[] windowName = new char[256];

#pragma warning disable IDE0044 // Add readonly modifier
    private static uint windowProcessId;
#pragma warning restore IDE0044 // Add readonly modifier

    private static readonly HashSet<HWND> _windows = [];

    /// <summary>
    /// Returns the windows to be managed.
    /// </summary>
    /// <remarks>Call <see cref="UpdateWindow"/> to keep the window list in
    /// sync.</remarks>
    /// <returns>An <see cref="ObservableCollection{T}"/> of
    /// <see cref="Window"/>s representing the windows to be managed.</returns>
    internal static ObservableCollection<Window> EnumerateWindows() {
      ObservableCollection<Window> windows = [];

      wndEnumProc = new(WndEnumProc);

      PInvoke.EnumWindows(wndEnumProc, 0);

      foreach (HWND hWnd in hWnds) {
        if (!WindowIsInteresting(hWnd)) continue;

        string name = new(windowName, 0, windowNameLength);

        _windows.Add(hWnd);
        windows.Add(new(hWnd, windowProcessId, name));
      }

      return windows;
    }

    /// <summary>
    /// Determines whether a window being tracked has been foregrounded based
    /// on <paramref name="event"/>.
    /// </summary>
    /// <param name="windows">The list of windows.</param>
    /// <param name="event"><inheritdoc cref="WindowList.WinSystemEventProc"
    /// path="/param[@name='event']"/></param>
    /// <param name="hWnd"><inheritdoc cref="WindowList.WinSystemEventProc"
    /// path="/param[@name='hWnd']"/></param>
    internal static Window? IsForegrounded(ObservableCollection<Window> windows, uint @event, HWND hWnd) {
      if (@event == EVENT_SYSTEM_FOREGROUND) {
        if (!WindowIsInteresting(hWnd)) return null;

        if (_windows.Contains(hWnd)) {
          foreach (Window window in windows.Where(window => window.HWnd.Equals(hWnd))) {
            //Debug.WriteLine($"Foregrounding {hWnd}");
            return window;
          }
        }
      }

      return null;
    }

    /// <summary>
    /// Updates the list of windows based on <paramref name="event"/>.
    /// </summary>
    /// <param name="windows">The list of windows.</param>
    /// <param name="event"><inheritdoc
    /// cref="WindowList.WinObjectEventProc"
    /// path="/param[@name='event']"/></param>
    /// <param name="hWnd"><inheritdoc
    /// cref="WindowList.WinObjectEventProc"
    /// path="/param[@name='hWnd']"/></param>
    internal static void UpdateWindow(ObservableCollection<Window> windows, uint @event, HWND hWnd) {
      string name;

      switch (@event) {
        case EVENT_OBJECT_CREATE:
        case EVENT_OBJECT_SHOW:
          if (!WindowIsInteresting(hWnd)) return;

          if (!_windows.Contains(hWnd)) {
            name = new(windowName, 0, windowNameLength);

            _windows.Add(hWnd);
            windows.Add(new(hWnd, windowProcessId, name));
            //Debug.WriteLine($"Adding {name} ({hWnd})");
          }

          return;

        case EVENT_OBJECT_DESTROY:
        case EVENT_OBJECT_HIDE:
          if (WindowIsInteresting(hWnd)) return;

          ObservableCollection<Window> toRemove = [];

          if (_windows.Contains(hWnd)) {
            foreach (Window window in windows.Where(window => window.HWnd.Equals(hWnd))) {
              toRemove.Add(window);
              //Debug.WriteLine($"Removing {hWnd}");
            }

            _windows.Remove(hWnd);
            foreach (Window window in toRemove) windows.Remove(window);
          }

          return;

        case EVENT_OBJECT_NAMECHANGE:
          if (!WindowIsInteresting(hWnd)) return;

          name = new(windowName, 0, windowNameLength);

          if (_windows.Contains(hWnd)) {
            foreach (Window window in windows.Where(window => window.HWnd.Equals(hWnd))) {
              if (!name.Equals(window.Name)) {
                window.Name = name;
                //Debug.WriteLine($"Renamed {name} ({hWnd})");
              }
            }
          }

          return;
      }
    }

    /// <summary>
    /// Brings <paramref name="hWnd"/> to the foreground.
    /// </summary>
    /// <param name="windows">The list of windows.</param>
    /// <param name="hWnd"><inheritdoc
    /// cref="WindowList.WinSystemEventProc"
    /// path="/param[@name='hWnd']"/></param>
    internal static void Foreground(ObservableCollection<Window> windows, HWND hWnd) {
      if (_windows.Contains(hWnd)) {
        foreach (Window window in windows.Where(window => window.HWnd.Equals(hWnd))) {
          if (PInvoke.IsIconic(hWnd)) {
            PInvoke.ShowWindow(hWnd, SHOW_WINDOW_CMD.SW_RESTORE);
          }

          PInvoke.SetForegroundWindow(hWnd);

          //Debug.WriteLine($"Foregrounded {hWnd}");
        }
      }
    }

    /// <summary>
    /// Iconifies <paramref name="hWnd"/>.
    /// </summary>
    /// <param name="windows">The list of windows.</param>
    /// <param name="hWnd"><inheritdoc
    /// cref="WindowList.WinSystemEventProc"
    /// path="/param[@name='hWnd']"/></param>
    internal static void Iconify(ObservableCollection<Window> windows, HWND hWnd) {
      if (_windows.Contains(hWnd)) {
        foreach (Window window in windows.Where(window => window.HWnd.Equals(hWnd))) {
          if (!PInvoke.IsIconic(hWnd)) {
            PInvoke.ShowWindow(hWnd, SHOW_WINDOW_CMD.SW_MINIMIZE);
          }

          //Debug.WriteLine($"Iconified {hWnd}");
        }
      }
    }

    /// <summary>
    /// Determines whether a window should be included in the window list.
    /// </summary>
    /// <remarks>
    /// If the window is interesting, writes its name to <see
    /// cref="windowName"/>, the length of its name to <see
    /// cref="windowNameLength"/>, and its owning process ID to <see
    /// cref="windowProcessId"/>.
    /// </remarks>
    /// <param name="hWnd">The window's <see cref="HWND"/>.</param>
    /// <returns><c>true</c> if the window is interesting or <c>false</c>
    /// otherwise.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Impacts readability")]
    private static bool WindowIsInteresting(HWND hWnd) {
      if (!PInvoke.IsWindow(hWnd) || !PInvoke.IsWindowVisible(hWnd)) return false;

      // Extended styles
      int exStyle = PInvoke.GetWindowLong(hWnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);

      // Skip child or owned windows (unless they are explicitly top-level)
      if (PInvoke.GetWindow(hWnd, GET_WINDOW_CMD.GW_OWNER) != HWND.Null) {
        // But keep windows that explicitly ask to be on the taskbar
        if ((exStyle & (int) WINDOW_EX_STYLE.WS_EX_APPWINDOW) == 0) return true;
      }

      // Get window name and name length
      windowNameLength = PInvoke.GetWindowText(hWnd, windowName);

      // Skip windows with no name
      if (windowName[0] == 0) return false;

      // Get window process ID
      unsafe {
        fixed (uint* windowProcessIdPtr = &windowProcessId) {
          _ = PInvoke.GetWindowThreadProcessId(hWnd, windowProcessIdPtr);
        }
      }

      // Skip tool windows (like floating palettes, popups)
      if ((exStyle & (int) WINDOW_EX_STYLE.WS_EX_TOOLWINDOW) != 0) return false;

      // Skip non-activatable windows
      if ((exStyle & (int) WINDOW_EX_STYLE.WS_EX_NOACTIVATE) != 0) {
        // But keep windows that explicitly ask to be on the taskbar
        if ((exStyle & (int) WINDOW_EX_STYLE.WS_EX_APPWINDOW) == 0) return true;
      }

      // Get window styles
      int style = PInvoke.GetWindowLong(hWnd, WINDOW_LONG_PTR_INDEX.GWL_STYLE);

      // Must be a top-level overlapped window
      if ((style & (int) WINDOW_STYLE.WS_CHILD) != 0) return false;

      // Skip zero-size windows (invisible or dummy handles)
      if (!PInvoke.GetWindowRect(hWnd, out RECT rect)) return false;
      if (rect.right - rect.left == 0 || rect.bottom - rect.top == 0) return false;

      // Skip cloaked windows
      if (IsCloaked(hWnd)) return false;

      // Watch out for Chromium!
      if (PInvoke.GetAncestor(hWnd, GET_ANCESTOR_FLAGS.GA_ROOT) != hWnd) return false;

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
  }
}
