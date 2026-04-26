using TimeHelper.Models;
using TimeHelper.Services;

namespace TimeHelper.Views;

/// <summary>
/// Profile page.
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
        ThemePicker.SelectedItem = _profile.ThemeMode;
        VibrationSwitch.IsToggled = _profile.IsVibrationEnabled;
        FlashSwitch.IsToggled = _profile.IsFlashEnabled;

        MusicPathLabel.Text = string.IsNullOrWhiteSpace(_profile.AlarmMusicPath)
            ? "No MP3 alarm selected"
            : $"Selected MP3: {Path.GetFileName(_profile.AlarmMusicPath)}";

        AvatarPathLabel.Text = string.IsNullOrWhiteSpace(_profile.AvatarPath)
            ? "No avatar selected"
            : $"Selected image: {Path.GetFileName(_profile.AvatarPath)}";

        if (!string.IsNullOrWhiteSpace(_profile.AvatarPath) && File.Exists(_profile.AvatarPath))
        {
            AvatarImage.Source = ImageSource.FromFile(_profile.AvatarPath);
        }
        else
        {
            AvatarImage.Source = "dotnet_bot.png";
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

        _profile.IsVibrationEnabled = VibrationSwitch.IsToggled;
        _profile.IsFlashEnabled = FlashSwitch.IsToggled;

        await StorageService.SaveUserProfileAsync(_profile);
        ApplyTheme(_profile.ThemeMode);
        await DisplayAlertAsync("Saved", "Your profile has been updated.", "OK");
    }

    private void OnThemeChanged(object? sender, EventArgs e)
    {
        if (ThemePicker.SelectedItem is string selectedTheme)
        {
            _profile.ThemeMode = selectedTheme;
            ApplyTheme(selectedTheme);
        }
    }

    private void ApplyTheme(string themeMode)
    {
        Application.Current!.UserAppTheme = themeMode == "PureBlack" ? AppTheme.Dark : AppTheme.Light;
    }

    private async void OnChooseAvatarClicked(object? sender, EventArgs e)
    {
        var result = await LocalFileService.PickAvatarAsync();
        if (!result.Success)
        {
            await DisplayAlertAsync("Avatar", result.Message, "OK");
            return;
        }

        _profile.AvatarPath = result.FilePath;
        AvatarImage.Source = ImageSource.FromFile(result.FilePath);
        AvatarPathLabel.Text = $"Selected image: {Path.GetFileName(result.FilePath)}";
    }

    private async void OnChooseMusicClicked(object? sender, EventArgs e)
    {
        var result = await LocalFileService.PickAlarmMusicAsync();
        if (!result.Success)
        {
            await DisplayAlertAsync("Alarm Sound", result.Message, "OK");
            return;
        }

        _profile.AlarmMusicPath = result.FilePath;
        MusicPathLabel.Text = $"Selected MP3: {Path.GetFileName(result.FilePath)}";
        await DisplayAlertAsync("Alarm Sound", "Your MP3 alarm file is ready to use.", "OK");
    }
}
