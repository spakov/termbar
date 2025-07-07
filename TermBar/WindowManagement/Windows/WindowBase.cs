using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Hosting;
using System.ComponentModel;
using System.Drawing;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace TermBar.WindowManagement.Windows {
  /// <summary>
  /// The base class from which all TermBar windows derive.
  /// </summary>
  /// <param name="display">The display on which to present the
  /// window.</param>
  /// <param name="config">The <see cref="Configuration.Json.TermBar"/> for
  /// this window.</param>
  internal abstract class Window(Display display, Configuration.Json.TermBar? config) {
    protected readonly Display display = display;

    private INotifyPropertyChanged? island;
    private PropertyChangedEventHandler? islandRequestResize;
    private DesktopWindowXamlSource? xamlSource;
    protected HWND? hWnd;
    protected HWND? islandHWnd;

    /// <summary>
    /// The <see cref="Configuration.Json.TermBar"/> for this display.
    /// </summary>
    protected virtual Configuration.Json.TermBar? Config { get; } = config;

    /// <inheritdoc cref="Configuration.Json.TermBar.Margin"/>
    protected virtual int? Margin { get; } = config?.Margin;

    /// <inheritdoc cref="Configuration.Json.TermBar.Padding"/>
    protected virtual int? Padding { get; } = config?.Padding;

    /// <summary>
    /// The window width, in pixels.
    /// </summary>
    protected virtual int Width { get; set; }

    /// <inheritdoc cref="Configuration.Json.TermBar.Height"/>
    protected virtual int Height { get; set; }

    /// <summary>
    /// Displays the window.
    /// </summary>
    /// <param name="island">The XAML island to present in the window.</param>
    /// <param name="left">The window's initial left postion.</param>
    /// <param name="top">The window's initial top position.</param>
    /// <param name="width">The window's initial width.</param>
    /// <param name="height">The window's initial height.</param>
    /// <param name="requestResize">The <see
    /// cref="PropertyChangedEventHandler"/> to invoke when the <paramref
    /// name="island"/> is resized.</param>
    /// <param name="isDialog">Whether to include <see
    /// cref="WINDOW_EX_STYLE.WS_EX_TOPMOST"/> in the call to <see
    /// cref="PInvoke.CreateWindowEx"/>.</param>
    /// <param name="skipActivation">Whether to include <see
    /// cref="SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE"/> in the call to <see
    /// cref="PInvoke.SetWindowPos"/>.</param>
    protected void Display(INotifyPropertyChanged island, int left, int top, int width, int height, PropertyChangedEventHandler requestResize, bool isDialog = false, bool skipActivation = true) {
      this.island = island;
      islandRequestResize = requestResize;

      unsafe {
        hWnd = PInvoke.CreateWindowEx(
          WINDOW_EX_STYLE.WS_EX_COMPOSITED
          | WINDOW_EX_STYLE.WS_EX_LAYERED
          | WINDOW_EX_STYLE.WS_EX_TOOLWINDOW
          | (isDialog ? WINDOW_EX_STYLE.WS_EX_TOPMOST : 0),
          WindowManager.WindowClassName,
          null,
          WINDOW_STYLE.WS_POPUP
          | WINDOW_STYLE.WS_VISIBLE,
          left,
          top,
          width,
          height,
          HWND.Null,
          null,
          null,
          null
        );
      }

      PInvoke.SetLayeredWindowAttributes((HWND) hWnd, (COLORREF) 0, 255, LAYERED_WINDOW_ATTRIBUTES_FLAGS.LWA_COLORKEY);

      xamlSource = new();
      xamlSource.Initialize(Win32Interop.GetWindowIdFromWindow((HWND) hWnd));

      islandHWnd = new(Win32Interop.GetWindowFromWindowId(xamlSource.SiteBridge.WindowId));

      xamlSource.Content = (UIElement) island;
      island.PropertyChanged += requestResize;

      PInvoke.SetWindowPos(
        (HWND) islandHWnd,
        HWND.Null,
        0,
        0,
        width,
        height,
        SET_WINDOW_POS_FLAGS.SWP_NOZORDER
        | (skipActivation ? SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE : 0)
      );
    }

    /// <summary>
    /// Moves the window.
    /// </summary>
    /// <param name="islandHWnd">The XAML island's <see cref="HWND"/>.</param>
    /// <param name="left">The window's left postion.</param>
    /// <param name="top">The window's top position.</param>
    /// <param name="width">The window's width.</param>
    /// <param name="height">The window's height.</param>
    /// <param name="skipActivation">Whether to include <see
    /// cref="SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE"/> in the calls to <see
    /// cref="PInvoke.SetWindowPos"/>.</param>
    protected void Move(int left, int top, int width, int height, bool skipActivation = true) {
      if (hWnd is null || islandHWnd is null) return;

      PInvoke.SetWindowPos(
        (HWND) hWnd,
        HWND.Null,
        left,
        top,
        width,
        height,
        SET_WINDOW_POS_FLAGS.SWP_NOZORDER
        | (skipActivation ? SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE : 0)
      );

      PInvoke.SetWindowPos(
        (HWND) islandHWnd,
        HWND.Null,
        0,
        0,
        width,
        height,
        SET_WINDOW_POS_FLAGS.SWP_NOZORDER
        | (skipActivation ? SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE : 0)
      );
    }

    /// <summary>
    /// Returns a <see cref="Rectangle"/> describing the window's position on
    /// the screen.
    /// </summary>
    /// <returns>A <see cref="Rectangle"/>.</returns>
    /// <exception cref="Win32Exception"></exception>
    protected Rectangle GetPosition() {
      RECT windowRect = new();

      return PInvoke.GetWindowRect(
        (HWND) hWnd!,
        out windowRect
      )
        ? new(windowRect.X, windowRect.Y, windowRect.Width, windowRect.Height)
        : throw new Win32Exception(System.Runtime.InteropServices.Marshal.GetLastWin32Error());
    }

    /// <summary>
    /// Closes the window politely.
    /// </summary>
    internal void Close() {
      PInvoke.PostMessage((HWND) islandHWnd!, PInvoke.WM_CLOSE, 0, 0);

      if (island is not null && islandRequestResize is not null) {
        island.PropertyChanged -= islandRequestResize;
      }

      if (xamlSource is not null) {
        xamlSource.Content = null;
        xamlSource.Dispose();
        xamlSource = null;
      }

      PInvoke.PostMessage((HWND) hWnd!, PInvoke.WM_CLOSE, 0, 0);
    }

    /// <summary>
    /// Returns <paramref name="rawX"/>, scaled for the display's DPI.
    /// </summary>
    /// <param name="rawX">The value to scale.</param>
    /// <returns>The scaled value.</returns>
    protected int ScaleX(int rawX) => rawX * (int) (display.Dpi.X / PInvoke.USER_DEFAULT_SCREEN_DPI);

    /// <summary>
    /// Returns <paramref name="rawY"/>, scaled for the display's DPI.
    /// </summary>
    /// <param name="rawY">The value to scale.</param>
    /// <returns>The scaled value.</returns>
    protected int ScaleY(int rawY) => rawY * (int) (display.Dpi.Y / PInvoke.USER_DEFAULT_SCREEN_DPI);
  }
}
