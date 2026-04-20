using TimeHelper.Models;
using TimeHelper.Services;

namespace TimeHelper.Views;

/// <summary>
/// 倒计时主页。
/// </summary>
public partial class CountdownPage : ContentPage
{
    private TimeSpan _remainingTime = TimeSpan.Zero;
    private TimeSpan _initialTime = TimeSpan.Zero;
    private bool _isRunning;
    private bool _isPaused;
    private IDispatcherTimer? _timer;
    private List<CountdownPlan> _plans = new();
    private List<CountdownRecord> _records = new();
    private UserProfile _profile = new();
    private string _selectedPlanName = string.Empty;

    public CountdownPage()
    {
        InitializeComponent();

        InitializeTimer();
        UpdateTimerDisplay();
        UpdateStatus("Ready");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _plans = await StorageService.LoadPlansAsync();
        _records = await StorageService.LoadRecordsAsync();
        _profile = await StorageService.LoadUserProfileAsync();

        RefreshPlansView();
        UpdateAlarmLabel();
    }

    private void InitializeTimer()
    {
        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += OnTimerTick;
    }

    private async void OnTimerTick(object? sender, EventArgs e)
    {
        if (_remainingTime > TimeSpan.Zero)
        {
            _remainingTime = _remainingTime.Subtract(TimeSpan.FromSeconds(1));
            UpdateTimerDisplay();
        }

        if (_remainingTime <= TimeSpan.Zero)
        {
            StopTimer();
            _remainingTime = TimeSpan.Zero;
            UpdateTimerDisplay();
            UpdateStatus("Completed");

            await SaveCompletionRecordAsync();
            await DeviceService.TryVibrateAsync();
            await DeviceService.TryPlayAlarmAsync(_profile.AlarmMusicPath);
            await DisplayAlertAsync("Timer Complete", "Your countdown has finished.", "OK");
        }
    }

    private async void OnStartClicked(object? sender, EventArgs e)
    {
        await DeviceService.StopAlarmAsync();

        if (_isRunning)
        {
            return;
        }

        if (_isPaused && _remainingTime > TimeSpan.Zero)
        {
            _timer?.Start();
            _isRunning = true;
            _isPaused = false;
            UpdateStatus("Running");
            return;
        }

        bool isValid = int.TryParse(MinutesEntry.Text, out int minutes);
        if (!isValid || minutes <= 0)
        {
            await DisplayAlertAsync("Invalid Input", "Please enter a whole number greater than 0.", "OK");
            return;
        }

        _initialTime = TimeSpan.FromMinutes(minutes);
        _remainingTime = _initialTime;

        if (_selectedPlanName == string.Empty || _initialTime.TotalMinutes != minutes)
        {
            _selectedPlanName = string.Empty;
        }

        UpdateTimerDisplay();

        _timer?.Start();
        _isRunning = true;
        _isPaused = false;
        UpdateStatus("Running");
    }

    private void OnPauseClicked(object? sender, EventArgs e)
    {
        if (!_isRunning)
        {
            return;
        }

        _timer?.Stop();
        _isRunning = false;
        _isPaused = true;
        UpdateStatus("Paused");
    }

    private async void OnResetClicked(object? sender, EventArgs e)
    {
        StopTimer();
        await DeviceService.StopAlarmAsync();
        _remainingTime = _initialTime;
        UpdateTimerDisplay();
        UpdateStatus("Ready");
    }

    private async void OnSavePlanClicked(object? sender, EventArgs e)
    {
        bool isValidMinutes = int.TryParse(MinutesEntry.Text, out int minutes);
        if (!isValidMinutes || minutes <= 0)
        {
            await DisplayAlertAsync("Cannot Save", "Enter a valid countdown length first.", "OK");
            return;
        }

        string planName = PlanNameEntry.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(planName))
        {
            await DisplayAlertAsync("Cannot Save", "Please enter a plan name.", "OK");
            return;
        }

        bool alreadyExists = _plans.Any(p => p.Name.Equals(planName, StringComparison.OrdinalIgnoreCase));
        if (alreadyExists)
        {
            await DisplayAlertAsync("Cannot Save", "A plan with this name already exists.", "OK");
            return;
        }

        _plans.Add(new CountdownPlan
        {
            Name = planName,
            Minutes = minutes
        });

        await StorageService.SavePlansAsync(_plans);

        PlanNameEntry.Text = string.Empty;
        RefreshPlansView();

        await DisplayAlertAsync("Saved", "The plan was saved locally.", "OK");
    }

    private void OnUsePlanClicked(object? sender, EventArgs e)
    {
        if (sender is not Button button || button.CommandParameter is not CountdownPlan plan)
        {
            return;
        }

        StopTimer();
        MinutesEntry.Text = plan.Minutes.ToString();
        _initialTime = TimeSpan.FromMinutes(plan.Minutes);
        _remainingTime = _initialTime;
        _selectedPlanName = plan.Name;

        UpdateTimerDisplay();
        UpdateStatus($"Plan loaded: {plan.Name}");
    }

    private async Task SaveCompletionRecordAsync()
    {
        int completedMinutes = (int)_initialTime.TotalMinutes;
        if (completedMinutes <= 0)
        {
            return;
        }

        _records.Add(new CountdownRecord
        {
            CompletedAt = DateTime.Now,
            Minutes = completedMinutes,
            PlanName = _selectedPlanName
        });

        await StorageService.SaveRecordsAsync(_records);
    }

    private void StopTimer()
    {
        _timer?.Stop();
        _isRunning = false;
        _isPaused = false;
    }

    private void UpdateTimerDisplay()
    {
        int totalMinutes = (int)_remainingTime.TotalMinutes;
        int seconds = _remainingTime.Seconds;

        TimerLabel.Text = $"{totalMinutes:D2}:{seconds:D2}";
    }

    private void UpdateStatus(string status)
    {
        StatusLabel.Text = $"Status: {status}";
    }

    private void UpdateAlarmLabel()
    {
        AlarmPathLabel.Text = string.IsNullOrWhiteSpace(_profile.AlarmMusicPath)
            ? "Alarm: Default vibration only"
            : $"Alarm: {Path.GetFileName(_profile.AlarmMusicPath)}";
    }

    private void RefreshPlansView()
    {
        PlansCollectionView.ItemsSource = null;
        PlansCollectionView.ItemsSource = _plans;
    }
}
