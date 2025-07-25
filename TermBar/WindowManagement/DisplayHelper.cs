using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.HiDpi;

namespace Spakov.TermBar.WindowManagement
{
    /// <summary>
    /// Display helper methods.
    /// </summary>
    internal class DisplayHelper
    {
        /// <summary>
        /// <para>The <see cref="MONITORINFOEXW"/> structure contains
        /// information about a display monitor.</para>
        /// <para>The <see cref="GetMonitorInfoW"/> function stores information
        /// into a <see cref="MONITORINFOEXW"/> structure or a
        /// <c>MONITORINFO</c> structure.</para>
        /// <para>The <see cref="MONITORINFOEXW"/> structure is a superset of
        /// the <c>MONITORINFO</c> structure.The <see cref="MONITORINFOEXW"/>
        /// structure adds a string member to contain a name for the display
        /// monitor.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct MONITORINFOEXW
        {
            /// <summary>
            /// <para>The size of the structure, in bytes.</para>
            /// <para>Set this member to <c>sizeof ( MONITORINFOEXW )</c>
            /// before calling the <see cref="GetMonitorInfoW"/> function.
            /// Doing so lets the function determine the type of structure you
            /// are passing to it.</para>
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Match Win32")]
            public uint cbSize;

            /// <summary>
            /// A <see cref="RECT"/> structure that specifies the display
            /// monitor rectangle, expressed in virtual-screen coordinates.
            /// Note that if the monitor is not the primary display monitor,
            /// some of the rectangle's coordinates may be negative values.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Match Win32")]
            public RECT rcMonitor;

            /// <summary>
            /// A <see cref="RECT"/> structure that specifies the work area
            /// rectangle of the display monitor, expressed in virtual-screen
            /// coordinates. Note that if the monitor is not the primary
            /// display monitor, some of the rectangle's coordinates may be
            /// negative values.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Match Win32")]
            public RECT rcWork;

            /// <summary>
            /// <para>A set of flags that represent attributes of the display
            /// monitor.</para>
            /// <para>The following flag is defined:
            /// <c>MONITORINFOF_PRIMARY</c></para>
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Match Win32")]
            public uint dwFlags;

            /// <summary>
            /// A string that specifies the device name of the monitor being
            /// used. Most applications have no use for a display monitor name,
            /// and so can save some bytes by using a <c>MONITORINFO</c>
            /// structure.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Match Win32")]
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szDevice;
        }

        /// <summary>
        /// The <see cref="GetMonitorInfoW"/> function retrieves information
        /// about a display monitor.
        /// </summary>
        /// <remarks>Implements managed support for a call with <see
        /// cref="MONITORINFOEXW"/>.</remarks>
        /// <param name="hMonitor">A handle to the display monitor of
        /// interest.</param>
        /// <param name="lpmi">
        /// <para>A pointer to a <c>MONITORINFO</c> or <see
        /// cref="MONITORINFOEXW"/> structure that receives information about
        /// the specified display monitor.</para>
        /// <para>You must set the <c>cbSize</c> member of the structure to
        /// <c>sizeof(MONITORINFO)</c> or <c>sizeof(MONITORINFOEXW)</c> before
        /// calling the <see cref="GetMonitorInfoW"/> function. Doing so lets
        /// the function determine the type of structure you are passing to
        /// it.</para>
        /// <para>The <see cref="MONITORINFOEXW"/> structure is a superset of
        /// the <c>MONITORINFO</c> structure. It has one additional member: a
        /// string that contains a name for the display monitor. Most
        /// applications have no use for a display monitor name, and so can
        /// save some bytes by using a <c>MONITORINFO</c> structure.</para>
        /// </param>
        /// <returns>
        /// <para>If the function succeeds, the return value is nonzero.</para>
        /// <para>If the function fails, the return value is zero.</para>
        /// </returns>
        [DllImport("user32.dll")]
#pragma warning disable IDE0079 // Remove unnecessary suppression
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "SYSLIB1054:Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time", Justification = "GetMonitorInfo and MONITORINFOEX are a little odd: see https://github.com/microsoft/CsWin32/discussions/695.")]
#pragma warning restore IDE0079 // Remove unnecessary suppression
        static extern bool GetMonitorInfoW(IntPtr hMonitor, ref MONITORINFOEXW lpmi);

        /// <summary>
        /// Gets a dictionary of displays available on this device.
        /// </summary>
        /// <returns>A dictionary of displays.</returns>
        internal static unsafe Dictionary<string, Display> GetDisplays()
        {
            Dictionary<string, Display> displays = [];

            List<(MONITORINFOEXW, Dpi)> monitors = [];
            List<DISPLAY_DEVICEW> displayDevices = [];
            List<DISPLAY_DEVICEW> monitorDevices = [];

            PInvoke.EnumDisplayMonitors(
                HDC.Null,
                (RECT*)null,
                (hMonitor, _, _, _) =>
                {
                    MONITORINFOEXW monitorInfo = new() { cbSize = (uint)Marshal.SizeOf<MONITORINFOEXW>() };

                    if (GetMonitorInfoW(hMonitor, ref monitorInfo))
                    {
                        Dpi dpi;

                        PInvoke.GetDpiForMonitor(hMonitor, MONITOR_DPI_TYPE.MDT_EFFECTIVE_DPI, out dpi.X, out dpi.Y);
                        monitors.Add((monitorInfo, dpi));
                    }

                    return true;
                },
                (LPARAM)0
            );

            DISPLAY_DEVICEW displayDevice = new() { cb = (uint)Marshal.SizeOf<DISPLAY_DEVICEW>() };
            uint iDisplayDevNum = 0;

            while (PInvoke.EnumDisplayDevices(
                null,
                iDisplayDevNum++,
                ref displayDevice,
                0
            ))
            {
                displayDevices.Add(displayDevice);

                DISPLAY_DEVICEW monitorDevice = new() { cb = (uint)Marshal.SizeOf<DISPLAY_DEVICEW>() };
                uint iMonitorDevNum = 0;

                while (PInvoke.EnumDisplayDevices(
                    displayDevice.DeviceName.ToString(),
                    iMonitorDevNum++,
                    ref monitorDevice,
                    0
                ))
                {
                    monitorDevices.Add(monitorDevice);
                }
            }

            foreach (DISPLAY_DEVICEW _monitorDevice in monitorDevices)
            {
                foreach ((MONITORINFOEXW _monitorInfo, Dpi dpi) in monitors)
                {
                    if (_monitorDevice.DeviceName.ToString().StartsWith(_monitorInfo.szDevice))
                    {
                        displays.Add(
                            _monitorDevice.DeviceID.ToString(),
                            new(
                                path: _monitorInfo.szDevice,
                                name: _monitorDevice.DeviceString.ToString(),
                                id: _monitorDevice.DeviceID.ToString(),
                                dpi: dpi,
                                left: _monitorInfo.rcWork.left,
                                top: _monitorInfo.rcWork.top,
                                width: _monitorInfo.rcMonitor.Width,
                                height: _monitorInfo.rcMonitor.Height
                            )
                        );
                    }
                }
            }

            return displays;
        }
    }
}