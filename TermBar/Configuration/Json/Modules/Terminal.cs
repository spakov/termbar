using Spakov.Catppuccin;
using Spakov.TermBar.Configuration.Json.SchemaAttributes;
using Spakov.Terminal;

namespace Spakov.TermBar.Configuration.Json.Modules
{
    [Description("A TermBar terminal configuration. For all objects that accept a null value, a value of null means the default TerminalControl behavior takes effect, unless otherwise noted.")]
    internal class Terminal : IModule
    {
        private const int OrderDefault = 0;
        private const bool ExpandDefault = false;
        private const ColorEnum AccentColorDefault = ColorEnum.Lavender;
        private const string AccentColorDefaultAsString = "Lavender";
        private const string IconDefault = "";
        private const string VisualBellIconDefault = "";
        private const string CommandDefault = "powershell";
        private const bool RestartOnExitDefault = true;
        private const string DefaultWindowTitleDefault = "TermBar";
        private const int RowsDefault = 3;
        private const int ColumnsDefault = 60;
        private const int TabWidthDefault = 8;
        private const TextAntialiasingStyle TextAntialiasingDefault = TextAntialiasingStyle.Grayscale;
        private const string TextAntialiasingDefaultAsString = "Grayscale";
        private const bool FullColorEmojiDefault = false;
        private const bool UseBackgroundColorEraseDefault = true;
        private const bool BackgroundIsInvisibleDefault = true;
        private const bool UseVisualBellDefault = true;
        private const int VisualBellDisplayTimeDefault = 1000;
        private const bool UseContextMenuDefault = true;
        private const bool UseExtendedContextMenuDefault = true;
        private const CursorStyle CursorStyleDefault = Spakov.Terminal.CursorStyle.Underline;
        private const string CursorStyleDefaultAsString = "Underline";
        private const double CursorThicknessDefault = 0.1;
        private const bool CursorBlinkDefault = true;
        private const int CursorBlinkRateDefault = 500;
        private const ColorEnum CursorColorDefault = ColorEnum.Text;
        private const string CursorColorDefaultAsString = "Text";
        private const int ScrollbackLinesDefault = 5000;
        private const int LinesPerScrollbackDefault = 3;
        private const int LinesPerSmallScrollbackDefault = 1;
        private const int LinesPerWheelScrollbackDefault = 1;
        private const bool CopyOnMouseUpDefault = true;
        private const bool PasteOnMiddleClickDefault = true;
        private const bool PasteOnRightClickDefault = false;
        private const string CopyNewlineDefault = "\r\n";

        [Description("The order in which the module should be displayed on the TermBar.")]
        [DefaultIntNumber(OrderDefault)]
        [MinimumInt(int.MinValue)]
        [MaximumInt(int.MaxValue)]
        public int Order { get; set; } = OrderDefault;

        [Description("Whether the module should expand to take up as much space as possible.")]
        [DefaultBoolean(ExpandDefault)]
        public bool Expand { get; set; } = ExpandDefault;

        [Description("The Catppuccin color to use as an accent.")]
        [DefaultString(AccentColorDefaultAsString)]
        public ColorEnum AccentColor { get; set; } = AccentColorDefault;

        [Description("The text to use as the terminal icon.")]
        [DefaultString(IconDefault)]
        public string Icon { get; set; } = IconDefault;

        [Description("The text to use as the terminal visual bell icon when the visual bell is ringing. Has no effect if UseVisualBell is false.")]
        [DefaultString(VisualBellIconDefault)]
        public string VisualBellIcon { get; set; } = VisualBellIconDefault;

        [Description("The command to run in the terminal. This is typically a shell.")]
        [DefaultString(CommandDefault)]
        public string Command { get; set; } = CommandDefault;

        [Description("Whether to restart Command when it exits normally.")]
        [DefaultBoolean(RestartOnExitDefault)]
        public bool RestartOnExit { get; set; } = RestartOnExitDefault;

        [Description("The default terminal window title. The terminal window title can be viewed as a tooltip of the terminal icon.")]
        [DefaultString(DefaultWindowTitleDefault)]
        public string DefaultWindowTitle { get; set; } = DefaultWindowTitleDefault;

        [Description("The number of rows in the terminal.")]
        [DefaultIntNumber(RowsDefault)]
        [MinimumInt(1)]
        public int Rows { get; set; } = RowsDefault;

        [Description("The number of columns in the terminal.")]
        [DefaultIntNumber(ColumnsDefault)]
        [MinimumInt(1)]
        public int Columns { get; set; } = ColumnsDefault;

