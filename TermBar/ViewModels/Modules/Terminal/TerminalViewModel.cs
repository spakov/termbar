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

namespace Spakov.TermBar.ViewModels.Modules.Terminal
{
    /// <summary>
    /// The terminal viewmodel.
    /// </summary>
    internal partial class TerminalViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly TerminalView _terminalView;

        private readonly Configuration.Json.TermBar _config;
        private readonly Configuration.Json.Modules.Terminal _moduleConfig;

        private readonly Pseudoconsole _pseudoconsole;

        private readonly Dictionary<DependencyProperty, long> _callbackTokens;

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
        /// <param name="e">The exception that was generated when the
        /// pseudoconsole died.</param>
        public delegate void OnPseudoconsoleDied(Exception e);

        /// <summary>
        /// Invoked if the pseudoconsole dies.
        /// </summary>
        public event OnPseudoconsoleDied? PseudoconsoleDied;

        /// <summary>
        /// The window title.
        /// </summary>
        public string? WindowTitle
        {
            get => _windowTitle;

            set
            {
                if (_windowTitle != value)
                {
                    _windowTitle = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The <see cref="AnsiProcessor.AnsiColors.Palette"/> used for ANSI
        /// colors.
        /// </summary>
        public AnsiProcessor.AnsiColors.Palette AnsiColors
        {
            get => _ansiColors;

            set
            {
                if (_ansiColors != value)
                {
                    _ansiColors = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The console's output <see cref="FileStream"/>.
        /// </summary>
        public FileStream? ConsoleOutput
        {
            get => _consoleOutput;

            set
            {
                if (_consoleOutput != value)
                {
                    _consoleOutput = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The console's input <see cref="FileStream"/>.
        /// </summary>
        public FileStream? ConsoleInput
        {
            get => _consoleInput;

            set
            {
                if (_consoleInput != value)
                {
                    _consoleInput = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The number of console rows.
        /// </summary>
        public int Rows
        {
            get => _rows;

            set
            {
                int oldRows = (int)_rows;

                _rows = value;
                OnPropertyChanged();

                if (oldRows != value)
                {
                    _pseudoconsole.Rows = (uint)_rows;
                }
            }
        }

        /// <summary>
        /// The number of console columns.
        /// </summary>
        public int Columns
        {
            get => _columns;

            set
            {
                int oldColumns = _columns;

                _columns = value;
                OnPropertyChanged();

                if (oldColumns != value)
                {
                    _pseudoconsole.Columns = (uint)_columns;
                }
            }
        }

        /// <summary>
        /// Whether to restart the command when it exits normally.
        /// </summary>
        public bool RestartOnExit
        {
            get => _restartOnExit;

            set
            {
                if (_restartOnExit != value)
                {
                    _restartOnExit = value;
                    _pseudoconsole.RestartOnDone = _restartOnExit;
                }
            }
        }

        /// <summary>
        /// The terminal icon.
        /// </summary>
        public string? Icon
        {
            get => _icon;

            set
            {
                if (_icon != value)
                {
                    _icon = value;
                }
            }
        }

        /// <summary>
        /// The terminal visual bell icon.
        /// </summary>
        public string? VisualBellIcon
        {
            get => _visualBellIcon;

            set
            {
                if (_visualBellIcon != value)
                {
                    _visualBellIcon = value;
                }
            }
        }

        /// <summary>
        /// Initializes a <see cref="TerminalViewModel"/>.
        /// </summary>
        /// <param name="terminalView">A <see cref="TerminalView"/>.</param>
        /// <param name="config">A <see cref="Configuration.Json.TermBar"/>
        /// configuration.</param>
        /// <param name="moduleConfig">The <see
        /// cref="Configuration.Json.Modules.Terminal"/> configuration.</param>
        /// <param name="ansiColors">The colors to use as the terminal's
        /// palette.</param>
        internal TerminalViewModel(TerminalView terminalView, Configuration.Json.TermBar config, Configuration.Json.Modules.Terminal moduleConfig, AnsiProcessor.AnsiColors.Palette ansiColors)
        {
            _terminalView = terminalView;
            _config = config;
            _moduleConfig = moduleConfig;

            WindowTitle = moduleConfig.DefaultWindowTitle;

            _callbackTokens = [];

            _rows = moduleConfig.Rows;
            _columns = moduleConfig.Columns;
            _restartOnExit = moduleConfig.RestartOnExit;
            _ansiColors = ansiColors;

            _pseudoconsole = new(moduleConfig.Command, (uint)moduleConfig.Rows, (uint)moduleConfig.Columns, moduleConfig.RestartOnExit);
            _pseudoconsole.Ready += Pseudoconsole_Ready;
            _pseudoconsole.Done += Pseudoconsole_Done;

            StartPseudoconsole();

            Icon = moduleConfig.Icon;
            VisualBellIcon = moduleConfig.VisualBellIcon;
        }

        /// <summary>
        /// Binds TermBar's <see cref="Configuration.Json.Config"/> and <see
        /// cref="Configuration.Json.Modules.Terminal"/> to <paramref
        /// name="terminalControl"/>'s <see cref="DependencyProperty"/>s.
        /// </summary>
        /// <param name="terminalControl">A <see
        /// cref="TerminalControl"/>.</param>
        internal void BindSettings(TerminalControl terminalControl)
        {
            terminalControl.DefaultWindowTitle = _moduleConfig.DefaultWindowTitle;

            terminalControl.FontFamily = _moduleConfig.FontFamily is not null
                ? _moduleConfig.FontFamily
                : _config.FontFamily;

            terminalControl.FontSize = _moduleConfig.FontSize is not null
                ? (double)_moduleConfig.FontSize
                : _config.FontSize;

            if (_moduleConfig.TabWidth is not null)
            {
                terminalControl.TabWidth = (int)_moduleConfig.TabWidth;
            }

            if (_moduleConfig.TextAntialiasing is not null)
            {
                terminalControl.TextAntialiasing = (TextAntialiasingStyle)_moduleConfig.TextAntialiasing;
            }

            if (_moduleConfig.FullColorEmoji is not null)
            {
                terminalControl.FullColorEmoji = (bool)_moduleConfig.FullColorEmoji;
            }

            if (_moduleConfig.UseBackgroundColorErase is not null)
            {
                terminalControl.UseBackgroundColorErase = (bool)_moduleConfig.UseBackgroundColorErase;
            }

            if (_moduleConfig.BackgroundIsInvisible is not null)
            {
                terminalControl.BackgroundIsInvisible = (bool)_moduleConfig.BackgroundIsInvisible;
            }

            if (_moduleConfig.UseVisualBell is not null)
            {
                terminalControl.UseVisualBell = (bool)_moduleConfig.UseVisualBell;
            }

            if (_moduleConfig.UseContextMenu is not null)
            {
                terminalControl.UseContextMenu = (bool)_moduleConfig.UseContextMenu;
            }

            if (_moduleConfig.UseExtendedContextMenu is not null)
            {
                terminalControl.UseExtendedContextMenu = (bool)_moduleConfig.UseExtendedContextMenu;
            }

            if (_moduleConfig.CursorStyle is not null)
            {
                terminalControl.CursorStyle = (CursorStyle)_moduleConfig.CursorStyle;
            }

            if (_moduleConfig.CursorThickness is not null)
            {
                terminalControl.CursorThickness = (double)_moduleConfig.CursorThickness;
            }

            if (_moduleConfig.CursorBlink is not null)
            {
                terminalControl.CursorBlink = (bool)_moduleConfig.CursorBlink;
            }

            if (_moduleConfig.CursorBlinkRate is not null)
            {
                terminalControl.CursorBlinkRate = (int)_moduleConfig.CursorBlinkRate;
            }

            if (_moduleConfig.CursorColor is not null)
            {
                terminalControl.CursorColor = Palette.Instance[_config.Flavor].Colors[_moduleConfig.AccentColor].WUIColor;
            }

            if (_moduleConfig.ScrollbackLines is not null)
            {
                terminalControl.Scrollback = (int)_moduleConfig.ScrollbackLines;
            }

            if (_moduleConfig.LinesPerScrollback is not null)
            {
                terminalControl.LinesPerScrollback = (int)_moduleConfig.LinesPerScrollback;
            }

            if (_moduleConfig.LinesPerSmallScrollback is not null)
            {
                terminalControl.LinesPerSmallScrollback = (int)_moduleConfig.LinesPerSmallScrollback;
            }

            if (_moduleConfig.LinesPerWheelScrollback is not null)
            {
                terminalControl.LinesPerWheelScrollback = (int)_moduleConfig.LinesPerWheelScrollback;
            }

            if (_moduleConfig.CopyOnMouseUp is not null)
            {
                terminalControl.CopyOnMouseUp = (bool)_moduleConfig.CopyOnMouseUp;
            }

            if (_moduleConfig.PasteOnMiddleClick is not null)
            {
                terminalControl.PasteOnMiddleClick = (bool)_moduleConfig.PasteOnMiddleClick;
            }

            if (_moduleConfig.PasteOnRightClick is not null)
            {
                terminalControl.PasteOnRightClick = (bool)_moduleConfig.PasteOnRightClick;
            }

            if (_moduleConfig.CopyNewline is not null)
            {
                terminalControl.CopyNewline = (string)_moduleConfig.CopyNewline;
            }

            _callbackTokens.Add(
                TerminalControl.DefaultWindowTitleProperty,
                terminalControl.RegisterPropertyChangedCallback(
                    TerminalControl.DefaultWindowTitleProperty,
                    (_, _) => _moduleConfig.DefaultWindowTitle = terminalControl.DefaultWindowTitle
                )
            );

            _callbackTokens.Add(
                TerminalControl.RowsProperty,
                terminalControl.RegisterPropertyChangedCallback(
                    TerminalControl.RowsProperty,
                    (_, _) => _moduleConfig.Rows = terminalControl.Rows
                )
            );

            _callbackTokens.Add(
                TerminalControl.ColumnsProperty,
                terminalControl.RegisterPropertyChangedCallback(
                    TerminalControl.ColumnsProperty,
                    (_, _) => _moduleConfig.Columns = terminalControl.Columns
                )
            );

            _callbackTokens.Add(
                TerminalControl.TabWidthProperty,
                terminalControl.RegisterPropertyChangedCallback(
                    TerminalControl.TabWidthProperty,
                    (_, _) => _moduleConfig.TabWidth = terminalControl.TabWidth
                )
            );

            _callbackTokens.Add(
                TerminalControl.FontFamilyProperty,
                terminalControl.RegisterPropertyChangedCallback(
                    TerminalControl.FontFamilyProperty,
                    (_, _) => _moduleConfig.FontFamily = terminalControl.FontFamily
                )
            );

            _callbackTokens.Add(
                TerminalControl.FontSizeProperty,
                terminalControl.RegisterPropertyChangedCallback(
                    TerminalControl.FontSizeProperty,
                    (_, _) => _moduleConfig.FontSize = terminalControl.FontSize
                )
            );

            _callbackTokens.Add(
                TerminalControl.TextAntialiasingProperty,
                terminalControl.RegisterPropertyChangedCallback(
                    TerminalControl.TextAntialiasingProperty,
                    (_, _) => _moduleConfig.TextAntialiasing = terminalControl.TextAntialiasing
                )
            );

            _callbackTokens.Add(
                TerminalControl.FullColorEmojiProperty,
                terminalControl.RegisterPropertyChangedCallback(
                    TerminalControl.FullColorEmojiProperty,
                    (_, _) => _moduleConfig.FullColorEmoji = terminalControl.FullColorEmoji
                )
            );

            _callbackTokens.Add(
                TerminalControl.UseBackgroundColorEraseProperty,
                terminalControl.RegisterPropertyChangedCallback(
                    TerminalControl.UseBackgroundColorEraseProperty,
                    (_, _) => _moduleConfig.UseBackgroundColorErase = terminalControl.UseBackgroundColorErase
                )
            );

            _callbackTokens.Add(
                TerminalControl.BackgroundIsInvisibleProperty,
                terminalControl.RegisterPropertyChangedCallback(
                    TerminalControl.BackgroundIsInvisibleProperty,
                    (_, _) => _moduleConfig.BackgroundIsInvisible = terminalControl.BackgroundIsInvisible
                )
            );

            _callbackTokens.Add(
                TerminalControl.UseVisualBellProperty,
                terminalControl.RegisterPropertyChangedCallback(
                    TerminalControl.UseVisualBellProperty,
                    (_, _) => _moduleConfig.UseVisualBell = terminalControl.UseVisualBell
                )
            );

            _callbackTokens.Add(
                TerminalControl.UseContextMenuProperty,
                terminalControl.RegisterPropertyChangedCallback(
                    TerminalControl.UseContextMenuProperty,
                    (_, _) => _moduleConfig.UseContextMenu = terminalControl.UseContextMenu
                )
            );

            _callbackTokens.Add(
                TerminalControl.UseExtendedContextMenuProperty,
                terminalControl.RegisterPropertyChangedCallback(
                    TerminalControl.UseExtendedContextMenuProperty,
                    (_, _) => _moduleConfig.UseExtendedContextMenu = terminalControl.UseExtendedContextMenu
                )
            );

            _callbackTokens.Add(
                TerminalControl.CursorStyleProperty,
                terminalControl.RegisterPropertyChangedCallback(
                    TerminalControl.CursorStyleProperty,
                    (_, _) => _moduleConfig.CursorStyle = terminalControl.CursorStyle
                )
            );

            _callbackTokens.Add(
                TerminalControl.CursorThicknessProperty,
                terminalControl.RegisterPropertyChangedCallback(
                    TerminalControl.CursorThicknessProperty,
                    (_, _) => _moduleConfig.CursorThickness = terminalControl.CursorThickness
                )
            );

            _callbackTokens.Add(
                TerminalControl.CursorBlinkProperty,
                terminalControl.RegisterPropertyChangedCallback(
                    TerminalControl.CursorBlinkProperty,
                    (_, _) => _moduleConfig.CursorBlink = terminalControl.CursorBlink
                )
            );

            _callbackTokens.Add(
                TerminalControl.CursorBlinkRateProperty,
                terminalControl.RegisterPropertyChangedCallback(
                    TerminalControl.CursorBlinkRateProperty,
                    (_, _) => _moduleConfig.CursorBlinkRate = terminalControl.CursorBlinkRate
                )
            );

            _callbackTokens.Add(
                TerminalControl.CursorColorProperty,
                terminalControl.RegisterPropertyChangedCallback(
                    TerminalControl.CursorColorProperty,
                    (_, _) =>
                    {
                        _moduleConfig.CursorColor = Palette.Instance[_config.Flavor].Colors.FromRGB(
                            terminalControl.CursorColor.R,
                            terminalControl.CursorColor.G,
                            terminalControl.CursorColor.B
                        );
                    }
                )
            );

            _callbackTokens.Add(
                TerminalControl.ScrollbackProperty,
                terminalControl.RegisterPropertyChangedCallback(
                    TerminalControl.ScrollbackProperty,
                    (_, _) => _moduleConfig.ScrollbackLines = terminalControl.Scrollback
                )
            );

            _callbackTokens.Add(
                TerminalControl.LinesPerScrollbackProperty,
                terminalControl.RegisterPropertyChangedCallback(
                    TerminalControl.LinesPerScrollbackProperty,
                    (_, _) => _moduleConfig.LinesPerScrollback = terminalControl.LinesPerScrollback
                )
            );

            _callbackTokens.Add(
                TerminalControl.LinesPerSmallScrollbackProperty,
                terminalControl.RegisterPropertyChangedCallback(
                    TerminalControl.LinesPerSmallScrollbackProperty,
                    (_, _) => _moduleConfig.LinesPerSmallScrollback = terminalControl.LinesPerSmallScrollback
                )
            );

            _callbackTokens.Add(
                TerminalControl.LinesPerWheelScrollbackProperty,
                terminalControl.RegisterPropertyChangedCallback(
                    TerminalControl.LinesPerWheelScrollbackProperty,
                    (_, _) => _moduleConfig.LinesPerWheelScrollback = terminalControl.LinesPerWheelScrollback
                )
            );

            _callbackTokens.Add(
                TerminalControl.CopyOnMouseUpProperty,
                terminalControl.RegisterPropertyChangedCallback(
                    TerminalControl.CopyOnMouseUpProperty,
                    (_, _) => _moduleConfig.CopyOnMouseUp = terminalControl.CopyOnMouseUp
                )
            );

            _callbackTokens.Add(
                TerminalControl.PasteOnMiddleClickProperty,
                terminalControl.RegisterPropertyChangedCallback(
                    TerminalControl.PasteOnMiddleClickProperty,
                    (_, _) => _moduleConfig.PasteOnMiddleClick = terminalControl.PasteOnMiddleClick
                )
            );

            _callbackTokens.Add(
                TerminalControl.PasteOnRightClickProperty,
                terminalControl.RegisterPropertyChangedCallback(
                    TerminalControl.PasteOnRightClickProperty,
                    (_, _) => _moduleConfig.PasteOnRightClick = terminalControl.PasteOnRightClick
                )
            );

            _callbackTokens.Add(
                TerminalControl.CopyNewlineProperty,
                terminalControl.RegisterPropertyChangedCallback(
                    TerminalControl.CopyNewlineProperty,
                    (_, _) => _moduleConfig.CopyNewline = terminalControl.CopyNewline
                )
            );
        }

        /// <summary>
        /// Starts the pseudoconsole, checking for error conditions.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        private async void StartPseudoconsole()
        {
            try
            {
                await Task.Run(_pseudoconsole.Start);
            }
            catch (Win32Exception e)
            {
                PseudoconsoleDied?.Invoke(new ArgumentException(string.Format(App.ResourceLoader.GetString("FailedToStartPseudoconsole"), _pseudoconsole.Command), e));
            }
        }

        /// <summary>
        /// Attaches output and input streams.
        /// </summary>
        private void Pseudoconsole_Ready()
        {
            ConsoleOutput = _pseudoconsole.ConsoleOutStream;
            ConsoleInput = _pseudoconsole.ConsoleInStream;
        }

        /// <summary>
        /// Prints an exit message if the pseudoconsole process exited with
        /// non-zero.
        /// </summary>
        /// <param name="exitCode">The exit code of the command that
        /// executed.</param>
        private void Pseudoconsole_Done(uint exitCode)
        {
            bool printMessage = !RestartOnExit || exitCode != 0;

            if (printMessage)
            {
                _terminalView.Write(string.Format(App.ResourceLoader.GetString("PseudoconsoleExitedWith"), _pseudoconsole.Command, exitCode));
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? callerMemberName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerMemberName));
    }
}