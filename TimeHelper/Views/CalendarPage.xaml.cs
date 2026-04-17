using TimeHelper.Models;
using TimeHelper.Services;

namespace TimeHelper.Views;

/// <summary>
/// 日历统计页面
/// 用于显示本月倒计时完成情况，以及按日期分组的统计结果
/// </summary>
public partial class CalendarPage : ContentPage
{
    // 所有本地倒计时记录
    private List<CountdownRecord> _records = new();

    // 每日汇总结果
    private List<DailyRecordSummary> _dailySummaries = new();

    public CalendarPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 页面显示时自动加载本地记录并刷新界面
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // 读取本地倒计时记录
        _records = await StorageService.LoadRecordsAsync();

        // 刷新统计界面
        LoadCurrentMonthSummary();
    }

    /// <summary>
    /// 统计当前月份的数据，并显示到页面
    /// </summary>
    private void LoadCurrentMonthSummary()
    {
        DateTime now = DateTime.Now;

        // 仅筛选当前月份的数据
        var currentMonthRecords = _records
            .Where(r => r.CompletedAt.Year == now.Year && r.CompletedAt.Month == now.Month)
            .ToList();

        // 统计本月总完成次数
        int totalCount = currentMonthRecords.Count;

        // 统计本月总分钟数
        int totalMinutes = currentMonthRecords.Sum(r => r.Minutes);

        // 更新顶部概览
        MonthLabel.Text = $"月份：{now:yyyy年MM月}";
        TotalCountLabel.Text = $"本月完成次数：{totalCount}";
        TotalMinutesLabel.Text = $"本月累计分钟数：{totalMinutes}";

        // 按日期分组统计
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

        // 刷新列表
        DailySummaryCollectionView.ItemsSource = null;
        DailySummaryCollectionView.ItemsSource = _dailySummaries;
    }

    /// <summary>
    /// 导出方案按钮点击事件
    /// 当前先做占位，后续再实现文件导出功能
    /// </summary>
    private async void OnExportPlansClicked(object sender, EventArgs e)
    {
        await DisplayAlert("提示", "导出方案功能将在下一步实现。", "确定");
    }

    /// <summary>
    /// 导入方案按钮点击事件
    /// 当前先做占位，后续再实现文件导入功能
    /// </summary>
    private async void OnImportPlansClicked(object sender, EventArgs e)
    {
        await DisplayAlert("提示", "导入方案功能将在下一步实现。", "确定");
    }
}