using TimeHelper.Models;
using TimeHelper.Services;

namespace TimeHelper.Views;

/// <summary>
/// 用户设置页面。
/// </summary>
public partial class ProfilePage : ContentPage
{
    private UserProfile _profile = new();

    public ProfilePage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _profile = await StorageService.LoadUserProfileAsync();
        LoadProfileToView();
    }

    private void LoadProfileToView()
    {
        UserNameEntry.Text = _profile.UserName;
        LifeGoalEditor.Text = _profile.LifeGoal;
        ThemePicker.SelectedIndex = _profile.ThemeMode == "PureBlack" ? 0 : 1;

        if (string.IsNullOrWhiteSpace(_profile.AlarmMusicPath))
        {
            MusicPathLabel.Text = "当前未选择本地音乐";
        }
        else
        {
            MusicPathLabel.Text = $"当前音乐：{_profile.AlarmMusicPath}";
        }

        if (!string.IsNullOrWhiteSpace(_profile.AvatarPath))
        {
            AvatarImage.Source = _profile.AvatarPath;
        }

        ApplyTheme(_profile.ThemeMode);
    }

    private async void OnSaveProfileClicked(object? sender, EventArgs e)
    {
        _profile.UserName = UserNameEntry.Text?.Trim() ?? string.Empty;
        _profile.LifeGoal = LifeGoalEditor.Text?.Trim() ?? string.Empty;

        if (ThemePicker.SelectedItem is string selectedTheme)
        {
            _profile.ThemeMode = selectedTheme;
        }

        await StorageService.SaveUserProfileAsync(_profile);
        await DisplayAlertAsync("保存成功", "用户设置已保存到本地。", "确定");
    }

    private void OnThemeChanged(object? sender, EventArgs e)
    {
        if (ThemePicker.SelectedItem is string selectedTheme)
        {
            ApplyTheme(selectedTheme);
        }
    }

    private void ApplyTheme(string themeMode)
    {
        if (themeMode == "PureBlack")
        {
            BackgroundColor = Colors.Black;

            UserNameEntry.TextColor = Colors.White;
            UserNameEntry.BackgroundColor = Color.FromArgb("#222222");

            LifeGoalEditor.TextColor = Colors.White;
            LifeGoalEditor.BackgroundColor = Color.FromArgb("#222222");

            MusicPathLabel.TextColor = Colors.White;
        }
        else
        {
            BackgroundColor = Colors.White;

            UserNameEntry.TextColor = Colors.Black;
            UserNameEntry.BackgroundColor = Colors.White;

            LifeGoalEditor.TextColor = Colors.Black;
            LifeGoalEditor.BackgroundColor = Colors.White;

            MusicPathLabel.TextColor = Colors.Black;
        }
    }

    private async void OnChooseAvatarClicked(object? sender, EventArgs e)
    {
        await DisplayAlertAsync("提示", "头像选择功能暂未开放。", "确定");
    }

    private async void OnChooseMusicClicked(object? sender, EventArgs e)
    {
        await DisplayAlertAsync("提示", "本地音乐导入功能暂未开放。", "确定");
    }
}
