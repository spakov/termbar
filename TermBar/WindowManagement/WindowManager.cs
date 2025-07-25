using Spakov.TermBar.WindowManagement.Windows;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Spakov.TermBar.WindowManagement
{
    /// <summary>
    /// The TermBar window manager.
    /// </summary>
    /// <remarks>The window manager manages display of native windows
    /// containing XAML islands, to include their display, position, and
    /// events.</remarks>
    internal class WindowManager
    {
        private readonly Dictionary<string, TermBarWindow> _windows;

        /// <summary>
        /// The window class name.
        /// </summary>
        internal static string WindowClassName => "TermBarClass";

        /// <summary>
        /// Available displays.
        /// </summary>
        internal Dictionary<string, Display> Displays { get; private set; }

        /// <summary>
        /// Initializes a <see cref="WindowManager"/>.
        /// </summary>
        internal WindowManager()
        {
            Displays = DisplayHelper.GetDisplays();
            _windows = [];

            RegisterWindowClass();
        }

        /// <summary>
        /// Displays TermBar on configured displays.
        /// </summary>
        internal void Display()
        {
            if (App.Config is null)
            {
                return;
            }

            foreach (Configuration.Json.Display display in App.Config.Displays)
            {
                _windows.Add(display.Id, new(display.TermBar));
            }

            foreach (KeyValuePair<string, TermBarWindow> window in _windows)
            {
                window.Value.Display();
            }
        }

        /// <summary>
        /// Registers the TermBar window class.
        /// </summary>
        private static unsafe void RegisterWindowClass()
        {
            HMODULE hInstance = PInvoke.GetModuleHandle((PCWSTR)null);

            WNDCLASSEXW wndClass;

            fixed (char* classNamePtr = WindowClassName)
            {
                wndClass = new()
                {
                    cbSize = (uint)Marshal.SizeOf<WNDCLASSEXW>(),
                    lpfnWndProc = PInvoke.DefWindowProc,
                    hInstance = hInstance,
                    lpszClassName = new(classNamePtr)
                };
            }

            PInvoke.RegisterClassEx(wndClass);
        }
    }
}