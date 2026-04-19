using TimeHelper.Models;
using TimeHelper.Services;

namespace TimeHelper.Views;

/// <summary>
/// 日历统计页面。
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
            .Select(group => new DailyRecordSummary
            {
                Date = group.Key,
                Count = group.Count(),
                TotalMinutes = group.Sum(item => item.Minutes)
            })
            .OrderByDescending(item => item.Date)
            .ToList();

        DailySummaryCollectionView.ItemsSource = null;
        DailySummaryCollectionView.ItemsSource = _dailySummaries;
    }

    private async void OnExportPlansClicked(object? sender, EventArgs e)
    {
        List<CountdownPlan> plans = await StorageService.LoadPlansAsync();
        if (plans.Count == 0)
        {
            await DisplayAlertAsync("提示", "当前没有可导出的方案。", "确定");
            return;
        }

        var result = await PlanFileService.ExportPlansAsync(plans);
        await DisplayAlertAsync(result.Success ? "导出成功" : "导出失败", result.Message, "确定");
    }

    private async void OnImportPlansClicked(object? sender, EventArgs e)
    {
        var result = await PlanFileService.ImportPlansAsync();
        if (!result.Success)
        {
            await DisplayAlertAsync("导入结果", result.Message, "确定");
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
            "导入完成",
            $"成功导入 {importedCount} 个方案，跳过 {skippedCount} 个重复方案。",
            "确定");
    }
}