        [Description("The font family to use in the terminal. Set to null to use the TermBar font family.")]
        [DefaultNull]
        public string? FontFamily { get; set; } = null;

        [Description("The font size to use in the terminal. Set to null to use the TermBar font size.")]
        [DefaultNull]
        public double? FontSize { get; set; } = null;

        [Description("The default tab width in the terminal. (Practically speaking, this is virtually irrelevant today.)")]
        [DefaultIntNumber(TabWidthDefault)]
        [MinimumInt(1)]
        public int? TabWidth { get; set; } = TabWidthDefault;

        [Description("The text antialiasing style to apply in the terminal. Unicode box-drawing characters are always drawn without antialiasing.")]
        [DefaultString(TextAntialiasingDefaultAsString)]
        public TextAntialiasingStyle? TextAntialiasing { get; set; } = TextAntialiasingDefault;

        [Description("Whether to draw emoji in full color in the terminal using the Segoe UI Emoji font.")]
        [DefaultBoolean(FullColorEmojiDefault)]
        public bool? FullColorEmoji { get; set; } = FullColorEmojiDefault;

        [Description("Whether to use background color erase in the terminal. See https://sunaku.github.io/vim-256color-bce.html for a description of what this does.")]
        [DefaultBoolean(UseBackgroundColorEraseDefault)]
        public bool? UseBackgroundColorErase { get; set; } = UseBackgroundColorEraseDefault;

        [Description("Whether to render the default background color as invisible in the terminal.")]
        [DefaultBoolean(BackgroundIsInvisibleDefault)]
        public bool? BackgroundIsInvisible { get; set; } = BackgroundIsInvisibleDefault;

        [Description("Whether to use the visual bell in the terminal. If this is true, VisualBellIcon is displayed for VisualBellDisplayTime milliseconds when the terminal bell rings. If this is false, a sound plays instead.")]
        [DefaultBoolean(UseVisualBellDefault)]
        public bool? UseVisualBell { get; set; } = UseVisualBellDefault;

        [Description("The number of milliseconds to display the visual bell in the terminal. Does nothing if UseVisualBell is false.")]
        [DefaultIntNumber(VisualBellDisplayTimeDefault)]
        [MinimumInt(1)]
        public int VisualBellDisplayTime { get; set; } = VisualBellDisplayTimeDefault;

        [Description("Whether to show the context menu when right clicking the terminal.")]
        [DefaultBoolean(UseContextMenuDefault)]
        public bool? UseContextMenu { get; set; } = UseContextMenuDefault;

        [Description("Whether to show the extended context menu when right clicking the terminal. Does nothing if UseContextMenu is false.")]
        [DefaultBoolean(UseExtendedContextMenuDefault)]
        public bool? UseExtendedContextMenu { get; set; } = UseExtendedContextMenuDefault;

        [Description("The default cursor style to use in the terminal.")]
        [DefaultString(CursorStyleDefaultAsString)]
        public CursorStyle? CursorStyle { get; set; } = CursorStyleDefault;

        [Description("The cursor thickness in the terminal, as a fraction of the font size, between 0.0 and 1.0. Does not affect the block cursor.")]
        [DefaultDoubleNumber(CursorThicknessDefault)]
        [MinimumDouble(0.0)]
        [MaximumDouble(1.0)]
        public double? CursorThickness { get; set; } = CursorThicknessDefault;

        [Description("Whether the cursor should blink by default in the terminal.")]
        [DefaultBoolean(CursorBlinkDefault)]
        public bool? CursorBlink { get; set; } = CursorBlinkDefault;

        [Description("The number of milliseconds that pass between the blinking cursor being displayed in the terminal and hidden. Has no effect if CursorBlink is false.")]
        [DefaultIntNumber(CursorBlinkRateDefault)]
        [MinimumInt(1)]
        public int? CursorBlinkRate { get; set; } = CursorBlinkRateDefault;

        [Description("The cursor color in the terminal.")]
        [DefaultString(CursorColorDefaultAsString)]
        public ColorEnum? CursorColor { get; set; } = CursorColorDefault;

        [Description("The number of scrollback lines in the terminal.")]
        [DefaultIntNumber(ScrollbackLinesDefault)]
        [MinimumInt(0)]
        public int? ScrollbackLines { get; set; } = ScrollbackLinesDefault;

        [Description("The number of scrollback lines per large scrollback in the terminal. A large scrollback is invoked with Shift-Page Up and reversed with Shift-Page Down.")]
        [DefaultIntNumber(LinesPerScrollbackDefault)]
        [MinimumInt(1)]
        public int? LinesPerScrollback { get; set; } = LinesPerScrollbackDefault;

