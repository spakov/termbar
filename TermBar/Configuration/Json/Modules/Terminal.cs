using Catppuccin;
using TermBar.Configuration.Json.SchemaAttributes;
using Terminal;

namespace TermBar.Configuration.Json.Modules {
  [Description("A TermBar terminal configuration. For all objects that accept a null value, a value of null means the default TerminalControl behavior takes effect, unless otherwise noted.")]
  internal class Terminal : IModule {
    private const int orderDefault = 0;
    private const bool expandDefault = false;
    private const ColorEnum accentColorDefault = ColorEnum.Lavender;
    private const string accentColorDefaultAsString = "Lavender";
    private const string iconDefault = "";
    private const string visualBellIconDefault = "";
    private const string commandDefault = "powershell";
    private const bool restartOnExitDefault = true;
    private const string defaultWindowTitleDefault = "TermBar";
    private const int rowsDefault = 3;
    private const int columnsDefault = 60;
    private const int tabWidthDefault = 8;
    private const TextAntialiasingStyles textAntialiasingDefault = TextAntialiasingStyles.Grayscale;
    private const string textAntialiasingDefaultAsString = "Grayscale";
    private const bool fullColorEmojiDefault = false;
    private const bool useBackgroundColorEraseDefault = true;
    private const bool backgroundIsInvisibleDefault = true;
    private const bool useVisualBellDefault = true;
    private const int visualBellDisplayTimeDefault = 1000;
    private const bool useContextMenuDefault = true;
    private const bool useExtendedContextMenuDefault = true;
    private const CursorStyles cursorStyleDefault = CursorStyles.Underline;
    private const string cursorStyleDefaultAsString = "Underline";
    private const double cursorThicknessDefault = 0.1;
    private const bool cursorBlinkDefault = true;
    private const int cursorBlinkRateDefault = 500;
    private const ColorEnum cursorColorDefault = ColorEnum.Text;
    private const string cursorColorDefaultAsString = "Text";
    private const int scrollbackLinesDefault = 5000;
    private const int linesPerScrollbackDefault = 3;
    private const int linesPerSmallScrollbackDefault = 1;
    private const int linesPerWheelScrollbackDefault = 1;
    private const bool copyOnMouseUpDefault = true;
    private const bool pasteOnMiddleClickDefault = true;
    private const bool pasteOnRightClickDefault = false;
    private const string copyNewlineDefault = "\r\n";

    [Description("The order in which the module should be displayed on the TermBar.")]
    [DefaultIntNumber(orderDefault)]
    [MinimumInt(int.MinValue)]
    [MaximumInt(int.MaxValue)]
    public int Order { get; set; } = orderDefault;

    [Description("Whether the module should expand to take up as much space as possible.")]
    [DefaultBoolean(expandDefault)]
    public bool Expand { get; set; } = expandDefault;

    [Description("The Catppuccin color to use as an accent.")]
    [DefaultString(accentColorDefaultAsString)]
    public ColorEnum AccentColor { get; set; } = accentColorDefault;

    [Description("The text to use as the terminal icon.")]
    [DefaultString(iconDefault)]
    public string Icon { get; set; } = iconDefault;

    [Description("The text to use as the terminal visual bell icon when the visual bell is ringing. Has no effect if UseVisualBell is false.")]
    [DefaultString(visualBellIconDefault)]
    public string VisualBellIcon { get; set; } = visualBellIconDefault;

    [Description("The command to run in the terminal. This is typically a shell.")]
    [DefaultString(commandDefault)]
    public string Command { get; set; } = commandDefault;

    [Description("Whether to restart Command when it exits normally.")]
    [DefaultBoolean(restartOnExitDefault)]
    public bool RestartOnExit { get; set; } = restartOnExitDefault;

    [Description("The default terminal window title. The terminal window title can be viewed as a tooltip of the terminal icon.")]
    [DefaultString(defaultWindowTitleDefault)]
    public string DefaultWindowTitle { get; set; } = defaultWindowTitleDefault;

    [Description("The number of rows in the terminal.")]
    [DefaultIntNumber(rowsDefault)]
    [MinimumInt(1)]
    public int Rows { get; set; } = rowsDefault;

