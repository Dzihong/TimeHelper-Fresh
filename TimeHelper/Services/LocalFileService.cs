using TimeHelper.Models;

namespace TimeHelper.Services;

/// <summary>
/// Local files.
/// </summary>
public static class LocalFileService
{
    public static async Task<(bool Success, string Message, string FilePath)> PickAvatarAsync()
    {
        try
        {
            PickOptions options = new()
            {
                PickerTitle = "Choose an avatar image",
                FileTypes = FilePickerFileType.Images
            };

            FileResult? result = await FilePicker.Default.PickAsync(options);
            if (result is null)
            {
                return (false, "Avatar selection cancelled.", string.Empty);
            }

            string savedPath = await StorageService.SavePickedFileAsync(result, "avatar");
            return (true, "Avatar selected successfully.", savedPath);
        }
        catch (Exception)
        {
            return (false, "Unable to select an avatar file.", string.Empty);
        }
    }

    public static async Task<(bool Success, string Message, string FilePath)> PickAlarmMusicAsync()
    {
        try
        {
            PickOptions options = new()
            {
                PickerTitle = "Choose an MP3 alarm file",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "audio/mpeg", "audio/mp3", "*/*" } },
                    { DevicePlatform.WinUI, new[] { ".mp3" } },
                    { DevicePlatform.iOS, new[] { "public.mp3", "public.audio" } },
                    { DevicePlatform.MacCatalyst, new[] { "public.mp3", "public.audio" } }
                })
            };

            FileResult? result = await FilePicker.Default.PickAsync(options);
            if (result is null)
            {
                return (false, "Alarm selection cancelled.", string.Empty);
            }

            string extension = Path.GetExtension(result.FileName);
            if (!extension.Equals(".mp3", StringComparison.OrdinalIgnoreCase))
            {
                return (false, "Please choose an MP3 file.", string.Empty);
            }

            string savedPath = await StorageService.SavePickedFileAsync(result, "alarm");
            return (true, "Alarm file selected successfully.", savedPath);
        }
        catch (Exception)
        {
            return (false, "Unable to select an MP3 file.", string.Empty);
        }
    }
}
