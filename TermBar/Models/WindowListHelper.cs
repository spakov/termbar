using Microsoft.Extensions.Logging;
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

namespace Spakov.TermBar.Models
{
    /// <summary>
    /// Helper methods abstracting Win32 for <see cref="WindowList"/>.
    /// </summary>
    internal static class WindowListHelper
    {
        /// <summary>
        /// The window class of a WinUI 3 popup.
        /// </summary>
        private const string PopupClass = "Microsoft.UI.Content.PopupWindowSiteBridge";

        /// <summary>
        /// Window classes to mark as uninteresting.
        /// </summary>
        private static readonly List<string> s_ignoredClassNames =
        [
            PopupClass,
            "Progman",
            "PseudoConsoleWindow",
            "Tooltips_Class32",
            "WorkerW",
            "Xaml_WindowedPopupClass"
        ];

        private static WNDENUMPROC? s_wndEnumProc;

        private static readonly HashSet<HWND> s_hWnds = [];
        private static readonly Dictionary<HWND, CancellationTokenSource> s_pendingRemovals = [];

        private static readonly int s_termBarProcessId = Environment.ProcessId;
        private static readonly HashSet<HWND> s_popupHWnds = [];

        private static int s_windowNameLength;
        private static readonly char[] s_windowName = new char[256];

        private static int s_windowClassNameLength;
        private static readonly char[] s_windowClassName = new char[256];

#pragma warning disable IDE0044 // Add readonly modifier
        private static uint s_windowProcessId;
#pragma warning restore IDE0044 // Add readonly modifier

        private static readonly HashSet<HWND> s_windows = [];

        /// <summary>
        /// Whether a TermBar popup is currently displayed.
        /// </summary>
        internal static bool IsPopupOpen => s_popupHWnds.Count != 0;

        /// <summary>
        /// Returns the windows to be managed.
        /// </summary>
        /// <remarks>
        /// <para>Intended to be invoked once to gather a list of windows that
        /// exist when starting up. After that, call <see cref="UpdateWindow"/>
        /// to keep the window list in sync.</para>
        /// </remarks>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        /// <returns>An <see cref="ObservableCollection{T}"/> of
        /// <see cref="Window"/>s representing the windows to be
        /// managed.</returns>
        internal static ObservableCollection<Window> EnumerateWindows(ILogger? logger = null)
        {
            ObservableCollection<Window> windows = [];

            s_wndEnumProc = new(WndEnumProc);

            PInvoke.EnumWindows(s_wndEnumProc, 0);

            foreach (HWND hWnd in s_hWnds)
            {
                bool isInteresting = WindowIsInteresting(hWnd, logger);

                string name = new(s_windowName, 0, s_windowNameLength);

                s_windows.Add(hWnd);
                windows.Add(new(hWnd, s_windowProcessId, name, isInteresting));

                logger?.LogDebug("Tracking window {hWnd} \"{name}\" (interesting: {isInteresting})", hWnd, name, isInteresting);
            }

            return windows;
        }

