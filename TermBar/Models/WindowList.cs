using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Accessibility;

namespace Spakov.TermBar.Models
{
    /// <summary>
    /// The window list.
    /// </summary>
    /// <remarks>
    /// <para>This is the object that is managed by <see
    /// cref="WindowListHelper"/>. It is responsible for bridging the gap
    /// between Win32 interfaces and the presentation layer. This class is
    /// responsible for handling callbacks that provide window-related
    /// notifications.</para>
    /// <para>Intended to be interfaced with via <see cref="Windows"/>'s <see
    /// cref="System.Collections.Specialized.INotifyCollectionChanged"/> and
    /// <see cref="INotifyPropertyChanged"/>, for <see
    /// cref="ForegroundWindow"/>.</para>
    /// </remarks>
#pragma warning disable IDE0079 // Remove unnecessary suppression
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CsWinRT1028:Class is not marked partial", Justification = "Is a model")]
#pragma warning restore IDE0079 // Remove unnecessary suppression
    public class WindowList : INotifyPropertyChanged
    {
        internal ILogger? logger;

        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly DispatcherQueue _dispatcherQueue;

        private readonly WINEVENTPROC _winSystemEventProc;
        private readonly WINEVENTPROC _winObjectEventProc;

        private readonly ObservableCollection<Window> _windows;

        private static Window? s_foregroundWindow;

        private static WindowList? s_instance;

        /// <summary>
        /// The singleton instance.
        /// </summary>
        public static WindowList? Instance
        {
            get => s_instance;
            internal set => s_instance = value;
        }

        /// <summary>
        /// The list of <see cref="Window"/>s to be presented to the
        /// user.
        /// </summary>
        public static ObservableCollection<Window> Windows => Instance!._windows;

        /// <inheritdoc cref="WindowListHelper.IsPopupOpen"/>
        public static bool IsPopupOpen => WindowListHelper.IsPopupOpen;

