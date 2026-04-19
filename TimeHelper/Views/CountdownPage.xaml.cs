using TimeHelper.Models;
using TimeHelper.Services;

namespace TimeHelper.Views;

/// <summary>
/// 倒计时页面
/// 负责倒计时的启动、暂停、重置、保存方案与记录完成历史
/// </summary>
public partial class CountdownPage : ContentPage
{
    // 当前剩余时间
    private TimeSpan _remainingTime = TimeSpan.Zero;

    // 初始设定时间，用于重置
    private TimeSpan _initialTime = TimeSpan.Zero;

    // 当前是否正在运行
    private bool _isRunning = false;

    // 当前是否处于暂停状态
    private bool _isPaused = false;

    // MAUI 计时器，每秒触发一次
    private IDispatcherTimer? _timer;

    // 已保存的方案列表
    private List<CountdownPlan> _plans = new();

    // 倒计时完成记录列表
    private List<CountdownRecord> _records = new();

    // 当前选中的方案名称
    private string _selectedPlanName = string.Empty;

    public CountdownPage()
    {
        InitializeComponent();

        // 初始化计时器
        InitializeTimer();

        // 初始化界面显示
        UpdateTimerDisplay();
        UpdateStatus("准备就绪");
    }

    /// <summary>
    /// 页面显示时触发
    /// 用于加载本地保存的方案和记录
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _plans = await StorageService.LoadPlansAsync();
        _records = await StorageService.LoadRecordsAsync();

        RefreshPlansView();
    }

    /// <summary>
    /// 初始化计时器，每秒执行一次 Tick 事件
    /// </summary>
    private void InitializeTimer()
    {
        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += OnTimerTick;
    }

    /// <summary>
    /// 每秒执行一次的倒计时逻辑
    /// </summary>
    private async void OnTimerTick(object? sender, EventArgs e)
    {
        if (_remainingTime > TimeSpan.Zero)
        {
            _remainingTime = _remainingTime.Subtract(TimeSpan.FromSeconds(1));
            UpdateTimerDisplay();
        }

        // 当倒计时结束时，停止计时器并保存完成记录
        if (_remainingTime <= TimeSpan.Zero)
        {
            StopTimer();
            _remainingTime = TimeSpan.Zero;
            UpdateTimerDisplay();
            UpdateStatus("已结束");

            await SaveCompletionRecordAsync();

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("时间助手", "倒计时结束。", "确定");
            });
        }
    }

    /// <summary>
    /// 点击“开始”按钮
    /// 如果当前是暂停状态，则继续；
    /// 如果是全新开始，则从输入框读取分钟数
    /// </summary>
    private async void OnStartClicked(object? sender, EventArgs e)
    {
        if (_isRunning)
        {
            return;
        }

        // 如果当前是暂停状态，则直接继续倒计时
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

        // 设置初始时间和剩余时间
        _initialTime = TimeSpan.FromMinutes(minutes);
        _remainingTime = _initialTime;

        // 如果用户不是通过“使用此方案”进入，而是手动输入，
        // 则当前方案名称清空，避免错误记录旧方案名
        _selectedPlanName = string.Empty;

        UpdateTimerDisplay();

        _timer?.Start();
        _isRunning = true;
        _isPaused = false;
        UpdateStatus("运行中");
    }

    /// <summary>
    /// 点击“暂停”按钮
    /// </summary>
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

    /// <summary>
    /// 点击“重置”按钮
    /// 将倒计时恢复到初始时间
    /// </summary>
    private void OnResetClicked(object? sender, EventArgs e)
    {
        StopTimer();
        _remainingTime = _initialTime;
        UpdateTimerDisplay();
        UpdateStatus("准备就绪");
    }

    /// <summary>
    /// 点击“保存当前方案”按钮
    /// 将当前输入的分钟数和方案名保存到本地
    /// </summary>
    private async void OnSavePlanClicked(object? sender, EventArgs e)
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

        // 防止同名方案重复保存
        bool alreadyExists = _plans.Any(p => p.Name.Equals(planName, StringComparison.OrdinalIgnoreCase));

        if (alreadyExists)
        {
            await DisplayAlert("无法保存", "已存在同名方案，请换一个名称。", "确定");
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

        await DisplayAlert("保存成功", "方案已保存到本地。", "确定");
    }

    /// <summary>
    /// 点击“使用此方案”按钮
    /// 自动把方案分钟数填入输入框，并更新当前倒计时显示
    /// </summary>
    private void OnUsePlanClicked(object? sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is CountdownPlan plan)
        {
            MinutesEntry.Text = plan.Minutes.ToString();
            _initialTime = TimeSpan.FromMinutes(plan.Minutes);
            _remainingTime = _initialTime;
            _selectedPlanName = plan.Name;

            UpdateTimerDisplay();
            UpdateStatus($"已选择方案：{plan.Name}");
        }
    }

    /// <summary>
    /// 保存一次倒计时完成记录到本地
    /// </summary>
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

    /// <summary>
    /// 停止计时器并重置状态
    /// </summary>
    private void StopTimer()
    {
        _timer?.Stop();
        _isRunning = false;
        _isPaused = false;
    }

    /// <summary>
    /// 更新页面上的时间显示
    /// 格式为：分钟:秒
    /// </summary>
    private void UpdateTimerDisplay()
    {
        int totalMinutes = (int)_remainingTime.TotalMinutes;
        int seconds = _remainingTime.Seconds;

        TimerLabel.Text = $"{totalMinutes:D2}:{seconds:D2}";
    }

    /// <summary>
    /// 更新状态提示文字
    /// </summary>
    private void UpdateStatus(string status)
    {
        StatusLabel.Text = $"状态：{status}";
    }

    /// <summary>
    /// 刷新方案列表显示
    /// </summary>
    private void RefreshPlansView()
    {
        PlansCollectionView.ItemsSource = null;
        PlansCollectionView.ItemsSource = _plans;
    }
}