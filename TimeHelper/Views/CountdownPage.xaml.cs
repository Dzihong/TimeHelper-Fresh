using TimeHelper.Models;

namespace TimeHelper.Views;

public partial class CountdownPage : ContentPage
{
    private TimeSpan _remainingTime = TimeSpan.Zero;
    private TimeSpan _initialTime = TimeSpan.Zero;

    private bool _isRunning = false;
    private bool _isPaused = false;

    private IDispatcherTimer? _timer;

    private readonly List<CountdownPlan> _plans = new();
    private readonly List<CountdownRecord> _records = new();

    public CountdownPage()
    {
        InitializeComponent();
        InitializeTimer();
        UpdateTimerDisplay();
        UpdateStatus("准备就绪");
        RefreshPlansView();
    }

    private void InitializeTimer()
    {
        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += OnTimerTick;
    }

    private void OnTimerTick(object? sender, EventArgs e)
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

            SaveCompletionRecord();

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("时间助手", "倒计时结束。", "确定");
            });
        }
    }

    private async void OnStartClicked(object sender, EventArgs e)
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
            await DisplayAlert("输入无效", "请输入大于 0 的整数分钟数。", "确定");
            return;
        }

        _initialTime = TimeSpan.FromMinutes(minutes);
        _remainingTime = _initialTime;

        UpdateTimerDisplay();

        _timer?.Start();
        _isRunning = true;
        _isPaused = false;
        UpdateStatus("运行中");
    }

    private void OnPauseClicked(object sender, EventArgs e)
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

    private void OnResetClicked(object sender, EventArgs e)
    {
        StopTimer();
        _remainingTime = _initialTime;
        UpdateTimerDisplay();
        UpdateStatus("准备就绪");
    }

    private async void OnSavePlanClicked(object sender, EventArgs e)
    {
        bool isValidMinutes = int.TryParse(MinutesEntry.Text, out int minutes);

        if (!isValidMinutes || minutes <= 0)
        {
            await DisplayAlert("无法保存", "请先输入有效的倒计时分钟数。", "确定");
            return;
        }

        string planName = PlanNameEntry.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(planName))
        {
            await DisplayAlert("无法保存", "请输入方案名称。", "确定");
            return;
        }

        _plans.Add(new CountdownPlan
        {
            Name = planName,
            Minutes = minutes
        });

        PlanNameEntry.Text = string.Empty;
        RefreshPlansView();

        await DisplayAlert("保存成功", "方案已保存。", "确定");
    }

    private void OnUsePlanClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is CountdownPlan plan)
        {
            MinutesEntry.Text = plan.Minutes.ToString();
            _initialTime = TimeSpan.FromMinutes(plan.Minutes);
            _remainingTime = _initialTime;
            UpdateTimerDisplay();
            UpdateStatus($"已选择方案：{plan.Name}");
        }
    }

    private void SaveCompletionRecord()
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
            PlanName = PlanNameEntry.Text?.Trim() ?? string.Empty
        });
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