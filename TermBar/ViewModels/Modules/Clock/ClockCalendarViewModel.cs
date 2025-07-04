using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using TermBar.Catppuccin;

namespace TermBar.ViewModels.Modules.Clock {
  /// <summary>
  /// The clock calendar viewmodel.
  /// </summary>
#pragma warning disable IDE0079 // Remove unnecessary suppression
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CsWinRT1028:Class is not marked partial", Justification = "Trivial viewmodel")]
#pragma warning restore IDE0079 // Remove unnecessary suppression
  internal class ClockCalendarViewModel : ObservableObject {
    private readonly Configuration.Json.TermBar config;
    private readonly Configuration.Json.Modules.Clock moduleConfig;

    private DateTime _date;
    private string? _headerDay;
    private string? _headerDate;
    private string? _lastMonth;
    private string? _nextMonth;

    /// <summary>
    /// The date for which to generate a calendar.
    /// </summary>
    /// <remarks>Only the month and year are relevant.</remarks>
    public DateTime Date {
      get => _date;

      set {
        SetProperty(ref _date, new(value.Year, value.Month, 1));

        HeaderDay = DateTime.Now.Year == _date.Year && DateTime.Now.Month == _date.Month
          ? DateTime.Now.ToString(moduleConfig.Calendar!.TodayDayFormat)
          : string.Empty;

        HeaderDate = DateTime.Now.Year == _date.Year && DateTime.Now.Month == _date.Month
          ? DateTime.Now.ToString(moduleConfig.Calendar!.TodayDateFormat)
          : Date.ToString(moduleConfig.Calendar!.OtherDateFormat);

        LastMonth = Date.AddMonths(-1).ToString(moduleConfig.Calendar!.OtherMonthFormat);
        NextMonth = Date.AddMonths(+1).ToString(moduleConfig.Calendar!.OtherMonthFormat);

        UpdateDays();
      }
    }

    /// <summary>
    /// The header day.
    /// </summary>
    public string? HeaderDay {
      get => _headerDay;
      set => SetProperty(ref _headerDay, value);
    }

    /// <summary>
    /// The header date.
    /// </summary>
    public string? HeaderDate {
      get => _headerDate;
      set => SetProperty(ref _headerDate, value);
    }

    /// <summary>
    /// Last month.
    /// </summary>
    public string? LastMonth {
      get => _lastMonth;
      set => SetProperty(ref _lastMonth, value);
    }

    /// <summary>
    /// Next month.
    /// </summary>
    public string? NextMonth {
      get => _nextMonth;
      set => SetProperty(ref _nextMonth, value);
    }

    /// <summary>
    /// The days of the week to present in the calendar.
    /// </summary>
    public ObservableCollection<UIElement> DaysOfWeek = [];

    /// <summary>
    /// The days to present in the calendar.
    /// </summary>
    internal ObservableCollection<UIElement> Days = [];

    /// <summary>
    /// Initializes a <see cref="ClockCalendarViewModel"/>.
    /// </summary>
    /// <param name="config">A <see cref="Configuration.Json.TermBar"/>
    /// config.</param>
    /// <param name="moduleConfig">A <see
    /// cref="Configuration.Json.Modules.Clock"/> config.</param>
    internal ClockCalendarViewModel(Configuration.Json.TermBar config, Configuration.Json.Modules.Clock moduleConfig) {
      this.config = config;
      this.moduleConfig = moduleConfig;

      UpdateDaysOfWeek();
      Date = DateTime.Now;
    }

    /// <summary>
    /// Goes to the last month.
    /// </summary>
    internal void GoToLastMonth() => Date = Date.AddMonths(-1);

    /// <summary>
    /// Goes to the next month.
    /// </summary>
    internal void GoToNextMonth() => Date = Date.AddMonths(+1);

    /// <summary>
    /// Updates the calendar's <see cref="DaysOfWeek"/>.
    /// </summary>
    private void UpdateDaysOfWeek() {
      DaysOfWeek.Clear();

      for (
        int i = (int) CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
        i < 7 + (int) CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
        i++
      ) {
        DaysOfWeek.Add(new TextBlock() { Text = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[i % 7] });
      }
    }

    /// <summary>
    /// Updates the calendar's <see cref="Days"/>.
    /// </summary>
    private void UpdateDays() {
      Days.Clear();

      for (DayOfWeek i = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek; i < Date.DayOfWeek; i++) {
        Days.Add(new Border() { Width = 0, Height = 0, Visibility = Visibility.Collapsed });
      }

      for (int i = Date.Day; i <= DateTime.DaysInMonth(Date.Year, Date.Month); i++) {
        TextBlock day = new() { Text = i.ToString() };

        if (Date.Year == DateTime.Now.Year && Date.Month == DateTime.Now.Month && i == DateTime.Now.Day)           day.Foreground = PaletteHelper.Palette[config.Flavor].Colors[moduleConfig.AccentColor].SolidColorBrush;

        Days.Add(day);
      }
    }
  }
}
