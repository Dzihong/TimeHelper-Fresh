namespace TimeHelper.Models;

/// <summary>
/// 倒计时完成记录模型。
/// 用于记录每次完成倒计时的时间和时长。
/// </summary>
public class CountdownRecord
{
    /// <summary>
    /// 完成倒计时的时间。
    /// </summary>
    public DateTime CompletedAt { get; set; }

    /// <summary>
    /// 本次倒计时的分钟数。
    /// </summary>
    public int Minutes { get; set; }

    /// <summary>
    /// 对应的方案名称；若为手动输入，则可以为空。
    /// </summary>
    public string PlanName { get; set; } = string.Empty;
}
