namespace TimeHelper.Models;

/// <summary>
/// 每日倒计时统计模型。
/// 用于在日历统计页面展示单日汇总结果。
/// </summary>
public class DailyRecordSummary
{
    /// <summary>
    /// 日期。
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// 当天完成次数。
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// 当天累计分钟数。
    /// </summary>
    public int TotalMinutes { get; set; }
}
