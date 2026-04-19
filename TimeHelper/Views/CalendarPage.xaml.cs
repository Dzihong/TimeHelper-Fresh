using TimeHelper.Models;
using TimeHelper.Services;

namespace TimeHelper.Views;

/// <summary>
/// 日历统计页面。
/// 用于展示本月倒计时完成情况，以及按日期分组的统计结果。
/// </summary>
public partial class CalendarPage : ContentPage
{
    private List<CountdownRecord> _records = new();
    private List<DailyRecordSummary> _dailySummaries = new();

    public CalendarPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _records = await StorageService.LoadRecordsAsync();
        LoadCurrentMonthSummary();
    }

    private void LoadCurrentMonthSummary()
    {
        DateTime now = DateTime.Now;

        var currentMonthRecords = _records
            .Where(r => r.CompletedAt.Year == now.Year && r.CompletedAt.Month == now.Month)
            .ToList();

        int totalCount = currentMonthRecords.Count;
        int totalMinutes = currentMonthRecords.Sum(r => r.Minutes);

        MonthLabel.Text = $"月份：{now:yyyy年M月}";
        TotalCountLabel.Text = $"本月完成次数：{totalCount}";
        TotalMinutesLabel.Text = $"本月累计分钟数：{totalMinutes}";

        _dailySummaries = currentMonthRecords
            .GroupBy(r => r.CompletedAt.Date)
            .Select(g => new DailyRecordSummary
            {
                Date = g.Key,
                Count = g.Count(),
                TotalMinutes = g.Sum(x => x.Minutes)
            })
            .OrderByDescending(x => x.Date)
            .ToList();

        DailySummaryCollectionView.ItemsSource = null;
        DailySummaryCollectionView.ItemsSource = _dailySummaries;
    }

    private async void OnExportPlansClicked(object? sender, EventArgs e)
    {
        await DisplayAlertAsync("提示", "导出方案功能将在下一步实现。", "确定");
    }

    private async void OnImportPlansClicked(object? sender, EventArgs e)
    {
        await DisplayAlertAsync("提示", "导入方案功能将在下一步实现。", "确定");
    }
}
