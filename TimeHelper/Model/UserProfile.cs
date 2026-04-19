namespace TimeHelper.Models;

/// <summary>
/// 用户资料模型。
/// 用于保存基础信息、主题模式和提醒音乐路径。
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
    /// 用户头像路径。
    /// 当前版本暂仅保存路径。
    /// </summary>
    public string AvatarPath { get; set; } = string.Empty;

    /// <summary>
    /// 主题模式，可选值为 PureBlack 或 PureWhite。
    /// </summary>
    public string ThemeMode { get; set; } = "PureWhite";

    /// <summary>
    /// 倒计时结束提醒音乐路径。
    /// 当前版本暂仅保存路径。
    /// </summary>
    public string AlarmMusicPath { get; set; } = string.Empty;
}
