using Spakov.Catppuccin;
using Spakov.TermBar.Configuration.Json.SchemaAttributes;

namespace Spakov.TermBar.Configuration.Json.Modules
{
    [Description("A TermBar clock configuration.")]
    internal class Clock : IModule
    {
        private const int OrderDefault = int.MaxValue;
        private const bool ExpandDefault = false;
        private const ColorEnum AccentColorDefault = ColorEnum.Sapphire;
        private const string AccentColorDefaultAsString = "Sapphire";
        private const string IconDefault = "";
        private const string TimeFormatDefault = "h:mm tt";
        private const int UpdateIntervalDefault = 1000;

        [Description("The order in which the module should be displayed on the TermBar.")]
        [DefaultIntNumber(OrderDefault)]
        [MinimumInt(int.MinValue)]
        [MaximumInt(int.MaxValue)]
        public int Order { get; set; } = OrderDefault;

        [Description("Whether the module should expand to take up as much space as possible.")]
        [DefaultBoolean(ExpandDefault)]
        public bool Expand { get; set; } = ExpandDefault;

        [Description("The Catppuccin color to use for the clock icon.")]
        [DefaultString(AccentColorDefaultAsString)]
        public ColorEnum AccentColor { get; set; } = AccentColorDefault;

        [Description("The text to use as the clock icon.")]
        [DefaultString(IconDefault)]
        public string Icon { get; set; } = IconDefault;

        [Description("The time format to use for the clock. These are documented in https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings.")]
        [DefaultString(TimeFormatDefault)]
        public string TimeFormat { get; set; } = TimeFormatDefault;

        [Description("The clock update interval, in milliseconds.")]
        [DefaultIntNumber(UpdateIntervalDefault)]
        [MinimumInt(1)]
        public int UpdateInterval { get; set; } = UpdateIntervalDefault;

        [Description("Clock module calendar configuration. Set to null to disable the calendar. The calendar can be displayed by clicking on the clock module.")]
        public Calendar? Calendar { get; set; } = new();
    }

    [Description("The clock module calendar configuration.")]
    internal class Calendar
    {
        private const string TodayDayFormatDefault = "dddd";
        private const string TodayDateFormatDefault = "MMMM d, yyyy";
        private const string OtherDateFormatDefault = "MMMM yyyy";
        private const string OtherMonthFormatDefault = "MMM";

        [Description("The date format to use for today’s day in the calendar. These are documented in https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings.")]
        [DefaultString(TodayDayFormatDefault)]
        public string TodayDayFormat { get; set; } = TodayDayFormatDefault;

        [Description("The date format to use for today’s date in the calendar. These are documented in https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings.")]
        [DefaultString(TodayDateFormatDefault)]
        public string TodayDateFormat { get; set; } = TodayDateFormatDefault;

        [Description("The date format to use for months other than the current month in the calendar. These are documented in https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings.")]
        [DefaultString(OtherDateFormatDefault)]
        public string OtherDateFormat { get; set; } = OtherDateFormatDefault;

        [Description("The date format to use for months other than the current month in the last month and next month buttons in the calendar. These are documented in https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings.")]
        [DefaultString(OtherMonthFormatDefault)]
        public string OtherMonthFormat { get; set; } = OtherMonthFormatDefault;
    }
}