namespace TimeHelper.Models;

/// <summary>
/// 每日统计摘要。
/// </summary>
public class DailyRecordSummary
{
    /// <summary>
    /// 日期。
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// 完成次数。
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// 累计分钟数。
    /// </summary>
    public int TotalMinutes { get; set; }
}
