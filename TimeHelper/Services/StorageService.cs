using System.Text.Json;
using TimeHelper.Models;

namespace TimeHelper.Services;

/// <summary>
/// 本地存储服务。
/// 负责保存和读取方案、记录以及用户资料。
/// </summary>
public static class StorageService
{
    private static readonly string PlansFilePath =
        Path.Combine(FileSystem.AppDataDirectory, "plans.json");

    private static readonly string RecordsFilePath =
        Path.Combine(FileSystem.AppDataDirectory, "records.json");

    private static readonly string UserProfileFilePath =
        Path.Combine(FileSystem.AppDataDirectory, "userprofile.json");

    public static async Task<List<CountdownPlan>> LoadPlansAsync()
    {
        return await LoadAsync(PlansFilePath, new List<CountdownPlan>());
    }

    public static async Task SavePlansAsync(List<CountdownPlan> plans)
    {
        await SaveAsync(PlansFilePath, plans);
    }

    public static async Task<List<CountdownRecord>> LoadRecordsAsync()
    {
        return await LoadAsync(RecordsFilePath, new List<CountdownRecord>());
    }

    public static async Task SaveRecordsAsync(List<CountdownRecord> records)
    {
        await SaveAsync(RecordsFilePath, records);
    }

    public static async Task<UserProfile> LoadUserProfileAsync()
    {
        return await LoadAsync(UserProfileFilePath, new UserProfile());
    }

    public static async Task SaveUserProfileAsync(UserProfile profile)
    {
        await SaveAsync(UserProfileFilePath, profile);
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
