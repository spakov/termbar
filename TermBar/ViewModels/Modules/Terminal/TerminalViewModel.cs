using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Spakov.ConPTY;
using Spakov.TermBar.Views.Modules.Terminal;

namespace Spakov.TermBar.ViewModels.Modules.Terminal {
  /// <summary>
  /// The terminal viewmodel.
  /// </summary>
  internal partial class TerminalViewModel : INotifyPropertyChanged {
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly TerminalView terminalView;
    private readonly Configuration.Json.Modules.Terminal config;

    private readonly Pseudoconsole pseudoconsole;

    private AnsiProcessor.AnsiColors.Palette _ansiColors;

    private string? _windowTitle;
    private FileStream? _consoleOutput;
    private FileStream? _consoleInput;
    private int _rows;
    private int _columns;
    private bool _restartOnExit;

    private string? _icon;
    private string? _visualBellIcon;

    /// <summary>
    /// Callback for handling the case in which the pseudoconsole dies.
    /// </summary>
    public delegate void OnPseudoconsoleDied(Exception e);

    /// <summary>
    /// Invoked if the pseudoconsole dies.
    /// </summary>
    public event OnPseudoconsoleDied? PseudoconsoleDied;

    /// <summary>
    /// The window title.
    /// </summary>
    public string? WindowTitle {
      get => _windowTitle;

      set {
        if (_windowTitle != value) {
          _windowTitle = value;
          OnPropertyChanged();
        }
      }
    }

    /// <summary>
    /// The <see cref="AnsiProcessor.AnsiColors.Palette"/> used for ANSI
    /// colors.
    /// </summary>
    public AnsiProcessor.AnsiColors.Palette AnsiColors {
      get => _ansiColors;

      set {
        if (_ansiColors != value) {
          _ansiColors = value;
          OnPropertyChanged();
        }
      }
    }

    /// <summary>
    /// The console's output <see cref="FileStream"/>.
    /// </summary>
    public FileStream? ConsoleOutput {
      get => _consoleOutput;

      set {
        if (_consoleOutput != value) {
          _consoleOutput = value;
          OnPropertyChanged();
        }
      }
    }

    /// <summary>
    /// The console's input <see cref="FileStream"/>.
    /// </summary>
    public FileStream? ConsoleInput {
      get => _consoleInput;

      set {
        if (_consoleInput != value) {
          _consoleInput = value;
          OnPropertyChanged();
        }
      }
    }

    /// <summary>
    /// The number of console rows.
    /// </summary>
    public int Rows {
      get => _rows;

      set {
        int oldRows = (int) _rows;

        _rows = value;
        OnPropertyChanged();

        if (oldRows != value) {
          pseudoconsole.Rows = (uint) _rows;
        }
      }
    }

    /// <summary>
    /// The number of console columns.
    /// </summary>
    public int Columns {
      get => _columns;

      set {
        int oldColumns = _columns;

        _columns = value;
        OnPropertyChanged();

        if (oldColumns != value) {
          pseudoconsole.Columns = (uint) _columns;
        }
      }
    }

    /// <summary>
    /// Whether to restart the command when it exits normally.
    /// </summary>
    public bool RestartOnExit {
      get => _restartOnExit;

      set {
        if (_restartOnExit != value) {
          _restartOnExit = value;
          pseudoconsole.RestartOnDone = _restartOnExit;
        }
      }
    }

    /// <summary>
    /// The terminal icon.
    /// </summary>
    public string? Icon {
      get => _icon;

      set {
        if (_icon != value) {
          _icon = value;
        }
      }
    }

    /// <summary>
    /// The terminal visual bell icon.
    /// </summary>
    public string? VisualBellIcon {
      get => _visualBellIcon;

      set {
        if (_visualBellIcon != value) {
          _visualBellIcon = value;
        }
      }
    }

    /// <summary>
    /// Initializes a <see cref="TerminalViewModel"/>.
    /// </summary>
    /// <param name="terminalView">A <see cref="TerminalView"/>.</param>
    /// <param name="config">The <see
    /// cref="Configuration.Json.Modules.Terminal"/> configuration.</param>
    /// <param name="ansiColors">The colors to use as the terminal's
    /// palette.</param>
    internal TerminalViewModel(TerminalView terminalView, Configuration.Json.Modules.Terminal config, AnsiProcessor.AnsiColors.Palette ansiColors) {
      this.terminalView = terminalView;
      this.config = config;

      WindowTitle = config.DefaultWindowTitle;

      _rows = config.Rows;
      _columns = config.Columns;
      _restartOnExit = config.RestartOnExit;
      _ansiColors = ansiColors;

      pseudoconsole = new(config.Command, (uint) config.Rows, (uint) config.Columns, config.RestartOnExit);
      pseudoconsole.Ready += Pseudoconsole_Ready;
      pseudoconsole.Done += Pseudoconsole_Done;

      StartPseudoconsole();

      Icon = config.Icon;
      VisualBellIcon = config.VisualBellIcon;
    }

    /// <summary>
    /// Starts the pseudoconsole, checking for error conditions.
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    private async void StartPseudoconsole() {
      try {
        await Task.Run(pseudoconsole.Start);
      } catch (Win32Exception e) {
        PseudoconsoleDied?.Invoke(new ArgumentException($"Failed to start pseudoconsole with command \"{pseudoconsole.Command}\".", e));
      }
    }

    /// <summary>
    /// Invoked when the pseudoconsole is ready.
    /// </summary>
    private void Pseudoconsole_Ready() {
      ConsoleOutput = pseudoconsole.ConsoleOutStream;
      ConsoleInput = pseudoconsole.ConsoleInStream;
    }

    /// <summary>
    /// Invoked when the pseudoconsole is done.
    /// </summary>
    /// <param name="exitCode">The exit code of the command that
    /// executed.</param>
    private void Pseudoconsole_Done(uint exitCode) {
      bool printMessage = !RestartOnExit || exitCode != 0;

      if (printMessage) {
        terminalView.Write($"{pseudoconsole.Command} exited with {exitCode}");
      }
    }

    private void OnPropertyChanged([CallerMemberName] string? callerMemberName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerMemberName));
  }
}