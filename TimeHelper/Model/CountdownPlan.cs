namespace TimeHelper.Models;

/// <summary>
/// 倒计时方案。
/// </summary>
public class CountdownPlan
{
    /// <summary>
    /// 方案名称。
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 倒计时分钟数。
    /// </summary>
    public int Minutes { get; set; }
}
