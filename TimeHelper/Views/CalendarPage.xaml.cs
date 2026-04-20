using Microsoft.Maui.Controls.Shapes;
using TimeHelper.Models;
using TimeHelper.Services;

namespace TimeHelper.Views;

/// <summary>
/// 日历统计页。
/// </summary>
public partial class CalendarPage : ContentPage
{
    private List<CountdownRecord> _records = new();
    private DateTime _displayMonth = new(DateTime.Now.Year, DateTime.Now.Month, 1);

    public CalendarPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _records = await StorageService.LoadRecordsAsync();
        RefreshCalendar();
    }

    private void RefreshCalendar()
    {
        LoadCurrentMonthSummary();
        BuildCalendar();
        SelectedDayLabel.Text = "Tap a day";
        SelectedDaySummaryLabel.Text = "Choose a date in the calendar to inspect its countdown usage.";
    }

    private void LoadCurrentMonthSummary()
    {
        var monthRecords = _records
            .Where(r => r.CompletedAt.Year == _displayMonth.Year && r.CompletedAt.Month == _displayMonth.Month)
            .ToList();

        MonthLabel.Text = _displayMonth.ToString("MMMM yyyy");
        TotalCountLabel.Text = $"{monthRecords.Count} sessions";
        TotalMinutesLabel.Text = $"{monthRecords.Sum(r => r.Minutes)} min";
    }

    private void BuildCalendar()
    {
        CalendarGrid.Children.Clear();
        CalendarGrid.RowDefinitions.Clear();

        DateTime firstDay = new(_displayMonth.Year, _displayMonth.Month, 1);
        int daysInMonth = DateTime.DaysInMonth(_displayMonth.Year, _displayMonth.Month);
        int startColumn = (int)firstDay.DayOfWeek;
        int totalCells = startColumn + daysInMonth;
        int totalRows = (int)Math.Ceiling(totalCells / 7d);

        for (int row = 0; row < totalRows; row++)
        {
            CalendarGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        }

        for (int day = 1; day <= daysInMonth; day++)
        {
            DateTime date = new(_displayMonth.Year, _displayMonth.Month, day);
            int index = startColumn + day - 1;
            int row = index / 7;
            int column = index % 7;

            var dayRecords = _records
                .Where(r => r.CompletedAt.Date == date.Date)
                .ToList();

            Border dayCard = CreateDayCard(date, dayRecords);
            Grid.SetRow(dayCard, row);
            Grid.SetColumn(dayCard, column);
            CalendarGrid.Children.Add(dayCard);
        }
    }

    private Border CreateDayCard(DateTime date, List<CountdownRecord> dayRecords)
    {
        Border border = new()
        {
            Padding = 10,
            StrokeShape = new RoundRectangle { CornerRadius = 18 },
            BackgroundColor = GetDayColor(date, dayRecords.Count > 0)
        };

        VerticalStackLayout layout = new()
        {
            Spacing = 4
        };

        layout.Children.Add(new Label
        {
            Text = date.Day.ToString(),
            FontFamily = "OpenSansSemibold",
            FontSize = 18,
            HorizontalTextAlignment = TextAlignment.Center,
            TextColor = GetThemeColor("TextLight", "TextDark")
        });

        layout.Children.Add(new Label
        {
            Text = dayRecords.Count == 0 ? "No use" : $"{dayRecords.Count} session(s)",
            FontSize = 11,
            HorizontalTextAlignment = TextAlignment.Center,
            TextColor = GetThemeColor("MutedLight", "MutedDark")
        });

        border.Content = layout;

        TapGestureRecognizer tapGesture = new();
        tapGesture.Tapped += async (_, _) => await OnDayTappedAsync(date, dayRecords);
        border.GestureRecognizers.Add(tapGesture);

        return border;
    }

    private Color GetDayColor(DateTime date, bool hasRecords)
    {
        if (date.Date == DateTime.Today)
        {
            return GetThemeColor("CalendarTodayLight", "CalendarTodayDark");
        }

        if (hasRecords)
        {
            return GetThemeColor("CalendarBusyLight", "CalendarBusyDark");
        }

        return GetThemeColor("CalendarIdleLight", "CalendarIdleDark");
    }

    private Color GetThemeColor(string lightKey, string darkKey)
    {
        bool isDark = Application.Current?.UserAppTheme == AppTheme.Dark;
        string key = isDark ? darkKey : lightKey;
        return (Color)Application.Current!.Resources[key];
    }

    private async Task OnDayTappedAsync(DateTime date, List<CountdownRecord> dayRecords)
    {
        SelectedDayLabel.Text = date.ToString("dddd, MMM dd");

        if (dayRecords.Count == 0)
        {
            SelectedDaySummaryLabel.Text = "No countdown activity was recorded on this day.";
            await DisplayAlertAsync("Day Details", $"{date:dddd, MMM dd}\n\nNo countdown sessions were recorded.", "OK");
            return;
        }

        int totalMinutes = dayRecords.Sum(r => r.Minutes);
        string details = string.Join(
            Environment.NewLine,
            dayRecords.Select(r =>
                $"- {(string.IsNullOrWhiteSpace(r.PlanName) ? "Quick timer" : r.PlanName)} | {r.Minutes} min | {r.CompletedAt:HH:mm}"));

        SelectedDaySummaryLabel.Text = $"{dayRecords.Count} session(s), {totalMinutes} min total";

        await DisplayAlertAsync(
            "Day Details",
            $"{date:dddd, MMM dd}\n\nSessions: {dayRecords.Count}\nTotal Minutes: {totalMinutes}\n\n{details}",
            "OK");
    }

    private void OnPreviousMonthClicked(object? sender, EventArgs e)
    {
        _displayMonth = _displayMonth.AddMonths(-1);
        RefreshCalendar();
    }

    private void OnNextMonthClicked(object? sender, EventArgs e)
    {
        _displayMonth = _displayMonth.AddMonths(1);
        RefreshCalendar();
    }

    private async void OnExportPlansClicked(object? sender, EventArgs e)
    {
        List<CountdownPlan> plans = await StorageService.LoadPlansAsync();
        if (plans.Count == 0)
        {
            await DisplayAlertAsync("Nothing to Export", "There are no saved plans yet.", "OK");
            return;
        }

        var result = await PlanFileService.ExportPlansAsync(plans);
        await DisplayAlertAsync(result.Success ? "Export Complete" : "Export Failed", result.Message, "OK");
    }

    private async void OnImportPlansClicked(object? sender, EventArgs e)
    {
        var result = await PlanFileService.ImportPlansAsync();
        if (!result.Success)
        {
            await DisplayAlertAsync("Import Result", result.Message, "OK");
            return;
        }

        List<CountdownPlan> existingPlans = await StorageService.LoadPlansAsync();
        int importedCount = 0;
        int skippedCount = 0;

        foreach (CountdownPlan plan in result.Plans)
        {
            bool exists = existingPlans.Any(item =>
                item.Name.Equals(plan.Name, StringComparison.OrdinalIgnoreCase));

            if (exists)
            {
                skippedCount++;
                continue;
            }

            existingPlans.Add(plan);
            importedCount++;
        }

        await StorageService.SavePlansAsync(existingPlans);
        await DisplayAlertAsync(
            "Import Complete",
            $"Imported {importedCount} plan(s) and skipped {skippedCount} duplicate plan(s).",
            "OK");
    }
}
