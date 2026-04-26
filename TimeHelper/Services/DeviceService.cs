#if ANDROID
using AndroidMediaPlayer = Android.Media.MediaPlayer;
#elif WINDOWS
using Windows.Media.Core;
using WindowsMediaPlayer = Windows.Media.Playback.MediaPlayer;
#endif
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices;

namespace TimeHelper.Services;

/// <summary>
/// Device tools.
/// </summary>
public static class DeviceService
{
#if ANDROID
    private static AndroidMediaPlayer? _mediaPlayer;
#elif WINDOWS
    private static WindowsMediaPlayer? _mediaPlayer;
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

    public static async Task TryFlashAsync()
    {
        try
        {
            await Flashlight.Default.TurnOnAsync();
            await Task.Delay(700);
            await Flashlight.Default.TurnOffAsync();
        }
        catch (FeatureNotSupportedException)
        {
        }
        catch (PermissionException)
        {
        }
        catch (Exception)
        {
            try
            {
                await Flashlight.Default.TurnOffAsync();
            }
            catch (Exception)
            {
            }
        }
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

            _mediaPlayer = new AndroidMediaPlayer();
            _mediaPlayer.SetDataSource(filePath);
            _mediaPlayer.Prepare();
            _mediaPlayer.Start();
        }
        catch (Exception)
        {
        }
#elif WINDOWS
        try
        {
            _mediaPlayer?.Pause();
            _mediaPlayer?.Dispose();

            _mediaPlayer = new WindowsMediaPlayer
            {
                Source = MediaSource.CreateFromUri(new Uri(filePath))
            };
            _mediaPlayer.Play();
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
#elif WINDOWS
        try
        {
            _mediaPlayer?.Pause();
            _mediaPlayer?.Dispose();
            _mediaPlayer = null;
        }
        catch (Exception)
        {
        }
#endif

        return Task.CompletedTask;
    }
}
