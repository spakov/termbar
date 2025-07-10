namespace Spakov.TermBar.Models {
  /// <summary>
  /// A window that can be managed by <see cref="WindowListHelper"/>.
  /// </summary>
  public interface IWindowListWindow {
    /// <summary>
    /// The window's process ID.
    /// </summary>
    public uint WindowProcessId { get; }

    /// <summary>
    /// The window's name.
    /// </summary>
    public string? WindowName { get; }
  }
}