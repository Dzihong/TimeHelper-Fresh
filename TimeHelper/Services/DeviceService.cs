#if ANDROID
using Android.Media;
#endif

namespace TimeHelper.Services;

/// <summary>
/// 负责设备原生能力。
/// </summary>
public static class DeviceService
{
#if ANDROID
    private static MediaPlayer? _mediaPlayer;
#endif

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

    public static Task TryPlayAlarmAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
        {
            return Task.CompletedTask;
        }

#if ANDROID
        try
        {
            _mediaPlayer?.Stop();
            _mediaPlayer?.Release();

            _mediaPlayer = new MediaPlayer();
            _mediaPlayer.SetDataSource(filePath);
            _mediaPlayer.Prepare();
            _mediaPlayer.Start();
        }
        catch (Exception)
        {
        }
#endif

        return Task.CompletedTask;
    }

    public static Task StopAlarmAsync()
    {
#if ANDROID
        try
        {
            _mediaPlayer?.Stop();
            _mediaPlayer?.Release();
            _mediaPlayer = null;
        }
        catch (Exception)
        {
        }
#endif

        return Task.CompletedTask;
    }
}