        /// <summary>
        /// Determines the correct order in which <paramref name="window"/>
        /// should appear in <paramref name="windowList"/> and inserts it at
        /// the proper index.
        /// </summary>
        /// <typeparam name="T">A type implementing <see
        /// cref="IWindowListWindow"/>, representing a tracked and interesting
        /// window.</typeparam>
        /// <param name="config">A <see cref="Configuration.Json.WindowList"/>
        /// configuration.</param>
        /// <param name="window">The window to be added.</param>
        /// <param name="windowList">The window list.</param>
        /// <param name="processId">The window's process ID.</param>
        /// <param name="name">The window's name.</param>
        internal static void OrderAndInsert<T>(Configuration.Json.WindowList config, T window, ObservableCollection<T> windowList, uint processId, string? name) where T : IWindowListWindow
        {
            Range highPriorityWindows = new(-1, -1);
            Range normalPriorityWindows = new(-1, -1);
            Range lowPriorityWindows = new(-1, -1);

            // Adjust ranges
            for (int i = 0; i < windowList.Count; i++)
            {
                using Process process = Process.GetProcessById((int)windowList[i].WindowProcessId);

                if (config.HighPriorityWindows is not null)
                {
                    if (config.HighPriorityWindows.Contains(process.ProcessName))
                    {
                        if (highPriorityWindows.Low < 0)
                        {
                            highPriorityWindows.Low = i;
                        }
                    }
                    else
                    {
                        if (normalPriorityWindows.Low < 0)
                        {
                            normalPriorityWindows.Low = i;
                        }
                    }
                }
                else
                {
                    if (normalPriorityWindows.Low < 0)
                    {
                        normalPriorityWindows.Low = i;
                    }
                }

                if (config.LowPriorityWindows is not null)
                {
                    if (config.LowPriorityWindows.Contains(process.ProcessName))
                    {
                        if (lowPriorityWindows.Low < 0)
                        {
                            lowPriorityWindows.Low = i;
                        }
                    }
                }
            }

            if (lowPriorityWindows.High < 0 && lowPriorityWindows.Low >= 0)
            {
                lowPriorityWindows.High = windowList.Count - 1;
            }

            if (normalPriorityWindows.High < 0 && normalPriorityWindows.Low >= 0)
            {
                normalPriorityWindows.High = lowPriorityWindows.Low >= 0
                    ? lowPriorityWindows.Low - 1
                    : windowList.Count - 1;
            }

            if (highPriorityWindows.High < 0 && highPriorityWindows.Low >= 0)
            {
                highPriorityWindows.High = normalPriorityWindows.Low >= 0
                    ? normalPriorityWindows.Low - 1
                    : windowList.Count - 1;
            }

            Range insertionRange = normalPriorityWindows;

            string processName;
            using (Process process = Process.GetProcessById((int)processId))
            {
                processName = process.ProcessName;
            }

            // Determine priority
            if (config.LowPriorityWindows is not null)
            {
                if (config.LowPriorityWindows.Contains(processName))
                {
                    insertionRange = lowPriorityWindows;
                }
            }

            if (config.HighPriorityWindows is not null)
            {
                if (config.HighPriorityWindows.Contains(processName))
                {
                    insertionRange = highPriorityWindows;
                }
            }

            // If we have no insertion range at this point, our list is empty
            if (insertionRange.Low < 0 && insertionRange.High < 0)
            {
                windowList.Add(window);

                return;
            }

            int groupLowIndex = -1;

            // Determine grouping
            for (int i = insertionRange.Low; i <= insertionRange.High; i++)
            {
                using Process process = Process.GetProcessById((int)windowList[i].WindowProcessId);

                if (process.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase))
                {
                    groupLowIndex = i;
                    break;
                }
            }

            if (groupLowIndex >= 0)
            {
                insertionRange.Low = groupLowIndex;
            }

            int insertionIndex = -1;

            // Check for group alphabetic sorting
            if (config.SortGroupsAlphabetically && groupLowIndex >= 0)
            {
                for (int i = insertionRange.Low; i <= insertionRange.High; i++)
                {
                    using Process process = Process.GetProcessById((int)windowList[i].WindowProcessId);

                    if (!process.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase))
                    {
                        insertionIndex = i;
                        break;
                    }
                    else
                    {
                        if (string.Compare(windowList[i].WindowName, name, StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            insertionIndex = i;
                            break;
                        }
                    }
                }
            }

