using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Win32.Foundation;

namespace TermBar.Models {
  /// <summary>
  /// A window.
  /// </summary>
#pragma warning disable IDE0079 // Remove unnecessary suppression
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CsWinRT1028:Class is not marked partial", Justification = "Is a model")]
#pragma warning restore IDE0079 // Remove unnecessary suppression
  public class Window : INotifyPropertyChanged {
    public event PropertyChangedEventHandler? PropertyChanged;

    private string _name;

    /// <summary>
    /// The window's <see cref="HWND"/>.
    /// </summary>
    internal HWND HWnd { get; private set; }

    /// <summary>
    /// The window's owning process ID.
    /// </summary>
    public uint ProcessId { get; private set; }

    /// <summary>
    /// The window's name.
    /// </summary>
    public string Name {
      get => _name;
      
      internal set {
        if (_name != value) {
          _name = value;
          OnPropertyChanged();
        }
      }
    }

    /// <summary>
    /// Initializes a <see cref="Window"/>.
    /// </summary>
    /// <param name="hWnd"><inheritdoc cref="HWnd" path="/summary"/></param>
    /// <param name="processId"><inheritdoc cref="ProcessId"
    /// path="/summary"/></param>
    /// <param name="name"><inheritdoc cref="Name" path="/summary"/></param>
    internal Window(HWND hWnd, uint processId, string name) {
      HWnd = hWnd;
      ProcessId = processId;
      _name = name;
    }

    private void OnPropertyChanged([CallerMemberName] string? callerMemberName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerMemberName));
  }
}
