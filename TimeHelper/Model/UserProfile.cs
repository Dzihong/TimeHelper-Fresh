namespace TimeHelper.Models;

/// <summary>
/// User profile.
/// </summary>
public class UserProfile
{
    /// <summary>
    /// User name.
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Life goal.
    /// </summary>
    public string LifeGoal { get; set; } = string.Empty;

    /// <summary>
    /// Avatar path.
    /// </summary>
    public string AvatarPath { get; set; } = string.Empty;

    /// <summary>
    /// Theme mode.
    /// </summary>
    public string ThemeMode { get; set; } = "PureWhite";

    /// <summary>
    /// Alarm path.
    /// </summary>
    public string AlarmMusicPath { get; set; } = string.Empty;
}