            // Determine the insertion index
            if (insertionIndex < 0)
            {
                for (int i = insertionRange.Low; i <= insertionRange.High; i++)
                {
                    if (config.HighPriorityWindows is not null)
                    {
                        if (config.HighPriorityWindows.Contains(processName))
                        {
                            using Process process = Process.GetProcessById((int)windowList[i].WindowProcessId);

                            if (config.HighPriorityWindows.IndexOf(process.ProcessName) > config.HighPriorityWindows.IndexOf(processName))
                            {
                                insertionIndex = i;
                                break;
                            }
                        }
                    }

                    if (config.LowPriorityWindows is not null)
                    {
                        if (config.LowPriorityWindows.Contains(processName))
                        {
                            using Process process = Process.GetProcessById((int)windowList[i].WindowProcessId);

                            if (config.LowPriorityWindows.IndexOf(process.ProcessName) > config.LowPriorityWindows.IndexOf(processName))
                            {
                                insertionIndex = i;
                                break;
                            }
                        }
                    }
                }

                if (insertionIndex < 0)
                {
                    insertionIndex = insertionRange.High + 1;
                }
            }

            windowList.Insert(insertionIndex, window);

            return;
        }

        /// <summary>
        /// Determines whether a window has been foregrounded based on
        /// <paramref name="event"/>.
        /// </summary>
        /// <param name="windows">The list of windows.</param>
        /// <param name="event"><inheritdoc
        /// cref="WindowList.WinSystemEventProc"
        /// path="/param[@name='event']"/></param>
        /// <param name="hWnd"><inheritdoc cref="WindowList.WinSystemEventProc"
        /// path="/param[@name='hWnd']"/></param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        internal static Window? IsForegrounded(ObservableCollection<Window> windows, uint @event, HWND hWnd, ILogger? logger = null)
        {
            if (@event == PInvoke.EVENT_SYSTEM_FOREGROUND)
            {
                if (s_windows.Contains(hWnd))
                {
                    foreach (Window window in windows.Where(window => window.HWnd.Equals(hWnd)))
                    {
                        logger?.LogDebug("Foregrounded: window {hWnd} \"{name}\" (interesting: {isInteresting})", hWnd, window.Name, window.IsInteresting);

                        return window;
                    }
                }
                else
                {
                    logger?.LogDebug("Foregrounded: untracked window {hWnd}, will track it", hWnd);

                    string name = new(s_windowName, 0, s_windowNameLength);
                    bool isInteresting = WindowIsInteresting(hWnd, logger);

                    logger?.LogDebug("Tracking window {hWnd} \"{name}\" (interesting: {isInteresting})", hWnd, name, isInteresting);

                    Window window = new(hWnd, s_windowProcessId, name, isInteresting);

                    s_windows.Add(hWnd);
                    windows.Add(window);

                    return window;
                }
            }

            logger?.LogWarning("Foregrounded: unknown window {hWnd}", hWnd);

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
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        internal static void UpdateWindow(ObservableCollection<Window> windows, uint @event, HWND hWnd, ILogger? logger = null)
        {
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
            )
            {
                return;
            }

            string name;
            bool isInteresting = WindowIsInteresting(hWnd, logger);

            logger?.LogTrace("WinObjectEventProc for window {hWnd}: event 0x{event:x}", hWnd, @event);

            if (@event is PInvoke.EVENT_OBJECT_CREATE or PInvoke.EVENT_OBJECT_SHOW)
            {
                name = new(s_windowClassName, 0, s_windowClassNameLength);

                if (name == PopupClass && s_windowProcessId == s_termBarProcessId)
                {
                    s_popupHWnds.Add(hWnd);
                }
            }
            else if (@event is PInvoke.EVENT_OBJECT_DESTROY or PInvoke.EVENT_OBJECT_HIDE)
            {
                s_popupHWnds.Remove(hWnd);
            }

            switch (@event)
            {
                case PInvoke.EVENT_OBJECT_CREATE:
                case PInvoke.EVENT_OBJECT_SHOW:
                case PInvoke.EVENT_OBJECT_UNCLOAKED:
                    if (!s_windows.Contains(hWnd))
                    {
                        name = new(s_windowName, 0, s_windowNameLength);

                        logger?.LogDebug("Tracking window {hWnd} \"{name}\" (interesting: {isInteresting})", hWnd, name, isInteresting);
                        s_windows.Add(hWnd);
                        windows.Add(new(hWnd, s_windowProcessId, name, isInteresting));
                    }
                    else
                    {
                        if (s_pendingRemovals.TryGetValue(hWnd, out CancellationTokenSource? cts))
                        {
                            if (cts is not null)
                            {
                                cts.Cancel();
                                s_pendingRemovals.Remove(hWnd);
                                logger?.LogTrace("Canceled removal of {hWnd}", hWnd);
                            }
                        }
                        else
                        {
                            foreach (Window window in windows.Where(window => window.HWnd.Equals(hWnd)))
                            {
                                if (window.IsInteresting != isInteresting)
                                {
                                    logger?.LogDebug("Window {hWnd} \"{name}\" interesting changed {oldInteresting} -> {newInteresting}", hWnd, window.Name, window.IsInteresting, isInteresting);
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

                    if (s_windows.Contains(hWnd))
                    {
                        if (@event == PInvoke.EVENT_OBJECT_DESTROY)
                        {
                            foreach (Window window in windows.Where(window => window.HWnd.Equals(hWnd)))
                            {
                                logger?.LogTrace("Pending teardown check of {hWnd} \"{name}\"", hWnd, window.Name);
                                toRemove.Add(window);
                            }

                            foreach (Window window in toRemove)
                            {
                                ScheduleTeardownCheck(windows, window, logger);
                            }
                        }
                        else
                        {
                            foreach (Window window in windows.Where(window => window.HWnd.Equals(hWnd)))
                            {
                                if (window.IsInteresting != isInteresting)
                                {
                                    logger?.LogDebug("Window {hWnd} \"{name}\" interesting changed {oldInteresting} -> {newInteresting}", hWnd, window.Name, window.IsInteresting, isInteresting);
                                    window.IsInteresting = isInteresting;
                                }
                            }
                        }
                    }

                    return;

                case PInvoke.EVENT_OBJECT_NAMECHANGE:
                    name = new(s_windowName, 0, s_windowNameLength);

                    if (s_windows.Contains(hWnd))
                    {
                        foreach (Window window in windows.Where(window => window.HWnd.Equals(hWnd)))
                        {
                            if (!name.Equals(window.Name))
                            {
                                logger?.LogDebug("Renaming window {hWnd} \"{oldName}\" -> \"{newName}\"", hWnd, window.Name, name);
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
        /// <param name="windows">The list of windows.</param>
        /// <param name="hWnd"><inheritdoc
        /// cref="WindowList.WinSystemEventProc"
        /// path="/param[@name='hWnd']"/></param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        internal static void Foreground(ObservableCollection<Window> windows, HWND hWnd, ILogger? logger = null)
        {
            if (s_windows.Contains(hWnd))
            {
                foreach (Window window in windows.Where(window => window.HWnd.Equals(hWnd)))
                {
                    if (!window.IsInteresting)
                    {
                        continue;
                    }

                    if (PInvoke.IsIconic(hWnd))
                    {
                        PInvoke.ShowWindow(hWnd, SHOW_WINDOW_CMD.SW_RESTORE);
                    }

                    logger?.LogDebug("Foregrounding: window {hWnd} \"{name}\"", hWnd, window.Name);
                    PInvoke.SetForegroundWindow(hWnd);
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
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        internal static void Iconify(ObservableCollection<Window> windows, HWND hWnd, ILogger? logger = null)
        {
            if (s_windows.Contains(hWnd))
            {
                foreach (Window window in windows.Where(window => window.HWnd.Equals(hWnd)))
                {
                    if (!window.IsInteresting)
                    {
                        continue;
                    }

                    if (!PInvoke.IsIconic(hWnd))
                    {
                        logger?.LogDebug("Iconifying: window {hWnd} \"{name}\"", hWnd, window.Name);
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
        /// <para>This is a very important method in that it is the core of the
        /// determination as to which windows are "interesting" enough to be
        /// visually tracked. This is done via a variety of means that were
        /// difficult to determine and (hopefully) roughly match the way the
        /// Windows taskbar determines which windows to track.</para>
        /// <para>It is guaranteed, if the HWND refers to a window, that the
        /// HWND's name is written to <see cref="s_windowName"/>, the length of
        /// its name to <see cref="s_windowNameLength"/>, its class name to
        /// <see cref="s_windowClassName"/>, the length of its class name to
        /// <see cref="s_windowClassNameLength"/>, and its owning process ID to
        /// <see cref="s_windowProcessId"/>.</para>
        /// </remarks>
        /// <param name="hWnd">The window's <see cref="HWND"/>.</param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        /// <returns><see langword="true"/> if the window is interesting or
        /// <see langword="false"/> otherwise.</returns>
        private static bool WindowIsInteresting(HWND hWnd, ILogger? logger = null)
        {
            // Skip ghosts
            if (!PInvoke.IsWindow(hWnd))
            {
                logger?.LogTrace("Window {hWnd} not interesting: not a window", hWnd);
                return false;
            }

            // Get window name and name length
            s_windowNameLength = PInvoke.GetWindowText(hWnd, s_windowName);

            // Get window class and class length
            s_windowClassNameLength = PInvoke.GetClassName(hWnd, s_windowClassName);

            // Get window process ID
            unsafe
            {
                fixed (uint* windowProcessIdPtr = &s_windowProcessId)
                {
                    _ = PInvoke.GetWindowThreadProcessId(hWnd, windowProcessIdPtr);
                }
            }

            // Skip non-visible windows
            if (!PInvoke.IsWindowVisible(hWnd))
            {
                logger?.LogTrace("Window {hWnd} not interesting: not visible", hWnd);
                return false;
            }

            // Skip windows with no name
            if (s_windowName[0] == 0)
            {
                logger?.LogTrace("Window {hWnd} not interesting: no name", hWnd);
                return false;
            }

            // Skip ignored window class names
            string name = new(s_windowClassName, 0, s_windowClassNameLength);
            if (s_ignoredClassNames.Contains(name))
            {
                logger?.LogTrace("Window {hWnd} not interesting: ignored window class", hWnd);
                return false;
            }

            // Extended styles
            long exStyle = PInvoke.GetWindowLongPtr(hWnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE).ToInt64();

            // Keep windows with WS_EX_APPWINDOW
            if ((exStyle & (int)WINDOW_EX_STYLE.WS_EX_APPWINDOW) != 0)
            {
                logger?.LogTrace("Window {hWnd} interesting: app window", hWnd);
                return true;
            }

            // Skip tool windows (like floating palettes, popups)
            if ((exStyle & (long)WINDOW_EX_STYLE.WS_EX_TOOLWINDOW) != 0)
            {
                logger?.LogTrace("Window {hWnd} not interesting: tool window", hWnd);
                return false;
            }

            // Skip non-activatable windows
            if ((exStyle & (long)WINDOW_EX_STYLE.WS_EX_NOACTIVATE) != 0)
            {
                logger?.LogTrace("Window {hWnd} not interesting: not activatable", hWnd);
                return false;
            }

            // Get window styles
            long style = PInvoke.GetWindowLongPtr(hWnd, WINDOW_LONG_PTR_INDEX.GWL_STYLE).ToInt64();

            // Skip frameless popup windows
            if (
              (style & (long)WINDOW_STYLE.WS_POPUP) != 0
              && (style & (long)WINDOW_STYLE.WS_THICKFRAME) == 0
            )
            {
                logger?.LogTrace("Window {hWnd} not interesting: frameless popup", hWnd);
                return false;
            }

            // Skip zero-size windows (invisible or dummy handles)
            if (!PInvoke.GetWindowRect(hWnd, out RECT rect))
            {
                logger?.LogTrace("Window {hWnd} not interesting: GetWindowRect() failed", hWnd);
                return false;
            }

            if (rect.right - rect.left == 0 || rect.bottom - rect.top == 0)
            {
                logger?.LogTrace("Window {hWnd} not interesting: zero-size rect", hWnd);
                return false;
            }

            // Skip cloaked windows
            if (IsCloaked(hWnd))
            {
                logger?.LogTrace("Window {hWnd} not interesting: cloaked", hWnd);
                return false;
            }

            // Watch out for Chromium!
            if (PInvoke.GetAncestor(hWnd, GET_ANCESTOR_FLAGS.GA_ROOT) != hWnd)
            {
                logger?.LogTrace("Window {hWnd} not interesting: not self-rooted", hWnd);
                return false;
            }

            logger?.LogTrace("{hWnd} interesting: not uninteresting", hWnd);
            return true;
        }

        /// <summary>
        /// Determines whether the window is cloaked.
        /// </summary>
        /// <param name="hWnd"><inheritdoc cref="WindowIsInteresting"
        /// path="/param[@name='hWnd']"/></param>
        /// <returns><c>true</c> if the window is cloaked or <c>false</c>
        /// otherwise.</returns>
        private static unsafe bool IsCloaked(HWND hWnd)
        {
            int isCloaked;
            HRESULT result = PInvoke.DwmGetWindowAttribute(hWnd, DWMWINDOWATTRIBUTE.DWMWA_CLOAKED, &isCloaked, sizeof(int));

            return result == 0 && isCloaked != 0;
        }

        /// <summary>An application-defined callback function used with the
        /// <see cref="PInvoke.EnumWindows"/> or EnumDesktopWindows function.
        /// It receives top-level window handles. The <see cref="WNDENUMPROC"/>
        /// type defines a pointer to this callback function.</summary>
        /// <param name="hWnd">A handle to a top-level window.</param>
        /// <param name="lParam">The application-defined value given in
        /// <see cref="PInvoke.EnumWindows"/> or EnumDesktopWindows.</param>
        /// <returns>To continue enumeration, the callback function must return
        /// <c>TRUE</c>; to stop enumeration, it must return
        /// <c>FALSE</c>.</returns>
        private static BOOL WndEnumProc(HWND hWnd, LPARAM lParam)
        {
            s_hWnds.Add(hWnd);
            return true;
        }

        /// <summary>
        /// Schedules a teardown check of <paramref name="window"/> from the
        /// window list.
        /// </summary>
        /// <remarks>If the teardown check passes, we schedule removal of the
        /// window.</remarks>
        /// <param name="windows">The list of windows.</param>
        /// <param name="window">A <see cref="Window"/>.</param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        private static async void ScheduleTeardownCheck(ObservableCollection<Window> windows, Window window, ILogger? logger = null)
        {
            CancellationTokenSource cts = new();

            s_pendingRemovals[window.HWnd] = cts;

            try
            {
                await Task.Delay(50, cts.Token);

                s_pendingRemovals.Remove(window.HWnd);

                if (!PInvoke.IsWindow(window.HWnd))
                {
                    logger?.LogTrace("Pending removal of {hWnd} \"{name}\"", window.HWnd, window.Name);
                    ScheduleRemoval(windows, window, logger);
                }
                else
                {
                    logger?.LogTrace("Teardown check failed, keeping {hWnd} \"{name}\"", window.HWnd, window.Name);
                }
            }
            catch (TaskCanceledException) {
            }
        }

        /// <summary>
        /// Schedules removal of <paramref name="window"/> from the window
        /// list.
        /// </summary>
        /// <param name="windows">The list of windows.</param>
        /// <param name="window">A <see cref="Window"/>.</param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        private static async void ScheduleRemoval(ObservableCollection<Window> windows, Window window, ILogger? logger = null)
        {
            CancellationTokenSource cts = new();

            s_pendingRemovals[window.HWnd] = cts;

            try
            {
                await Task.Delay(50, cts.Token);

                s_windows.Remove(window.HWnd);
                windows.Remove(window);
                s_pendingRemovals.Remove(window.HWnd);
                logger?.LogDebug("No longer tracking window {hWnd} \"{name}\"", window.HWnd, window.Name);
            }
            catch (TaskCanceledException) {
            }
        }
    }
}