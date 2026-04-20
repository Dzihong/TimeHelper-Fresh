using System.Text.Json;
using TimeHelper.Models;

namespace TimeHelper.Services;

/// <summary>
/// 负责应用内部 JSON 存储。
/// </summary>
public static class StorageService
{
    private static readonly string PlansFilePath =
        Path.Combine(FileSystem.AppDataDirectory, "plans.json");

    private static readonly string RecordsFilePath =
        Path.Combine(FileSystem.AppDataDirectory, "records.json");

    private static readonly string UserProfileFilePath =
        Path.Combine(FileSystem.AppDataDirectory, "userprofile.json");

    private static readonly string MediaDirectoryPath =
        Path.Combine(FileSystem.AppDataDirectory, "Media");

    public static Task<List<CountdownPlan>> LoadPlansAsync()
    {
        return LoadAsync(PlansFilePath, new List<CountdownPlan>());
    }

    public static Task SavePlansAsync(List<CountdownPlan> plans)
    {
        return SaveAsync(PlansFilePath, plans);
    }

    public static Task<List<CountdownRecord>> LoadRecordsAsync()
    {
        return LoadAsync(RecordsFilePath, new List<CountdownRecord>());
    }

    public static Task SaveRecordsAsync(List<CountdownRecord> records)
    {
        return SaveAsync(RecordsFilePath, records);
    }

    public static Task<UserProfile> LoadUserProfileAsync()
    {
        return LoadAsync(UserProfileFilePath, new UserProfile());
    }

    public static Task SaveUserProfileAsync(UserProfile profile)
    {
        return SaveAsync(UserProfileFilePath, profile);
    }

    public static async Task<string> SavePickedFileAsync(FileResult file, string prefix)
    {
        Directory.CreateDirectory(MediaDirectoryPath);

        string extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(extension))
        {
            extension = ".dat";
        }

        string safeName = $"{prefix}-{DateTime.Now:yyyyMMdd-HHmmss}{extension}";
        string destinationPath = Path.Combine(MediaDirectoryPath, safeName);

        await using Stream sourceStream = await file.OpenReadAsync();
        await using FileStream destinationStream = File.Create(destinationPath);
        await sourceStream.CopyToAsync(destinationStream);

        return destinationPath;
    }

    private static async Task<T> LoadAsync<T>(string filePath, T defaultValue)
    {
        if (!File.Exists(filePath))
        {
            return defaultValue;
        }

        try
        {
            string json = await File.ReadAllTextAsync(filePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                return defaultValue;
            }

            return JsonSerializer.Deserialize<T>(json) ?? defaultValue;
        }
        catch (JsonException)
        {
            return defaultValue;
        }
        catch (IOException)
        {
            return defaultValue;
        }
        catch (UnauthorizedAccessException)
        {
            return defaultValue;
        }
    }

    private static async Task SaveAsync<T>(string filePath, T value)
    {
        string json = JsonSerializer.Serialize(value, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await File.WriteAllTextAsync(filePath, json);
    }
}
