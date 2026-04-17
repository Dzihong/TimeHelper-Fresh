namespace TimeHelper.Models;

/// <summary>
/// 用户信息模型
/// 用于保存用户基本资料、主题风格和提醒音乐路径
/// </summary>
public class UserProfile
{
    /// <summary>
    /// 用户名称
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 用户当前生活目标
    /// </summary>
    public string LifeGoal { get; set; } = string.Empty;

    /// <summary>
    /// 用户头像路径
    /// 当前版本先保留字段，后续再真正实现图片选择
    /// </summary>
    public string AvatarPath { get; set; } = string.Empty;

    /// <summary>
    /// 主题模式
    /// 可选值：PureBlack / PureWhite
    /// </summary>
    public string ThemeMode { get; set; } = "PureWhite";

    /// <summary>
    /// 倒计时结束提醒音乐路径
    /// 当前版本先保留字段，后续再真正实现音频选择
    /// </summary>
    public string AlarmMusicPath { get; set; } = string.Empty;
}