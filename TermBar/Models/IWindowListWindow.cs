namespace Spakov.TermBar.Models
{
    /// <summary>
    /// A window that can be managed by <see
    /// cref="WindowListHelper.OrderAndInsert{T}"/>.
    /// </summary>
    /// <remarks>Contains the only metadata that needs to be exposed: the
    /// window's owning process ID, and the window's name.</remarks>
    public interface IWindowListWindow
    {
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