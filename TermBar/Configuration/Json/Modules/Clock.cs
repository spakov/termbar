using TermBar.Catppuccin;

namespace TermBar.Configuration.Json.Modules {
  /// <summary>
  /// A TermBar clock configuration.
  /// </summary>
  internal class Clock : IModule {
    public int Order { get; set; } = int.MaxValue;

    public bool Expand { get; set; } = false;

    /// <summary>
    /// The Catppuccin color to use for the clock icon.
    /// </summary>
    public ColorEnum AccentColor { get; set; } = ColorEnum.Sapphire;

    /// <summary>
    /// The text to use as the clock icon.
    /// </summary>
    public string Icon { get; set; } = "";

    /// <summary>
    /// The time format to use, as in
    /// https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings.
    /// </summary>
    public string TimeFormat { get; set; } = "h:mm tt";

    /// <summary>
    /// The clock update interval, in milliseconds.
    /// </summary>
    public uint UpdateInterval { get; set; } = 1000;

    /// <summary>
    /// Calendar configuration.
    /// </summary>
    /// <remarks>
    /// <para>The calendar is displayed by clicking on the clock.</para>
    /// <para>Set to <c>null</c> to disable the calendar.</para>
    /// </remarks>
    public Calendar? Calendar { get; set; } = new();
  }

  /// <summary>
  /// A TermBar clock calendar configuration.
  /// </summary>
  internal class Calendar {
    /// <summary>
    /// The date format to use, as in
    /// https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings,
    /// for today's day.
    /// </summary>
    public string TodayDayFormat { get; set; } = "dddd";

    /// <summary>
    /// The date format to use, as in
    /// https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings,
    /// for today's date.
    /// </summary>
    public string TodayDateFormat { get; set; } = "MMMM d, yyyy";

    /// <summary>
    /// The date format to use, as in
    /// https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings,
    /// for the header for months other than today's month.
    /// </summary>
    public string OtherDateFormat { get; set; } = "MMMM yyyy";

    /// <summary>
    /// The date format to use, as in
    /// https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings,
    /// for months other than today's month in last month and next month
    /// buttons.
    /// </summary>
    public string OtherMonthFormat { get; set; } = "MMM";
  }
}