        [Description("The number of scrollback lines per small scrollback in the terminal. A small scrollback is invoked with Shift-Up and reversed with Shift-Down.")]
        [DefaultIntNumber(LinesPerSmallScrollbackDefault)]
        [MinimumInt(1)]
        public int? LinesPerSmallScrollback { get; set; } = LinesPerSmallScrollbackDefault;

        [Description("The number of scrollback lines per mouse wheel scroll in the terminal. A mouse wheel scroll is invoked with mouse wheel up and reversed with mouse wheel down.")]
        [DefaultIntNumber(LinesPerWheelScrollbackDefault)]
        [MinimumInt(1)]
        public int? LinesPerWheelScrollback { get; set; } = LinesPerWheelScrollbackDefault;

        [Description("Whether to copy selected text in the terminal when the left mouse button is released. If this is false, the selection can be copied with Ctrl-Shift-C.")]
        [DefaultBoolean(CopyOnMouseUpDefault)]
        public bool? CopyOnMouseUp { get; set; } = CopyOnMouseUpDefault;

        [Description("Whether to paste text in the terminal when the terminal is middle clicked. Text can also be pasted with Ctrl-Shift-V.")]
        [DefaultBoolean(PasteOnMiddleClickDefault)]
        public bool? PasteOnMiddleClick { get; set; } = PasteOnMiddleClickDefault;

        [Description("Whether to paste text in the terminal when the terminal is right clicked. Text can also be pasted with Ctrl-Shift-V. If this is true, the context menu can be displayed by right clicking the terminal while pressing Ctrl (assuming UseContextMenu is true).")]
        [DefaultBoolean(PasteOnRightClickDefault)]
        public bool? PasteOnRightClick { get; set; } = PasteOnRightClickDefault;

        [Description("The line ending to use when copying text in the terminal.")]
        [DefaultString(CopyNewlineDefault)]
        public string? CopyNewline { get; set; } = CopyNewlineDefault;

        [Description("The ANSI colors to use in the terminal.")]
        public TerminalColors Colors { get; set; } = new();
    }

    [Description("Terminal ANSI colors.")]
    internal class TerminalColors
    {
        [Description("The default ANSI colors to use in the terminal.")]
        public DefaultColors DefaultColors { get; set; } = new();

        [Description("The standard (non-bright) ANSI colors to use in the terminal.")]
        public StandardColors StandardColors { get; set; } = new();

        [Description("The bright ANSI colors to use in the terminal.")]
        public BrightColors BrightColors { get; set; } = new();
    }

    [Description("Terminal default ANSI colors.")]
    internal class DefaultColors
    {
        private const AnsiColorEnum DefaultBackgroundColorDefault = AnsiColorEnum.Black;
        private const string DefaultBackgroundColorDefaultAsString = "Black";
        private const AnsiColorEnum DefaultForegroundColorDefault = AnsiColorEnum.White;
        private const string DefaultForegroundColorDefaultAsString = "White";
        private const AnsiColorEnum DefaultUnderlineColorDefault = AnsiColorEnum.White;
        private const string DefaultUnderlineColorDefaultAsString = "White";

        [Description("The default background color to use in the terminal.")]
        [DefaultString(DefaultBackgroundColorDefaultAsString)]
        public AnsiColorEnum DefaultBackgroundColor { get; set; } = DefaultBackgroundColorDefault;

        [Description("The default foreground color to use in the terminal.")]
        [DefaultString(DefaultForegroundColorDefaultAsString)]
        public AnsiColorEnum DefaultForegroundColor { get; set; } = DefaultForegroundColorDefault;

        [Description("The default underline color to use in the terminal.")]
        [DefaultString(DefaultUnderlineColorDefaultAsString)]
        public AnsiColorEnum DefaultUnderlineColor { get; set; } = DefaultUnderlineColorDefault;
    }

    [Description("Terminal standard (non-bright) ANSI colors.")]
    internal class StandardColors
    {
        private const AnsiColorEnum BlackDefault = AnsiColorEnum.Black;
        private const string BlackDefaultAsString = "Black";
        private const AnsiColorEnum RedDefault = AnsiColorEnum.Red;
        private const string RedDefaultAsString = "Red";
        private const AnsiColorEnum GreenDefault = AnsiColorEnum.Green;
        private const string GreenDefaultAsString = "Green";
        private const AnsiColorEnum YellowDefault = AnsiColorEnum.Yellow;
        private const string YellowDefaultAsString = "Yellow";
        private const AnsiColorEnum BlueDefault = AnsiColorEnum.Blue;
        private const string BlueDefaultAsString = "Blue";
        private const AnsiColorEnum MagentaDefault = AnsiColorEnum.Magenta;
        private const string MagentaDefaultAsString = "Magenta";
        private const AnsiColorEnum CyanDefault = AnsiColorEnum.Cyan;
        private const string CyanDefaultAsString = "Cyan";
        private const AnsiColorEnum WhiteDefault = AnsiColorEnum.White;
        private const string WhiteDefaultAsString = "White";

