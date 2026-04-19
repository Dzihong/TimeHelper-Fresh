namespace TimeHelper.Models;

/// <summary>
/// 倒计时方案模型。
/// 用于保存用户常用的倒计时模板。
/// </summary>
public class CountdownPlan
{
    /// <summary>
    /// 方案名称，例如“25分钟专注”。
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 倒计时时长，单位为分钟。
    /// </summary>
    public int Minutes { get; set; }
}