    [Description("The number of columns in the terminal.")]
    [DefaultIntNumber(columnsDefault)]
    [MinimumInt(1)]
    public int Columns { get; set; } = columnsDefault;

    [Description("The default tab width in the terminal. (Practically speaking, this is virtually irrelevant today.)")]
    [DefaultIntNumber(tabWidthDefault)]
    [MinimumInt(1)]
    public int? TabWidth { get; set; } = tabWidthDefault;

    [Description("The text antialiasing style to apply in the terminal. Unicode box-drawing characters are always drawn without antialiasing.")]
    [DefaultString(textAntialiasingDefaultAsString)]
    public TextAntialiasingStyles? TextAntialiasing { get; set; } = textAntialiasingDefault;

    [Description("Whether to draw emoji in full color in the terminal using the Segoe UI Emoji font.")]
    [DefaultBoolean(fullColorEmojiDefault)]
    public bool? FullColorEmoji { get; set; } = fullColorEmojiDefault;

    [Description("Whether to use background color erase in the terminal. See https://sunaku.github.io/vim-256color-bce.html for a description of what this does.")]
    [DefaultBoolean(useBackgroundColorEraseDefault)]
    public bool? UseBackgroundColorErase { get; set; } = useBackgroundColorEraseDefault;

    [Description("Whether to render the default background color as invisible in the terminal.")]
    [DefaultBoolean(backgroundIsInvisibleDefault)]
    public bool? BackgroundIsInvisible { get; set; } = backgroundIsInvisibleDefault;

    [Description("Whether to use the visual bell in the terminal. If this is true, VisualBellIcon is displayed for VisualBellDisplayTime milliseconds when the terminal bell rings. If this is false, a sound plays instead.")]
    [DefaultBoolean(useVisualBellDefault)]
    public bool? UseVisualBell { get; set; } = useVisualBellDefault;

    [Description("The number of milliseconds to display the visual bell in the terminal. Does nothing if UseVisualBell is false.")]
    [DefaultIntNumber(visualBellDisplayTimeDefault)]
    [MinimumInt(1)]
    public int VisualBellDisplayTime { get; set; } = visualBellDisplayTimeDefault;

    [Description("Whether to show the context menu when right clicking the terminal.")]
    [DefaultBoolean(useContextMenuDefault)]
    public bool? UseContextMenu { get; set; } = useContextMenuDefault;

    [Description("Whether to show the extended context menu when right clicking the terminal. Does nothing if UseContextMenu is false.")]
    [DefaultBoolean(useExtendedContextMenuDefault)]
    public bool? UseExtendedContextMenu { get; set; } = useExtendedContextMenuDefault;

    [Description("The default cursor style to use in the terminal.")]
    [DefaultString(cursorStyleDefaultAsString)]
    public CursorStyles? CursorStyle { get; set; } = cursorStyleDefault;

    [Description("The cursor thickness in the terminal, as a fraction of the font size, between 0.0 and 1.0. Does not affect the block cursor.")]
    [DefaultDoubleNumber(cursorThicknessDefault)]
    [MinimumDouble(0.0)]
    [MaximumDouble(1.0)]
    public double? CursorThickness { get; set; } = cursorThicknessDefault;

    [Description("Whether the cursor should blink by default in the terminal.")]
    [DefaultBoolean(cursorBlinkDefault)]
    public bool? CursorBlink { get; set; } = cursorBlinkDefault;

    [Description("The number of milliseconds that pass between the blinking cursor being displayed in the terminal and hidden. Has no effect if CursorBlink is false.")]
    [DefaultIntNumber(cursorBlinkRateDefault)]
    [MinimumInt(1)]
    public int? CursorBlinkRate { get; set; } = cursorBlinkRateDefault;

    [Description("The cursor color in the terminal.")]
    [DefaultString(cursorColorDefaultAsString)]
    public ColorEnum? CursorColor { get; set; } = cursorColorDefault;

    [Description("The number of scrollback lines in the terminal.")]
    [DefaultIntNumber(scrollbackLinesDefault)]
    [MinimumInt(0)]
    public int? ScrollbackLines { get; set; } = scrollbackLinesDefault;

    [Description("The number of scrollback lines per large scrollback in the terminal. A large scrollback is invoked with Shift-Page Up and reversed with Shift-Page Down.")]
    [DefaultIntNumber(linesPerScrollbackDefault)]
    [MinimumInt(1)]
    public int? LinesPerScrollback { get; set; } = linesPerScrollbackDefault;

