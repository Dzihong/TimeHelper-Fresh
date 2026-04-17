using System.Text.Json;
using TimeHelper.Models;

namespace TimeHelper.Services;

/// <summary>
/// 本地存储服务
/// 负责保存和读取方案、记录、用户资料
/// </summary>
public static class StorageService
{
    // 方案文件路径
    private static readonly string PlansFilePath =
        Path.Combine(FileSystem.AppDataDirectory, "plans.json");

    // 记录文件路径
    private static readonly string RecordsFilePath =
        Path.Combine(FileSystem.AppDataDirectory, "records.json");

    // 用户资料文件路径
    private static readonly string UserProfileFilePath =
        Path.Combine(FileSystem.AppDataDirectory, "userprofile.json");

    /// <summary>
    /// 读取本地方案列表
    /// </summary>
    public static async Task<List<CountdownPlan>> LoadPlansAsync()
    {
        if (!File.Exists(PlansFilePath))
        {
            return new List<CountdownPlan>();
        }

        string json = await File.ReadAllTextAsync(PlansFilePath);

        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<CountdownPlan>();
        }

        return JsonSerializer.Deserialize<List<CountdownPlan>>(json)
               ?? new List<CountdownPlan>();
    }

    /// <summary>
    /// 保存方案列表
    /// </summary>
    public static async Task SavePlansAsync(List<CountdownPlan> plans)
    {
        string json = JsonSerializer.Serialize(plans, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await File.WriteAllTextAsync(PlansFilePath, json);
    }

    /// <summary>
    /// 读取本地倒计时记录
    /// </summary>
    public static async Task<List<CountdownRecord>> LoadRecordsAsync()
    {
        if (!File.Exists(RecordsFilePath))
        {
            return new List<CountdownRecord>();
        }

        string json = await File.ReadAllTextAsync(RecordsFilePath);

        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<CountdownRecord>();
        }

        return JsonSerializer.Deserialize<List<CountdownRecord>>(json)
               ?? new List<CountdownRecord>();
    }

    /// <summary>
    /// 保存倒计时记录
    /// </summary>
    public static async Task SaveRecordsAsync(List<CountdownRecord> records)
    {
        string json = JsonSerializer.Serialize(records, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await File.WriteAllTextAsync(RecordsFilePath, json);
    }

    /// <summary>
    /// 读取用户资料
    /// </summary>
    public static async Task<UserProfile> LoadUserProfileAsync()
    {
        if (!File.Exists(UserProfileFilePath))
        {
            return new UserProfile();
        }

        string json = await File.ReadAllTextAsync(UserProfileFilePath);

        if (string.IsNullOrWhiteSpace(json))
        {
            return new UserProfile();
        }

        return JsonSerializer.Deserialize<UserProfile>(json)
               ?? new UserProfile();
    }

    /// <summary>
    /// 保存用户资料
    /// </summary>
    public static async Task SaveUserProfileAsync(UserProfile profile)
    {
        string json = JsonSerializer.Serialize(profile, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await File.WriteAllTextAsync(UserProfileFilePath, json);
    }
}