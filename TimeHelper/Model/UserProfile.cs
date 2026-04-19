namespace TimeHelper.Models;

/// <summary>
/// 用户资料。
/// </summary>
public class UserProfile
{
    /// <summary>
    /// 用户名。
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 当前生活目标。
    /// </summary>
    public string LifeGoal { get; set; } = string.Empty;

    /// <summary>
    /// 头像路径。
    /// </summary>
    public string AvatarPath { get; set; } = string.Empty;

    /// <summary>
    /// 主题模式。
    /// </summary>
    public string ThemeMode { get; set; } = "PureWhite";

    /// <summary>
    /// 提醒音乐路径。
    /// </summary>
    public string AlarmMusicPath { get; set; } = string.Empty;
}
