namespace TimeHelper.Models;

/// <summary>
/// 每日倒计时统计模型
/// 用于在日历/统计页面中显示某一天的倒计时使用情况
/// </summary>
public class DailyRecordSummary
{
    /// <summary>
    /// 日期
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// 当天完成次数
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// 当天累计分钟数
    /// </summary>
    public int TotalMinutes { get; set; }
}