    [Description("The number of scrollback lines per small scrollback in the terminal. A small scrollback is invoked with Shift-Up and reversed with Shift-Down.")]
    [DefaultIntNumber(linesPerSmallScrollbackDefault)]
    [MinimumInt(1)]
    public int? LinesPerSmallScrollback { get; set; } = linesPerSmallScrollbackDefault;

    [Description("The number of scrollback lines per mouse wheel scroll in the terminal. A mouse wheel scroll is invoked with mouse wheel up and reversed with mouse wheel down.")]
    [DefaultIntNumber(linesPerWheelScrollbackDefault)]
    [MinimumInt(1)]
    public int? LinesPerWheelScrollback { get; set; } = linesPerWheelScrollbackDefault;

    [Description("Whether to copy selected text in the terminal when the left mouse button is released. If this is false, the selection can be copied with Ctrl-Shift-C.")]
    [DefaultBoolean(copyOnMouseUpDefault)]
    public bool? CopyOnMouseUp { get; set; } = copyOnMouseUpDefault;

    [Description("Whether to paste text in the terminal when the terminal is middle clicked. Text can also be pasted with Ctrl-Shift-V.")]
    [DefaultBoolean(pasteOnMiddleClickDefault)]
    public bool? PasteOnMiddleClick { get; set; } = pasteOnMiddleClickDefault;

    [Description("Whether to paste text in the terminal when the terminal is right clicked. Text can also be pasted with Ctrl-Shift-V. If this is true, the context menu can be displayed by right clicking the terminal while pressing Ctrl (assuming UseContextMenu is true).")]
    [DefaultBoolean(pasteOnRightClickDefault)]
    public bool? PasteOnRightClick { get; set; } = pasteOnRightClickDefault;

    [Description("The line ending to use when copying text in the terminal.")]
    [DefaultString(copyNewlineDefault)]
    public string? CopyNewline { get; set; } = copyNewlineDefault;

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
    private const AnsiColorEnum defaultBackgroundColorDefault = AnsiColorEnum.Black;
    private const string defaultBackgroundColorDefaultAsString = "Black";
    private const AnsiColorEnum defaultForegroundColorDefault = AnsiColorEnum.White;
    private const string defaultForegroundColorDefaultAsString = "White";
    private const AnsiColorEnum defaultUnderlineColorDefault = AnsiColorEnum.White;
    private const string defaultUnderlineColorDefaultAsString = "White";

    [Description("The default background color to use in the terminal.")]
    [DefaultString(defaultBackgroundColorDefaultAsString)]
    public AnsiColorEnum DefaultBackgroundColor { get; set; } = defaultBackgroundColorDefault;

    [Description("The default foreground color to use in the terminal.")]
    [DefaultString(defaultForegroundColorDefaultAsString)]
    public AnsiColorEnum DefaultForegroundColor { get; set; } = defaultForegroundColorDefault;

    [Description("The default underline color to use in the terminal.")]
    [DefaultString(defaultUnderlineColorDefaultAsString)]
    public AnsiColorEnum DefaultUnderlineColor { get; set; } = defaultUnderlineColorDefault;
  }

  [Description("Terminal standard (non-bright) ANSI colors.")]
  internal class StandardColors {
    private const AnsiColorEnum blackDefault = AnsiColorEnum.Black;
    private const string blackDefaultAsString = "Black";
    private const AnsiColorEnum redDefault = AnsiColorEnum.Red;
    private const string redDefaultAsString = "Red";
    private const AnsiColorEnum greenDefault = AnsiColorEnum.Green;
    private const string greenDefaultAsString = "Green";
    private const AnsiColorEnum yellowDefault = AnsiColorEnum.Yellow;
    private const string yellowDefaultAsString = "Yellow";
    private const AnsiColorEnum blueDefault = AnsiColorEnum.Blue;
    private const string blueDefaultAsString = "Blue";
    private const AnsiColorEnum magentaDefault = AnsiColorEnum.Magenta;
    private const string magentaDefaultAsString = "Magenta";
    private const AnsiColorEnum cyanDefault = AnsiColorEnum.Cyan;
    private const string cyanDefaultAsString = "Cyan";
    private const AnsiColorEnum whiteDefault = AnsiColorEnum.White;
    private const string whiteDefaultAsString = "White";

