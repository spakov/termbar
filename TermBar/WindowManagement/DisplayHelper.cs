using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.HiDpi;

namespace TermBar.WindowManagement {
  /// <summary>
  /// Display helper methods.
  /// </summary>
  internal class DisplayHelper {
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct MONITORINFOEXW {
      public uint cbSize;
      public RECT rcMonitor;
      public RECT rcWork;
      public uint dwFlags;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
      public string szDevice;
    }

    [DllImport("user32.dll")]
#pragma warning disable IDE0079 // Remove unnecessary suppression
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "SYSLIB1054:Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time", Justification = "GetMonitorInfo and MONITORINFOEX are a little odd: see https://github.com/microsoft/CsWin32/discussions/695.")]
#pragma warning restore IDE0079 // Remove unnecessary suppression
    static extern bool GetMonitorInfoW(IntPtr hMonitor, ref MONITORINFOEXW lpmi);

    /// <summary>
    /// Returns a dictionary of displays.
    /// </summary>
    /// <returns>A dictionary of displays.</returns>
    internal static unsafe Dictionary<string, Display> GetDisplays() {
      Dictionary<string, Display> displays = [];

      List<(MONITORINFOEXW, Dpi)> monitors = [];
      List<DISPLAY_DEVICEW> displayDevices = [];
      List<DISPLAY_DEVICEW> monitorDevices = [];

      PInvoke.EnumDisplayMonitors(
        HDC.Null,
        (RECT*) null,
        (hMonitor, _, _, _) => {
          MONITORINFOEXW monitorInfo = new() { cbSize = (uint) Marshal.SizeOf<MONITORINFOEXW>() };

          if (GetMonitorInfoW(hMonitor, ref monitorInfo)) {
            Dpi dpi;

            PInvoke.GetDpiForMonitor(hMonitor, MONITOR_DPI_TYPE.MDT_EFFECTIVE_DPI, out dpi.X, out dpi.Y);
            monitors.Add((monitorInfo, dpi));
          }

          return true;
        },
        (LPARAM) 0
      );

      DISPLAY_DEVICEW displayDevice = new() { cb = (uint) Marshal.SizeOf<DISPLAY_DEVICEW>() };
      uint iDisplayDevNum = 0;

      while (PInvoke.EnumDisplayDevices(
        null,
        iDisplayDevNum++,
        ref displayDevice,
        0
      )) {
        displayDevices.Add(displayDevice);

        DISPLAY_DEVICEW monitorDevice = new() { cb = (uint) Marshal.SizeOf<DISPLAY_DEVICEW>() };
        uint iMonitorDevNum = 0;

        while (PInvoke.EnumDisplayDevices(
          displayDevice.DeviceName.ToString(),
          iMonitorDevNum++,
          ref monitorDevice,
          0
        )) {
          monitorDevices.Add(monitorDevice);
        }
      }

      foreach (DISPLAY_DEVICEW _monitorDevice in monitorDevices) {
        foreach ((MONITORINFOEXW _monitorInfo, Dpi dpi) in monitors) {
          if (_monitorDevice.DeviceName.ToString().StartsWith(_monitorInfo.szDevice)) {
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