        [Description("Black.")]
        [DefaultString(BlackDefaultAsString)]
        public AnsiColorEnum Black { get; set; } = BlackDefault;

        [Description("Red.")]
        [DefaultString(RedDefaultAsString)]
        public AnsiColorEnum Red { get; set; } = RedDefault;

        [Description("Green.")]
        [DefaultString(GreenDefaultAsString)]
        public AnsiColorEnum Green { get; set; } = GreenDefault;

        [Description("Yellow.")]
        [DefaultString(YellowDefaultAsString)]
        public AnsiColorEnum Yellow { get; set; } = YellowDefault;

        [Description("Blue.")]
        [DefaultString(BlueDefaultAsString)]
        public AnsiColorEnum Blue { get; set; } = BlueDefault;

        [Description("Magenta.")]
        [DefaultString(MagentaDefaultAsString)]
        public AnsiColorEnum Magenta { get; set; } = MagentaDefault;

        [Description("Cyan.")]
        [DefaultString(CyanDefaultAsString)]
        public AnsiColorEnum Cyan { get; set; } = CyanDefault;

        [Description("White.")]
        [DefaultString(WhiteDefaultAsString)]
        public AnsiColorEnum White { get; set; } = WhiteDefault;
    }

    [Description("Terminal bright ANSI colors.")]
    internal class BrightColors
    {
        private const AnsiColorEnum BrightBlackDefault = AnsiColorEnum.BrightBlack;
        private const string BrightBlackDefaultAsString = "BrightBlack";
        private const AnsiColorEnum BrightRedDefault = AnsiColorEnum.BrightRed;
        private const string BrightRedDefaultAsString = "BrightRed";
        private const AnsiColorEnum BrightGreenDefault = AnsiColorEnum.BrightGreen;
        private const string BrightGreenDefaultAsString = "BrightGreen";
        private const AnsiColorEnum BrightYellowDefault = AnsiColorEnum.BrightYellow;
        private const string BrightYellowDefaultAsString = "BrightYellow";
        private const AnsiColorEnum BrightBlueDefault = AnsiColorEnum.BrightBlue;
        private const string BrightBlueDefaultAsString = "BrightBlue";
        private const AnsiColorEnum BrightMagentaDefault = AnsiColorEnum.BrightMagenta;
        private const string BrightMagentaDefaultAsString = "BrightMagenta";
        private const AnsiColorEnum BrightCyanDefault = AnsiColorEnum.BrightCyan;
        private const string BrightCyanDefaultAsString = "BrightCyan";
        private const AnsiColorEnum BrightWhiteDefault = AnsiColorEnum.BrightWhite;
        private const string BrightWhiteDefaultAsString = "BrightWhite";

        [Description("Bright black.")]
        [DefaultString(BrightBlackDefaultAsString)]
        public AnsiColorEnum BrightBlack { get; set; } = BrightBlackDefault;

        [Description("Bright red.")]
        [DefaultString(BrightRedDefaultAsString)]
        public AnsiColorEnum BrightRed { get; set; } = BrightRedDefault;

        [Description("Bright green.")]
        [DefaultString(BrightGreenDefaultAsString)]
        public AnsiColorEnum BrightGreen { get; set; } = BrightGreenDefault;

        [Description("Bright yellow.")]
        [DefaultString(BrightYellowDefaultAsString)]
        public AnsiColorEnum BrightYellow { get; set; } = BrightYellowDefault;

        [Description("Bright blue.")]
        [DefaultString(BrightBlueDefaultAsString)]
        public AnsiColorEnum BrightBlue { get; set; } = BrightBlueDefault;

        [Description("Bright magenta.")]
        [DefaultString(BrightMagentaDefaultAsString)]
        public AnsiColorEnum BrightMagenta { get; set; } = BrightMagentaDefault;

        [Description("Bright cyan.")]
        [DefaultString(BrightCyanDefaultAsString)]
        public AnsiColorEnum BrightCyan { get; set; } = BrightCyanDefault;

        [Description("Bright white.")]
        [DefaultString(BrightWhiteDefaultAsString)]
        public AnsiColorEnum BrightWhite { get; set; } = BrightWhiteDefault;
    }
}