    [Description("Black.")]
    [DefaultString(blackDefaultAsString)]
    public AnsiColorEnum Black { get; set; } = blackDefault;

    [Description("Red.")]
    [DefaultString(redDefaultAsString)]
    public AnsiColorEnum Red { get; set; } = redDefault;

    [Description("Green.")]
    [DefaultString(greenDefaultAsString)]
    public AnsiColorEnum Green { get; set; } = greenDefault;

    [Description("Yellow.")]
    [DefaultString(yellowDefaultAsString)]
    public AnsiColorEnum Yellow { get; set; } = yellowDefault;

    [Description("Blue.")]
    [DefaultString(blueDefaultAsString)]
    public AnsiColorEnum Blue { get; set; } = blueDefault;

    [Description("Magenta.")]
    [DefaultString(magentaDefaultAsString)]
    public AnsiColorEnum Magenta { get; set; } = magentaDefault;

    [Description("Cyan.")]
    [DefaultString(cyanDefaultAsString)]
    public AnsiColorEnum Cyan { get; set; } = cyanDefault;

    [Description("White.")]
    [DefaultString(whiteDefaultAsString)]
    public AnsiColorEnum White { get; set; } = whiteDefault;
  }

  [Description("Terminal bright ANSI colors.")]
  internal class BrightColors {
    private const AnsiColorEnum brightBlackDefault = AnsiColorEnum.BrightBlack;
    private const string brightBlackDefaultAsString = "BrightBlack";
    private const AnsiColorEnum brightRedDefault = AnsiColorEnum.BrightRed;
    private const string brightRedDefaultAsString = "BrightRed";
    private const AnsiColorEnum brightGreenDefault = AnsiColorEnum.BrightGreen;
    private const string brightGreenDefaultAsString = "BrightGreen";
    private const AnsiColorEnum brightYellowDefault = AnsiColorEnum.BrightYellow;
    private const string brightYellowDefaultAsString = "BrightYellow";
    private const AnsiColorEnum brightBlueDefault = AnsiColorEnum.BrightBlue;
    private const string brightBlueDefaultAsString = "BrightBlue";
    private const AnsiColorEnum brightMagentaDefault = AnsiColorEnum.BrightMagenta;
    private const string brightMagentaDefaultAsString = "BrightMagenta";
    private const AnsiColorEnum brightCyanDefault = AnsiColorEnum.BrightCyan;
    private const string brightCyanDefaultAsString = "BrightCyan";
    private const AnsiColorEnum brightWhiteDefault = AnsiColorEnum.BrightWhite;
    private const string brightWhiteDefaultAsString = "BrightWhite";

    [Description("Bright black.")]
    [DefaultString(brightBlackDefaultAsString)]
    public AnsiColorEnum BrightBlack { get; set; } = brightBlackDefault;

    [Description("Bright red.")]
    [DefaultString(brightRedDefaultAsString)]
    public AnsiColorEnum BrightRed { get; set; } = brightRedDefault;

    [Description("Bright green.")]
    [DefaultString(brightGreenDefaultAsString)]
    public AnsiColorEnum BrightGreen { get; set; } = brightGreenDefault;

    [Description("Bright yellow.")]
    [DefaultString(brightYellowDefaultAsString)]
    public AnsiColorEnum BrightYellow { get; set; } = brightYellowDefault;

    [Description("Bright blue.")]
    [DefaultString(brightBlueDefaultAsString)]
    public AnsiColorEnum BrightBlue { get; set; } = brightBlueDefault;

    [Description("Bright magenta.")]
    [DefaultString(brightMagentaDefaultAsString)]
    public AnsiColorEnum BrightMagenta { get; set; } = brightMagentaDefault;

    [Description("Bright cyan.")]
    [DefaultString(brightCyanDefaultAsString)]
    public AnsiColorEnum BrightCyan { get; set; } = brightCyanDefault;

    [Description("Bright white.")]
    [DefaultString(brightWhiteDefaultAsString)]
    public AnsiColorEnum BrightWhite { get; set; } = brightWhiteDefault;
  }
}
