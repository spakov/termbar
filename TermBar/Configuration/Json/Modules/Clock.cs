using Catppuccin;
using TermBar.Configuration.Json.SchemaAttributes;

namespace TermBar.Configuration.Json.Modules {
  [Description("A TermBar clock configuration.")]
  internal class Clock : IModule {
    private const int orderDefault = int.MaxValue;
    private const bool expandDefault = false;
    private const ColorEnum accentColorDefault = ColorEnum.Sapphire;
    private const string accentColorDefaultAsString = "Sapphire";
    private const string iconDefault = "";
    private const string timeFormatDefault = "h:mm tt";
    private const int updateIntervalDefault = 1000;

    [Description("The order in which the module should be displayed on the TermBar.")]
    [DefaultIntNumber(orderDefault)]
    [MinimumInt(int.MinValue)]
    [MaximumInt(int.MaxValue)]
    public int Order { get; set; } = orderDefault;

    [Description("Whether the module should expand to take up as much space as possible.")]
    [DefaultBoolean(expandDefault)]
    public bool Expand { get; set; } = expandDefault;

    [Description("The Catppuccin color to use for the clock icon.")]
    [DefaultString(accentColorDefaultAsString)]
    public ColorEnum AccentColor { get; set; } = accentColorDefault;

    [Description("The text to use as the clock icon.")]
    [DefaultString(iconDefault)]
    public string Icon { get; set; } = iconDefault;

    [Description("The time format to use for the clock. These are documented in https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings.")]
    [DefaultString(timeFormatDefault)]
    public string TimeFormat { get; set; } = timeFormatDefault;

    [Description("The clock update interval, in milliseconds.")]
    [DefaultIntNumber(updateIntervalDefault)]
    [MinimumInt(1)]
    public int UpdateInterval { get; set; } = updateIntervalDefault;

    [Description("Clock module calendar configuration. Set to null to disable the calendar. The calendar can be displayed by clicking on the clock module.")]
    public Calendar? Calendar { get; set; } = new();
  }

  [Description("The clock module calendar configuration.")]
  internal class Calendar {
    private const string todayDayFormatDefault = "dddd";
    private const string todayDateFormatDefault = "MMMM d, yyyy";
    private const string otherDateFormatDefault = "MMMM yyyy";
    private const string otherMonthFormatDefault = "MMM";

    [Description("The date format to use for today’s day in the calendar. These are documented in https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings.")]
    [DefaultString(todayDayFormatDefault)]
    public string TodayDayFormat { get; set; } = todayDayFormatDefault;

    [Description("The date format to use for today’s date in the calendar. These are documented in https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings.")]
    [DefaultString(todayDateFormatDefault)]
    public string TodayDateFormat { get; set; } = todayDateFormatDefault;

    [Description("The date format to use for months other than the current month in the calendar. These are documented in https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings.")]
    [DefaultString(otherDateFormatDefault)]
    public string OtherDateFormat { get; set; } = otherDateFormatDefault;

    [Description("The date format to use for months other than the current month in the last month and next month buttons in the calendar. These are documented in https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings.")]
    [DefaultString(otherMonthFormatDefault)]
    public string OtherMonthFormat { get; set; } = otherMonthFormatDefault;
  }
}
