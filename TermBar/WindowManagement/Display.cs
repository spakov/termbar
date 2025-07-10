namespace Spakov.TermBar.WindowManagement {
  /// <summary>
  /// A display.
  /// </summary>
  internal class Display {
    /// <summary>
    /// The device path (<see
    /// cref="Windows.Win32.Graphics.Gdi.DISPLAY_DEVICEW.DeviceName"/>).
    /// </summary>
    internal string Path { get; private init; }

    /// <summary>
    /// The device name (<see
    /// cref="Windows.Win32.Graphics.Gdi.DISPLAY_DEVICEW.DeviceString"/>).
    /// </summary>
    internal string Name { get; private init; }

    /// <summary>
    /// The device ID (<see
    /// cref="Windows.Win32.Graphics.Gdi.DISPLAY_DEVICEW.DeviceID"/>).
    /// </summary>
    internal string Id { get; private init; }

    /// <summary>
    /// The display DPI.
    /// </summary>
    internal Dpi Dpi { get; private init; }

    /// <summary>
    /// The display's left work area.
    /// </summary>
    internal int Left { get; private init; }

    /// <summary>
    /// The display's top work area.
    /// </summary>
    internal int Top { get; private init; }

    /// <summary>
    /// The display width.
    /// </summary>
    internal int Width { get; private init; }

    /// <summary>
    /// The display height.
    /// </summary>
    internal int Height { get; private init; }

    /// <summary>
    /// Initializes a <see cref="Display"/>.
    /// </summary>
    /// <param name="path"><inheritdoc cref="Path" path="/summary"/></param>
    /// <param name="name"><inheritdoc cref="Name" path="/summary"/></param>
    /// <param name="id"><inheritdoc cref="Id" path="/summary"/></param>
    /// <param name="dpi"><inheritdoc cref="Dpi" path="/summary"/></param>
    /// <param name="left"><inheritdoc cref="Left" path="/summary"/></param>
    /// <param name="top"><inheritdoc cref="Top" path="/summary"/></param>
    /// <param name="width"><inheritdoc cref="Width" path="/summary"/></param>
    /// <param name="height"><inheritdoc cref="Height" path="/summary"/></param>
    internal Display(string path, string name, string id, Dpi dpi, int left, int top, int width, int height) {
      Path = path;
      Name = name;
      Id = id;
      Dpi = dpi;
      Left = left;
      Top = top;
      Width = width;
      Height = height;
    }
  }
}