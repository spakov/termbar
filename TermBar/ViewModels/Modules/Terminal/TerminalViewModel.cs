using Microsoft.UI.Xaml;
using Spakov.Catppuccin;
using Spakov.ConPTY;
using Spakov.TermBar.Views.Modules.Terminal;
using Spakov.Terminal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Spakov.TermBar.ViewModels.Modules.Terminal {
  /// <summary>
  /// The terminal viewmodel.
  /// </summary>
  internal partial class TerminalViewModel : INotifyPropertyChanged {
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly TerminalView terminalView;

    private readonly Configuration.Json.TermBar config;
    private readonly Configuration.Json.Modules.Terminal moduleConfig;

    private readonly Pseudoconsole pseudoconsole;

    private readonly Dictionary<DependencyProperty, long> callbackTokens;

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
    /// <param name="moduleConfig">The <see
    /// cref="Configuration.Json.Modules.Terminal"/> configuration.</param>
    /// <param name="ansiColors">The colors to use as the terminal's
    /// palette.</param>
    internal TerminalViewModel(TerminalView terminalView, Configuration.Json.TermBar config, Configuration.Json.Modules.Terminal moduleConfig, AnsiProcessor.AnsiColors.Palette ansiColors) {
      this.terminalView = terminalView;
      this.config = config;
      this.moduleConfig = moduleConfig;

      WindowTitle = moduleConfig.DefaultWindowTitle;

      callbackTokens = [];

      _rows = moduleConfig.Rows;
      _columns = moduleConfig.Columns;
      _restartOnExit = moduleConfig.RestartOnExit;
      _ansiColors = ansiColors;

      pseudoconsole = new(moduleConfig.Command, (uint) moduleConfig.Rows, (uint) moduleConfig.Columns, moduleConfig.RestartOnExit);
      pseudoconsole.Ready += Pseudoconsole_Ready;
      pseudoconsole.Done += Pseudoconsole_Done;

      StartPseudoconsole();

      Icon = moduleConfig.Icon;
      VisualBellIcon = moduleConfig.VisualBellIcon;
    }

    /// <summary>
    /// Binds TermBar's <see cref="Configuration.Json.Config"/> and <see
    /// cref="Configuration.Json.Modules.Terminal"/> to <paramref
    /// name="terminalControl"/>'s <see cref="DependencyProperty"/>s.
    /// </summary>
    /// <param name="terminalControl">A <see cref="TerminalControl"/>.</param>
    internal void BindSettings(TerminalControl terminalControl) {
      terminalControl.DefaultWindowTitle = moduleConfig.DefaultWindowTitle;

      terminalControl.FontFamily = moduleConfig.FontFamily is not null
        ? moduleConfig.FontFamily
        : config.FontFamily;

      terminalControl.FontSize = moduleConfig.FontSize is not null
        ? (double) moduleConfig.FontSize
        : config.FontSize;

      if (moduleConfig.TabWidth is not null) terminalControl.TabWidth = (int) moduleConfig.TabWidth;
      if (moduleConfig.TextAntialiasing is not null) terminalControl.TextAntialiasing = (TextAntialiasingStyles) moduleConfig.TextAntialiasing;
      if (moduleConfig.FullColorEmoji is not null) terminalControl.FullColorEmoji = (bool) moduleConfig.FullColorEmoji;
      if (moduleConfig.UseBackgroundColorErase is not null) terminalControl.UseBackgroundColorErase = (bool) moduleConfig.UseBackgroundColorErase;
      if (moduleConfig.BackgroundIsInvisible is not null) terminalControl.BackgroundIsInvisible = (bool) moduleConfig.BackgroundIsInvisible;
      if (moduleConfig.UseVisualBell is not null) terminalControl.UseVisualBell = (bool) moduleConfig.UseVisualBell;
      if (moduleConfig.UseContextMenu is not null) terminalControl.UseContextMenu = (bool) moduleConfig.UseContextMenu;
      if (moduleConfig.UseExtendedContextMenu is not null) terminalControl.UseExtendedContextMenu = (bool) moduleConfig.UseExtendedContextMenu;
      if (moduleConfig.CursorStyle is not null) terminalControl.CursorStyle = (CursorStyles) moduleConfig.CursorStyle;
      if (moduleConfig.CursorThickness is not null) terminalControl.CursorThickness = (double) moduleConfig.CursorThickness;
      if (moduleConfig.CursorBlink is not null) terminalControl.CursorBlink = (bool) moduleConfig.CursorBlink;
      if (moduleConfig.CursorBlinkRate is not null) terminalControl.CursorBlinkRate = (int) moduleConfig.CursorBlinkRate;
      if (moduleConfig.CursorColor is not null) terminalControl.CursorColor = Palette.Instance[config.Flavor].Colors[moduleConfig.AccentColor].WUIColor;
      if (moduleConfig.ScrollbackLines is not null) terminalControl.Scrollback = (int) moduleConfig.ScrollbackLines;
      if (moduleConfig.LinesPerScrollback is not null) terminalControl.LinesPerScrollback = (int) moduleConfig.LinesPerScrollback;
      if (moduleConfig.LinesPerSmallScrollback is not null) terminalControl.LinesPerSmallScrollback = (int) moduleConfig.LinesPerSmallScrollback;
      if (moduleConfig.LinesPerWheelScrollback is not null) terminalControl.LinesPerWheelScrollback = (int) moduleConfig.LinesPerWheelScrollback;
      if (moduleConfig.CopyOnMouseUp is not null) terminalControl.CopyOnMouseUp = (bool) moduleConfig.CopyOnMouseUp;
      if (moduleConfig.PasteOnMiddleClick is not null) terminalControl.PasteOnMiddleClick = (bool) moduleConfig.PasteOnMiddleClick;
      if (moduleConfig.PasteOnRightClick is not null) terminalControl.PasteOnRightClick = (bool) moduleConfig.PasteOnRightClick;
      if (moduleConfig.CopyNewline is not null) terminalControl.CopyNewline = (string) moduleConfig.CopyNewline;

      callbackTokens.Add(
        TerminalControl.DefaultWindowTitleProperty,
        terminalControl.RegisterPropertyChangedCallback(
          TerminalControl.DefaultWindowTitleProperty,
          (_, _) => moduleConfig.DefaultWindowTitle = terminalControl.DefaultWindowTitle
        )
      );

      callbackTokens.Add(
        TerminalControl.RowsProperty,
        terminalControl.RegisterPropertyChangedCallback(
          TerminalControl.RowsProperty,
          (_, _) => moduleConfig.Rows = terminalControl.Rows
        )
      );

      callbackTokens.Add(
        TerminalControl.ColumnsProperty,
        terminalControl.RegisterPropertyChangedCallback(
          TerminalControl.ColumnsProperty,
          (_, _) => moduleConfig.Columns = terminalControl.Columns
        )
      );

      callbackTokens.Add(
        TerminalControl.TabWidthProperty,
        terminalControl.RegisterPropertyChangedCallback(
          TerminalControl.TabWidthProperty,
          (_, _) => moduleConfig.TabWidth = terminalControl.TabWidth
        )
      );

      callbackTokens.Add(
        TerminalControl.FontFamilyProperty,
        terminalControl.RegisterPropertyChangedCallback(
          TerminalControl.FontFamilyProperty,
          (_, _) => moduleConfig.FontFamily = terminalControl.FontFamily
        )
      );

      callbackTokens.Add(
        TerminalControl.FontSizeProperty,
        terminalControl.RegisterPropertyChangedCallback(
          TerminalControl.FontSizeProperty,
          (_, _) => moduleConfig.FontSize = terminalControl.FontSize
        )
      );

      callbackTokens.Add(
        TerminalControl.TextAntialiasingProperty,
        terminalControl.RegisterPropertyChangedCallback(
          TerminalControl.TextAntialiasingProperty,
          (_, _) => moduleConfig.TextAntialiasing = terminalControl.TextAntialiasing
        )
      );

      callbackTokens.Add(
        TerminalControl.FullColorEmojiProperty,
        terminalControl.RegisterPropertyChangedCallback(
          TerminalControl.FullColorEmojiProperty,
          (_, _) => moduleConfig.FullColorEmoji = terminalControl.FullColorEmoji
        )
      );

      callbackTokens.Add(
        TerminalControl.UseBackgroundColorEraseProperty,
        terminalControl.RegisterPropertyChangedCallback(
          TerminalControl.UseBackgroundColorEraseProperty,
          (_, _) => moduleConfig.UseBackgroundColorErase = terminalControl.UseBackgroundColorErase
        )
      );

      callbackTokens.Add(
        TerminalControl.BackgroundIsInvisibleProperty,
        terminalControl.RegisterPropertyChangedCallback(
          TerminalControl.BackgroundIsInvisibleProperty,
          (_, _) => moduleConfig.BackgroundIsInvisible = terminalControl.BackgroundIsInvisible
        )
      );

      callbackTokens.Add(
        TerminalControl.UseVisualBellProperty,
        terminalControl.RegisterPropertyChangedCallback(
          TerminalControl.UseVisualBellProperty,
          (_, _) => moduleConfig.UseVisualBell = terminalControl.UseVisualBell
        )
      );

      callbackTokens.Add(
        TerminalControl.UseContextMenuProperty,
        terminalControl.RegisterPropertyChangedCallback(
          TerminalControl.UseContextMenuProperty,
          (_, _) => moduleConfig.UseContextMenu = terminalControl.UseContextMenu
        )
      );

      callbackTokens.Add(
        TerminalControl.UseExtendedContextMenuProperty,
        terminalControl.RegisterPropertyChangedCallback(
          TerminalControl.UseExtendedContextMenuProperty,
          (_, _) => moduleConfig.UseExtendedContextMenu = terminalControl.UseExtendedContextMenu
        )
      );

      callbackTokens.Add(
        TerminalControl.CursorStyleProperty,
        terminalControl.RegisterPropertyChangedCallback(
          TerminalControl.CursorStyleProperty,
          (_, _) => moduleConfig.CursorStyle = terminalControl.CursorStyle
        )
      );

      callbackTokens.Add(
        TerminalControl.CursorThicknessProperty,
        terminalControl.RegisterPropertyChangedCallback(
          TerminalControl.CursorThicknessProperty,
          (_, _) => moduleConfig.CursorThickness = terminalControl.CursorThickness
        )
      );

      callbackTokens.Add(
        TerminalControl.CursorBlinkProperty,
        terminalControl.RegisterPropertyChangedCallback(
          TerminalControl.CursorBlinkProperty,
          (_, _) => moduleConfig.CursorBlink = terminalControl.CursorBlink
        )
      );

      callbackTokens.Add(
        TerminalControl.CursorBlinkRateProperty,
        terminalControl.RegisterPropertyChangedCallback(
          TerminalControl.CursorBlinkRateProperty,
          (_, _) => moduleConfig.CursorBlinkRate = terminalControl.CursorBlinkRate
        )
      );

      callbackTokens.Add(
        TerminalControl.CursorColorProperty,
        terminalControl.RegisterPropertyChangedCallback(
          TerminalControl.CursorColorProperty,
          (_, _) => {
            moduleConfig.CursorColor = Palette.Instance[config.Flavor].Colors.FromRGB(
              terminalControl.CursorColor.R,
              terminalControl.CursorColor.G,
              terminalControl.CursorColor.B
            );
          }
        )
      );

      callbackTokens.Add(
        TerminalControl.ScrollbackProperty,
        terminalControl.RegisterPropertyChangedCallback(
          TerminalControl.ScrollbackProperty,
          (_, _) => moduleConfig.ScrollbackLines = terminalControl.Scrollback
        )
      );

      callbackTokens.Add(
        TerminalControl.LinesPerScrollbackProperty,
        terminalControl.RegisterPropertyChangedCallback(
          TerminalControl.LinesPerScrollbackProperty,
          (_, _) => moduleConfig.LinesPerScrollback = terminalControl.LinesPerScrollback
        )
      );

      callbackTokens.Add(
        TerminalControl.LinesPerSmallScrollbackProperty,
        terminalControl.RegisterPropertyChangedCallback(
          TerminalControl.LinesPerSmallScrollbackProperty,
          (_, _) => moduleConfig.LinesPerSmallScrollback = terminalControl.LinesPerSmallScrollback
        )
      );

      callbackTokens.Add(
        TerminalControl.LinesPerWheelScrollbackProperty,
        terminalControl.RegisterPropertyChangedCallback(
          TerminalControl.LinesPerWheelScrollbackProperty,
          (_, _) => moduleConfig.LinesPerWheelScrollback = terminalControl.LinesPerWheelScrollback
        )
      );

      callbackTokens.Add(
        TerminalControl.CopyOnMouseUpProperty,
        terminalControl.RegisterPropertyChangedCallback(
          TerminalControl.CopyOnMouseUpProperty,
          (_, _) => moduleConfig.CopyOnMouseUp = terminalControl.CopyOnMouseUp
        )
      );

      callbackTokens.Add(
        TerminalControl.PasteOnMiddleClickProperty,
        terminalControl.RegisterPropertyChangedCallback(
          TerminalControl.PasteOnMiddleClickProperty,
          (_, _) => moduleConfig.PasteOnMiddleClick = terminalControl.PasteOnMiddleClick
        )
      );

      callbackTokens.Add(
        TerminalControl.PasteOnRightClickProperty,
        terminalControl.RegisterPropertyChangedCallback(
          TerminalControl.PasteOnRightClickProperty,
          (_, _) => moduleConfig.PasteOnRightClick = terminalControl.PasteOnRightClick
        )
      );

      callbackTokens.Add(
        TerminalControl.CopyNewlineProperty,
        terminalControl.RegisterPropertyChangedCallback(
          TerminalControl.CopyNewlineProperty,
          (_, _) => moduleConfig.CopyNewline = terminalControl.CopyNewline
        )
      );
    }

    /// <summary>
    /// Starts the pseudoconsole, checking for error conditions.
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    private async void StartPseudoconsole() {
      try {
        await Task.Run(pseudoconsole.Start);
      } catch (Win32Exception e) {
        PseudoconsoleDied?.Invoke(new ArgumentException(string.Format(App.ResourceLoader.GetString("FailedToStartPseudoconsole"), pseudoconsole.Command), e));
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
        terminalView.Write(string.Format(App.ResourceLoader.GetString("PseudoconsoleExitedWith"), pseudoconsole.Command, exitCode));
      }
    }

    private void OnPropertyChanged([CallerMemberName] string? callerMemberName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerMemberName));
  }
}