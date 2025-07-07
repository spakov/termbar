using System.ComponentModel;
using TermBar.Catppuccin;
using Terminal;

namespace TermBar.Configuration.Json.Modules {
  [Description("A TermBar terminal configuration. For all objects, a value of null means the default TerminalControl behavior takes effect, unless otherwise noted.")]
  internal class Terminal : IModule {
    [Description("The order in which the module should be displayed on the TermBar.")]
    public int Order { get; set; } = 0;

    [Description("Whether the module should expand to take up as much space as possible.")]
    public bool Expand { get; set; } = false;

    [Description("The Catppuccin color to use as an accent.")]
    public ColorEnum AccentColor { get; set; } = ColorEnum.Lavender;

    [Description("The text to use as the terminal icon.")]
    public string Icon { get; set; } = "";

    [Description("The text to use as the terminal visual bell icon when the visual bell is ringing. Has no effect if UseVisualBell is false.")]
    public string VisualBellIcon { get; set; } = "";

    [Description("The command to run in the terminal. This is typically a shell.")]
    public string Command { get; set; } = "powershell";

    [Description("Whether to restart Command when it exits normally.")]
    public bool RestartOnExit { get; set; } = true;

    [Description("The default terminal window title. The terminal window title can be viewed as a tooltip of the terminal icon.")]
    public string DefaultWindowTitle { get; set; } = "TermBar";

    [Description("The number of rows in the terminal.")]
    public uint Rows { get; set; } = 3;

    [Description("The number of columns in the terminal.")]
    public uint Columns { get; set; } = 60;

    [Description("The default tab width in the terminal. (Practically speaking, this is virtually irrelevant today.)")]
    public int? TabWidth { get; set; } = 8;

    [Description("The text antialiasing style to apply in the terminal. Unicode box-drawing characters are always drawn without antialiasing.")]
    public TextAntialiasingStyles? TextAntialiasing { get; set; } = TextAntialiasingStyles.Grayscale;

    [Description("Whether to draw emoji in full color in the terminal using the Segoe UI Emoji font.")]
    public bool? FullColorEmoji { get; set; } = false;

    [Description("Whether to use background color erase in the terminal. See https://sunaku.github.io/vim-256color-bce.html for a description of what this does.")]
    public bool? UseBackgroundColorErase { get; set; } = true;

    [Description("Whether to render the default background color as invisible in the terminal.")]
    public bool? BackgroundIsInvisible { get; set; } = true;

    [Description("Whether to use the visual bell in the terminal. If this is true, VisualBellIcon is displayed for VisualBellDisplayTime milliseconds when the terminal bell rings. If this is false, a sound plays instead.")]
    public bool? UseVisualBell { get; set; } = true;

    [Description("The number of milliseconds to display the visual bell in the terminal. Does nothing if UseVisualBell is false.")]
    public int VisualBellDisplayTime { get; set; } = 1000;

    [Description("Whether to show the context menu when right clicking the terminal.")]
    public bool? UseContextMenu { get; set; } = true;

    [Description("Whether to show the extended context menu when right clicking the terminal. Does nothing if UseContextMenu is false.")]
    public bool? UseExtendedContextMenu { get; set; } = true;

    [Description("The default cursor style to use in the terminal.")]
    public CursorStyles? CursorStyle { get; set; } = CursorStyles.Underline;

    [Description("The cursor thickness in the terminal, as a fraction of the font size, between 0.0 and 1.0. Does not affect the block cursor.")]
    public double? CursorThickness { get; set; } = 0.1;

    [Description("Whether the cursor should blink by default in the terminal.")]
    public bool? CursorBlink { get; set; } = true;

    [Description("The number of milliseconds that pass between the blinking cursor being displayed in the terminal and hidden. Has no effect if CursorBlink is false.")]
    public int? CursorBlinkRate { get; set; } = 500;

    [Description("The cursor color in the terminal.")]
    public ColorEnum? CursorColor { get; set; } = ColorEnum.Text;

    [Description("The number of scrollback lines in the terminal.")]
    public int? ScrollbackLines { get; set; } = 5000;

    [Description("The number of scrollback lines per large scrollback in the terminal. A large scrollback is invoked with Shift-Page Up and reversed with Shift-Page Down.")]
    public int? LinesPerScrollback { get; set; } = 12;

    [Description("The number of scrollback lines per small scrollback in the terminal. A small scrollback is invoked with Shift-Up and reversed with Shift-Down.")]
    public int? LinesPerSmallScrollback { get; set; } = 1;

