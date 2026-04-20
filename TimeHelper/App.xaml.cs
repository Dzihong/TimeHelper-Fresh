using TimeHelper.Services;

namespace TimeHelper;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        ApplySavedThemeAsync();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }

    private async void ApplySavedThemeAsync()
    {
        var profile = await StorageService.LoadUserProfileAsync();
        Current!.UserAppTheme = profile.ThemeMode == "PureBlack" ? AppTheme.Dark : AppTheme.Light;
    }
}
