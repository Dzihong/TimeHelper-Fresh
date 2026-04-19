using TimeHelper.Models;
using TimeHelper.Services;

namespace TimeHelper.Views;

/// <summary>
/// 倒计时页面。
/// 负责倒计时的启动、暂停、重置，以及方案和完成记录的保存。
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
    private string _selectedPlanName = string.Empty;

    public CountdownPage()
    {
        InitializeComponent();

        InitializeTimer();
        UpdateTimerDisplay();
        UpdateStatus("准备就绪");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _plans = await StorageService.LoadPlansAsync();
        _records = await StorageService.LoadRecordsAsync();

        RefreshPlansView();
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
            UpdateStatus("已结束");

            await SaveCompletionRecordAsync();
            await DisplayAlertAsync("时间助手", "倒计时结束。", "确定");
        }
    }

    private async void OnStartClicked(object? sender, EventArgs e)
    {
        if (_isRunning)
        {
            return;
        }

        if (_isPaused && _remainingTime > TimeSpan.Zero)
        {
            _timer?.Start();
            _isRunning = true;
            _isPaused = false;
            UpdateStatus("运行中");
            return;
        }

        bool isValid = int.TryParse(MinutesEntry.Text, out int minutes);
        if (!isValid || minutes <= 0)
        {
            await DisplayAlertAsync("输入无效", "请输入大于 0 的整数分钟数。", "确定");
            return;
        }

        _initialTime = TimeSpan.FromMinutes(minutes);
        _remainingTime = _initialTime;
        _selectedPlanName = string.Empty;

        UpdateTimerDisplay();

        _timer?.Start();
        _isRunning = true;
        _isPaused = false;
        UpdateStatus("运行中");
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
        UpdateStatus("已暂停");
    }

    private void OnResetClicked(object? sender, EventArgs e)
    {
        StopTimer();
        _remainingTime = _initialTime;
        UpdateTimerDisplay();
        UpdateStatus("准备就绪");
    }

    private async void OnSavePlanClicked(object? sender, EventArgs e)
    {
        bool isValidMinutes = int.TryParse(MinutesEntry.Text, out int minutes);
        if (!isValidMinutes || minutes <= 0)
        {
            await DisplayAlertAsync("无法保存", "请先输入有效的倒计时分钟数。", "确定");
            return;
        }

        string planName = PlanNameEntry.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(planName))
        {
            await DisplayAlertAsync("无法保存", "请输入方案名称。", "确定");
            return;
        }

        bool alreadyExists = _plans.Any(p => p.Name.Equals(planName, StringComparison.OrdinalIgnoreCase));
        if (alreadyExists)
        {
            await DisplayAlertAsync("无法保存", "已存在同名方案，请更换一个名称。", "确定");
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

        await DisplayAlertAsync("保存成功", "方案已保存到本地。", "确定");
    }

    private void OnUsePlanClicked(object? sender, EventArgs e)
    {
        if (sender is not Button button || button.CommandParameter is not CountdownPlan plan)
        {
            return;
        }

        MinutesEntry.Text = plan.Minutes.ToString();
        _initialTime = TimeSpan.FromMinutes(plan.Minutes);
        _remainingTime = _initialTime;
        _selectedPlanName = plan.Name;

        UpdateTimerDisplay();
        UpdateStatus($"已选择方案：{plan.Name}");
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
        StatusLabel.Text = $"状态：{status}";
    }

    private void RefreshPlansView()
    {
        PlansCollectionView.ItemsSource = null;
        PlansCollectionView.ItemsSource = _plans;
    }
}