    [Description("The number of scrollback lines per mouse wheel scroll in the terminal. A mouse wheel scroll is invoked with mouse wheel up and reversed with mouse wheel down.")]
    public int? LinesPerWheelScrollback { get; set; } = 8;

    [Description("Whether to copy selected text in the terminal when the left mouse button is released. If this is false, the selection can be copied with Ctrl-Shift-C.")]
    public bool? CopyOnMouseUp { get; set; } = true;

    [Description("Whether to paste text in the terminal when the terminal is middle clicked. Text can also be pasted with Ctrl-Shift-V.")]
    public bool? PasteOnMiddleClick { get; set; } = true;

    [Description("Whether to paste text in the terminal when the terminal is right clicked. Text can also be pasted with Ctrl-Shift-V. If this is true, the context menu can be displayed by right clicking the terminal while pressing Ctrl (assuming UseContextMenu is true).")]
    public bool? PasteOnRightClick { get; set; } = false;

    [Description("The line ending to use when copying text in the terminal.")]
    public string? CopyNewline { get; set; } = "\r\n";

    [Description("The ANSI colors to use in the terminal.")]
    public TerminalColors Colors { get; set; } = new();
  }

  [Description("Terminal ANSI colors.")]
  internal class TerminalColors {
    [Description("The default ANSI colors to use in the terminal.")]
    public DefaultColors DefaultColors { get; set; } = new();

    [Description("The standard (non-bright) ANSI colors to use in the terminal.")]
    public StandardColors StandardColors { get; set; } = new();

    [Description("The bright ANSI colors to use in the terminal.")]
    public BrightColors BrightColors { get; set; } = new();
  }

  [Description("Terminal default ANSI colors.")]
  internal class DefaultColors {
    [Description("The default background color to use in the terminal.")]
    public AnsiColorEnum DefaultBackgroundColor { get; set; } = AnsiColorEnum.Black;

    [Description("The default foreground color to use in the terminal.")]
    public AnsiColorEnum DefaultForegroundColor { get; set; } = AnsiColorEnum.White;

    [Description("The default underline color to use in the terminal.")]
    public AnsiColorEnum DefaultUnderlineColor { get; set; } = AnsiColorEnum.White;
  }

  [Description("Terminal standard (non-bright) ANSI colors.")]
  internal class StandardColors {
    [Description("Black.")]
    public AnsiColorEnum Black { get; set; } = AnsiColorEnum.Black;

    [Description("Red.")]
    public AnsiColorEnum Red { get; set; } = AnsiColorEnum.Red;

    [Description("Green.")]
    public AnsiColorEnum Green { get; set; } = AnsiColorEnum.Green;

    [Description("Yellow.")]
    public AnsiColorEnum Yellow { get; set; } = AnsiColorEnum.Yellow;

    [Description("Blue.")]
    public AnsiColorEnum Blue { get; set; } = AnsiColorEnum.Blue;

    [Description("Magenta.")]
    public AnsiColorEnum Magenta { get; set; } = AnsiColorEnum.Magenta;

    [Description("Cyan.")]
    public AnsiColorEnum Cyan { get; set; } = AnsiColorEnum.Cyan;

    [Description("White.")]
    public AnsiColorEnum White { get; set; } = AnsiColorEnum.White;
  }

  [Description("Terminal bright ANSI colors.")]
  internal class BrightColors {
    [Description("Bright black.")]
    public AnsiColorEnum BrightBlack { get; set; } = AnsiColorEnum.BrightBlack;

    [Description("Bright red.")]
    public AnsiColorEnum BrightRed { get; set; } = AnsiColorEnum.BrightRed;

    [Description("Bright green.")]
    public AnsiColorEnum BrightGreen { get; set; } = AnsiColorEnum.BrightGreen;

    [Description("Bright yellow.")]
    public AnsiColorEnum BrightYellow { get; set; } = AnsiColorEnum.BrightYellow;

    [Description("Bright blue.")]
    public AnsiColorEnum BrightBlue { get; set; } = AnsiColorEnum.BrightBlue;

    [Description("Bright magenta.")]
    public AnsiColorEnum BrightMagenta { get; set; } = AnsiColorEnum.BrightMagenta;

    [Description("Bright cyan.")]
    public AnsiColorEnum BrightCyan { get; set; } = AnsiColorEnum.BrightCyan;

    [Description("Bright white.")]
    public AnsiColorEnum BrightWhite { get; set; } = AnsiColorEnum.BrightWhite;
  }
}
