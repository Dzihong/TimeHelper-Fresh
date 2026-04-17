namespace TimeHelper.Models;

/// <summary>
/// 倒计时方案模型
/// 用来保存用户设置好的常用倒计时模板
/// </summary>
public class CountdownPlan
{
    /// <summary>
    /// 方案名称
    /// 例如：25分钟专注、10分钟休息
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 倒计时时长（单位：分钟）
    /// </summary>
    public int Minutes { get; set; }
}