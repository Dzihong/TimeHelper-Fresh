namespace TimeHelper.Services;

/// <summary>
/// 设备原生能力服务。
/// </summary>
public static class DeviceService
{
    public static Task TryVibrateAsync()
    {
        try
        {
            Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(300));
        }
        catch (FeatureNotSupportedException)
        {
        }
        catch (Exception)
        {
        }

        return Task.CompletedTask;
    }
}
