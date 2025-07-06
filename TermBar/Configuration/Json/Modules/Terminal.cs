using TermBar.Catppuccin;
using Terminal;

namespace TermBar.Configuration.Json.Modules {
  /// <summary>
  /// A TermBar terminal configuration.
  /// </summary>
  /// <remarks>For all nullable properties: <see langword="null"/> means the
  /// default TerminalControl behavior takes effect.</remarks>
  internal class Terminal : IModule {
    public int Order { get; set; } = 0;

    public bool Expand { get; set; } = false;

    /// <summary>
    /// The Catppuccin color to use for the terminal icon.
    /// </summary>
    public ColorEnum AccentColor { get; set; } = ColorEnum.Lavender;

    /// <summary>
    /// The text to use as the terminal icon.
    /// </summary>
    public string Icon { get; set; } = "";

    /// <summary>
    /// The text to use as the terminal visual bell icon.
    /// </summary>
    /// <remarks>Not used if the visual bell is disabled.</remarks>
    public string VisualBellIcon { get; set; } = "";

    /// <summary>
    /// The command to run in the terminal.
    /// </summary>
    /// <remarks>This is typically a shell.</remarks>
    public string Command { get; set; } = "pwsh.exe";

    /// <summary>
    /// Whether to restart <see cref="Command"/> when it exits normally.
    /// </summary>
    public bool RestartOnExit { get; set; } = true;

    /// <summary>
    /// The default window title.
    /// </summary>
    /// <remarks>The window title can be viewed by mousing over the terminal
    /// icon.</remarks>
    public string DefaultWindowTitle { get; set; } = "TermBar";

    /// <summary>
    /// The number of rows in the terminal.
    /// </summary>
    public uint Rows { get; set; } = 3;

    /// <summary>
    /// The number of columns in the terminal.
    /// </summary>
    public uint Columns { get; set; } = 60;

    /// <summary>
    /// The default tab width in the terminal.
    /// </summary>
    /// <remarks>Practically speaking, this is virtually irrelevant
    /// today.</remarks>
    public int? TabWidth { get; set; } = 8;

    /// <summary>
    /// The text antialiasing style to apply.
    /// </summary>
    /// <remarks>Unicode box-drawing characters are always drawn without
    /// antialiasing.</remarks>
    public TextAntialiasingStyles? TextAntialiasing { get; set; } = TextAntialiasingStyles.Grayscale;

    /// <summary>
    /// Whether to draw emoji in full color using the Segoe UI Emoji font.
    /// </summary>
    public bool? FullColorEmoji { get; set; } = false;

    /// <summary>
    /// Whether to use background color erase.
    /// </summary>
    /// <remarks>See <see
    /// href="https://sunaku.github.io/vim-256color-bce.html"/> for a
    /// description of what this does.</remarks>
    public bool? UseBackgroundColorErase { get; set; } = true;

    /// <summary>
    /// Whether to render the default background color as invisible.
    /// </summary>
    public bool? BackgroundIsInvisible { get; set; } = true;

    /// <summary>
    /// Whether to use the visual bell rather than playing a sound when the
    /// terminal bell rings.
    /// </summary>
    /// <remarks>The bell causes <see cref="VisualBellIcon"/> to be displayed
    /// for <see cref="VisualBellDisplayTime"/> milliseconds.</remarks>
    public bool? UseVisualBell { get; set; } = true;

    /// <summary>
    /// The number of milliseconds to display <see cref="VisualBellIcon"/> when
    /// the visual bell rings.
    /// </summary>
    /// <remarks>Does nothing if <see cref="UseVisualBell"/> is <see
    /// langword="false"/>.</remarks>
    public int VisualBellDisplayTime { get; set; } = 1000;

    /// <summary>
    /// Whether to show the context menu when right-clicking the terminal.
    /// </summary>
    public bool? UseContextMenu { get; set; } = true;

    /// <summary>
    /// Whether to show the extended context menu when right-clicking the
    /// terminal.
    /// </summary>
    /// <remarks>Does nothing if <see cref="UseContextMenu"/> is <see
    /// langword="false"/>.</remarks>
    public bool? UseExtendedContextMenu { get; set; } = true;

    /// <summary>
    /// The cursor style to use by default.
    /// </summary>
    public CursorStyles? CursorStyle { get; set; } = CursorStyles.Underline;

    /// <summary>
    /// The cursor thickness, as a fraction of the font size, between 0.0 and
    /// 1.0.
    /// </summary>
    /// <remarks>Does not affect the block cursor.</remarks>
    public double? CursorThickness { get; set; } = 0.1;

    /// <summary>
    /// Whether the cursor should blink by default.
    /// </summary>
    public bool? CursorBlink { get; set; } = true;

    /// <summary>
    /// The number of milliseconds that pass between the blinking cursor being
    /// displayed and not displayed.
    /// </summary>
    /// <remarks>Has no effect if <see cref="CursorBlink"/> is <see
    /// langword="false"/>.</remarks>
    public int? CursorBlinkRate { get; set; } = 500;

    /// <summary>
    /// The cursor color.
    /// </summary>
    public ColorEnum? CursorColor { get; set; } = ColorEnum.Text;

    /// <summary>
    /// The number of scrollback lines.
    /// </summary>
    public int? ScrollbackLines { get; set; } = 5000;

    /// <summary>
    /// The number of lines to scroll per large scrollback.
    /// </summary>
    /// <remarks>A large scrollback is invoked with Shift-Page Up and reversed
    /// with Shift-Page Down.</remarks>
    public int? LinesPerScrollback { get; set; } = 12;

