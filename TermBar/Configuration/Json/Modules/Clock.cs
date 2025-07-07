using System.ComponentModel;
using TermBar.Catppuccin;

namespace TermBar.Configuration.Json.Modules {
  [Description("A TermBar clock configuration.")]
  internal class Clock : IModule {
    [Description("The order in which the module should be displayed on the TermBar.")]
    public int Order { get; set; } = int.MaxValue;

    [Description("Whether the module should expand to take up as much space as possible.")]
    public bool Expand { get; set; } = false;

    [Description("The Catppuccin color to use for the clock icon.")]
    public ColorEnum AccentColor { get; set; } = ColorEnum.Sapphire;

    [Description("The text to use as the clock icon.")]
    public string Icon { get; set; } = "";

    [Description("The time format to use for the clock. These are documented in https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings.")]
    public string TimeFormat { get; set; } = "h:mm tt";

    [Description("The clock update interval, in milliseconds.")]
    public uint UpdateInterval { get; set; } = 1000;

    [Description("Clock module calendar configuration. Set to null to disable the calendar. The calendar can be displayed by clicking on the clock module.")]
    public Calendar? Calendar { get; set; } = new();
  }

  [Description("The clock module calendar configuration.")]
  internal class Calendar {
    [Description("The date format to use for today's day in the calendar. These are documented in https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings.")]
    public string TodayDayFormat { get; set; } = "dddd";

    [Description("The date format to use for today's date in the calendar. These are documented in https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings.")]
    public string TodayDateFormat { get; set; } = "MMMM d, yyyy";

    [Description("The date format to use for months other than the current month in the calendar. These are documented in https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings.")]
    public string OtherDateFormat { get; set; } = "MMMM yyyy";

    [Description("The date format to use for months other than the current month in the last month and next month buttons in the calendar. These are documented in https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings.")]
    public string OtherMonthFormat { get; set; } = "MMM";
  }
}