        /// <summary>
        /// The currently foregrounded window.
        /// </summary>
        public static Window? ForegroundWindow
        {
            get => s_foregroundWindow;

            set
            {
                if (s_foregroundWindow != value)
                {
                    if (value is not null)
                    {
                        WindowListHelper.Foreground(Instance!._windows, value.HWnd, Instance!.logger);
                    }

                    Instance?.logger?.LogDebug("ForegroundWindow: {oldHWnd} \"{oldName}\" -> {newHWnd} \"{newName}\"", s_foregroundWindow?.HWnd, s_foregroundWindow?.Name, value?.HWnd, value?.Name);
                    s_foregroundWindow = value;

                    Instance!.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Iconifies the foregrounded window.
        /// </summary>
        public static void Iconify()
        {
            if (s_foregroundWindow is not null)
            {
                WindowListHelper.Iconify(Instance!._windows, s_foregroundWindow.HWnd, Instance!.logger);
            }
        }

        /// <summary>
        /// Initializes a <see cref="WindowList"/>.
        /// </summary>
        internal WindowList(DispatcherQueue dispatcherQueue)
        {
            logger = LoggerHelper.CreateLogger<WindowList>();

            _dispatcherQueue = dispatcherQueue;
            _windows = WindowListHelper.EnumerateWindows(logger);

            HWND foregroundHWnd = PInvoke.GetForegroundWindow();

            foreach (Window window in _windows.Where(window => window.HWnd.Equals(foregroundHWnd)))
            {
                s_foregroundWindow = window;
            }

            _winSystemEventProc = new(WinSystemEventProc);
            _winObjectEventProc = new(WinObjectEventProc);

            _ = PInvoke.SetWinEventHook(
                PInvoke.EVENT_SYSTEM_FOREGROUND,
                PInvoke.EVENT_SYSTEM_FOREGROUND,
                (HMODULE)(nint)0,
                _winSystemEventProc,
                0,
                0,
                PInvoke.WINEVENT_OUTOFCONTEXT
            );

            _ = PInvoke.SetWinEventHook(
                PInvoke.EVENT_OBJECT_CREATE,
                PInvoke.EVENT_OBJECT_UNCLOAKED,
                (HMODULE)(nint)0,
                _winObjectEventProc,
                0,
                0,
                PInvoke.WINEVENT_OUTOFCONTEXT
            );
        }

        /// <summary>
        /// Handles event <c>EVENT_SYSTEM_FOREGROUND</c>.
        /// </summary>
        /// <remarks>
        /// <para>An application-defined callback (or hook) function that the
        /// system calls in response to events generated by an accessible
        /// object. The hook function processes the event notifications as
        /// required. Clients install the hook function and request specific
        /// types of event notifications by calling <see
        /// cref="PInvoke.SetWinEventHook"/>.</para>
        /// <para>The <see cref="WINEVENTPROC"/> type defines a pointer to this
        /// callback function. <c>WinEventProc</c> is a placeholder for the
        /// application-defined function name.</para>
        /// </remarks>
        /// <param name="hWinEventHook">Handle to an event hook function. This
        /// value is returned by <see cref="PInvoke.SetWinEventHook"/> when the
        /// hook function is installed and is specific to each instance of the
        /// hook function.</param>
        /// <param name="event">Specifies the event that occurred. This value
        /// is one of the event constants.</param>
        /// <param name="hWnd">Handle to the window that generates the event,
        /// or <c>null</c> if no window is associated with the event. For
        /// example, the mouse pointer is not associated with a window.</param>
        /// <param name="idObject">Identifies the object associated with the
        /// event. This is one of the object identifiers or a custom object
        /// ID.</param>
        /// <param name="idChild">Identifies whether the event was triggered by
        /// an object or a child element of the object. If this value is
        /// <c>CHILDID_SELF</c>, the event was triggered by the object;
        /// otherwise, this value is the child ID of the element that triggered
        /// the event.</param>
        /// <param name="idEventThread"></param>
        /// <param name="dwmsEventTime">Specifies the time, in milliseconds,
        /// that the event was generated.</param>
        private void WinSystemEventProc(
            HWINEVENTHOOK hWinEventHook,
            uint @event,
            HWND hWnd,
            int idObject,
            int idChild,
            uint idEventThread,
            uint dwmsEventTime
        ) => ForegroundWindow = WindowListHelper.IsForegrounded(_windows, @event, hWnd, logger);

        /// <summary>
        /// Handles events <c>EVENT_OBJECT_CREATE</c> through
        /// <c>EVENT_OBJECT_NAMECHANGE</c>.
        /// </summary>
        /// <remarks>
        /// <para>An application-defined callback (or hook) function that the
        /// system calls in response to events generated by an accessible
        /// object. The hook function processes the event notifications as
        /// required. Clients install the hook function and request specific
        /// types of event notifications by calling <see
        /// cref="PInvoke.SetWinEventHook"/>.</para>
        /// <para>The <see cref="WINEVENTPROC"/> type defines a pointer to this
        /// callback function. <c>WinEventProc</c> is a placeholder for the
        /// application-defined function name.</para>
        /// </remarks>
        /// <param name="hWinEventHook">Handle to an event hook function. This
        /// value is returned by <see cref="PInvoke.SetWinEventHook"/> when the
        /// hook function is installed and is specific to each instance of the
        /// hook function.</param>
        /// <param name="event">Specifies the event that occurred. This value
        /// is one of the event constants.</param>
        /// <param name="hWnd">Handle to the window that generates the event,
        /// or <c>null</c> if no window is associated with the event. For
        /// example, the mouse pointer is not associated with a window.</param>
        /// <param name="idObject">Identifies the object associated with the
        /// event. This is one of the object identifiers or a custom object
        /// ID.</param>
        /// <param name="idChild">Identifies whether the event was triggered by
        /// an object or a child element of the object. If this value is
        /// <c>CHILDID_SELF</c>, the event was triggered by the object;
        /// otherwise, this value is the child ID of the element that triggered
        /// the event.</param>
        /// <param name="idEventThread"></param>
        /// <param name="dwmsEventTime">Specifies the time, in milliseconds,
        /// that the event was generated.</param>
        private void WinObjectEventProc(
            HWINEVENTHOOK hWinEventHook,
            uint @event,
            HWND hWnd,
            int idObject,
            int idChild,
            uint idEventThread,
            uint dwmsEventTime
        ) => _dispatcherQueue.TryEnqueue(() => WindowListHelper.UpdateWindow(
            _windows,
            @event,
            hWnd,
            logger
        ));

        private void OnPropertyChanged([CallerMemberName] string? callerMemberName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerMemberName));
    }
}