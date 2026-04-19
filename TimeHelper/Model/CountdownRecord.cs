namespace TimeHelper.Models;

/// <summary>
/// 倒计时完成记录。
/// </summary>
public class CountdownRecord
{
    /// <summary>
    /// 完成时间。
    /// </summary>
    public DateTime CompletedAt { get; set; }

    /// <summary>
    /// 完成分钟数。
    /// </summary>
    public int Minutes { get; set; }

    /// <summary>
    /// 对应方案名。
    /// </summary>
    public string PlanName { get; set; } = string.Empty;
}