    /// <summary>
    /// The number of lines to scroll per small scrollback.
    /// </summary>
    /// <remarks>A large scrollback is invoked with Shift-Up and reversed with
    /// Shift-Down.</remarks>
    public int? LinesPerSmallScrollback { get; set; } = 1;

    /// <summary>
    /// The number of lines to scroll per mouse wheel scrollback.
    /// </summary>
    /// <remarks>A mouse wheel scrollback is invoked with mouse wheel up and
    /// reversed with mouse wheel down.</remarks>
    public int? LinesPerWheelScrollback { get; set; } = 8;

    /// <summary>
    /// Whether to copy selected terminal text when the left mouse button is
    /// released.
    /// </summary>
    /// <remarks>If this is <see langword="false"/>, the selection can be
    /// copied with Ctrl-Shift-C.</remarks>
    public bool? CopyOnMouseUp { get; set; } = true;

    /// <summary>
    /// Whether to paste from the clipboard when the terminal is middle
    /// clicked.
    /// </summary>
    public bool? PasteOnMiddleClick { get; set; } = true;

    /// <summary>
    /// Whether to paste from the clipboard when the terminal is right
    /// clicked.
    /// </summary>
    /// <remarks>If this is <see langword="true"/>, the context menu can be
    /// accessed with Ctrl-Right Click (if <see cref="UseContextMenu"/> is
    /// <see langword="true"/>).</remarks>
    public bool? PasteOnRightClick { get; set; } = false;

    /// <summary>
    /// The line ending to use for lines copied from the terminal.
    /// </summary>
    public string? CopyNewline { get; set; } = "\r\n";

    /// <summary>
    /// The ANSI colors to use for the terminal.
    /// </summary>
    public TerminalColors Colors { get; set; } = new();
  }

  /// <summary>
  /// A TermBar terminal colors configuration.
  /// </summary>
  internal class TerminalColors {
    /// <summary>
    /// The terminal default ANSI colors.
    /// </summary>
    public DefaultColors DefaultColors { get; set; } = new();

    /// <summary>
    /// The terminal standard ANSI colors.
    /// </summary>
    public StandardColors StandardColors { get; set; } = new();

    /// <summary>
    /// The terminal bright ANSI colors.
    /// </summary>
    public BrightColors BrightColors { get; set; } = new();
  }

  /// <summary>
  /// A TermBar terminal default colors configuration.
  /// </summary>
  internal class DefaultColors {
    /// <summary>
    /// The default background color.
    /// </summary>
    public AnsiColorEnum DefaultBackgroundColor { get; set; } = AnsiColorEnum.Black;

    /// <summary>
    /// The default foreground color.
    /// </summary>
    public AnsiColorEnum DefaultForegroundColor { get; set; } = AnsiColorEnum.White;

    /// <summary>
    /// The default underline color.
    /// </summary>
    public AnsiColorEnum DefaultUnderlineColor { get; set; } = AnsiColorEnum.White;
  }

  /// <summary>
  /// A TermBar terminal standard colors configuration.
  /// </summary>
  internal class StandardColors {
    /// <summary>
    /// Black.
    /// </summary>
    public AnsiColorEnum Black { get; set; } = AnsiColorEnum.Black;

    /// <summary>
    /// Red.
    /// </summary>
    public AnsiColorEnum Red { get; set; } = AnsiColorEnum.Red;

    /// <summary>
    /// Green.
    /// </summary>
    public AnsiColorEnum Green { get; set; } = AnsiColorEnum.Green;

    /// <summary>
    /// Yellow.
    /// </summary>
    public AnsiColorEnum Yellow { get; set; } = AnsiColorEnum.Yellow;

    /// <summary>
    /// Blue.
    /// </summary>
    public AnsiColorEnum Blue { get; set; } = AnsiColorEnum.Blue;

    /// <summary>
    /// Magenta.
    /// </summary>
    public AnsiColorEnum Magenta { get; set; } = AnsiColorEnum.Magenta;

    /// <summary>
    /// Cyan.
    /// </summary>
    public AnsiColorEnum Cyan { get; set; } = AnsiColorEnum.Cyan;

    /// <summary>
    /// White.
    /// </summary>
    public AnsiColorEnum White { get; set; } = AnsiColorEnum.White;
  }

  /// <summary>
  /// A TermBar terminal bright colors configuration.
  /// </summary>
  internal class BrightColors {
    /// <summary>
    /// Bright black.
    /// </summary>
    public AnsiColorEnum BrightBlack { get; set; } = AnsiColorEnum.BrightBlack;

    /// <summary>
    /// Bright red.
    /// </summary>
    public AnsiColorEnum BrightRed { get; set; } = AnsiColorEnum.BrightRed;

    /// <summary>
    /// Bright green.
    /// </summary>
    public AnsiColorEnum BrightGreen { get; set; } = AnsiColorEnum.BrightGreen;

    /// <summary>
    /// Bright yellow.
    /// </summary>
    public AnsiColorEnum BrightYellow { get; set; } = AnsiColorEnum.BrightYellow;

    /// <summary>
    /// Bright blue.
    /// </summary>
    public AnsiColorEnum BrightBlue { get; set; } = AnsiColorEnum.BrightBlue;

    /// <summary>
    /// Bright magenta.
    /// </summary>
    public AnsiColorEnum BrightMagenta { get; set; } = AnsiColorEnum.BrightMagenta;

    /// <summary>
    /// Bright cyan.
    /// </summary>
    public AnsiColorEnum BrightCyan { get; set; } = AnsiColorEnum.BrightCyan;

    /// <summary>
    /// Bright white.
    /// </summary>
    public AnsiColorEnum BrightWhite { get; set; } = AnsiColorEnum.BrightWhite;
  }
}
