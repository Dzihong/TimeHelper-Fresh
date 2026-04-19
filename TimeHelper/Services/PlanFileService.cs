using System.Text.Json;
using TimeHelper.Models;

namespace TimeHelper.Services;

/// <summary>
/// 方案文件导入导出服务。
/// </summary>
public static class PlanFileService
{
    public static async Task<(bool Success, string Message)> ExportPlansAsync(List<CountdownPlan> plans)
    {
        try
        {
            string exportDirectory = Path.Combine(FileSystem.AppDataDirectory, "Exports");
            Directory.CreateDirectory(exportDirectory);

            string fileName = $"plans-{DateTime.Now:yyyyMMdd-HHmmss}.json";
            string filePath = Path.Combine(exportDirectory, fileName);

            string json = JsonSerializer.Serialize(plans, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(filePath, json);
            return (true, $"方案已导出到：{filePath}");
        }
        catch (Exception)
        {
            return (false, "导出失败，请稍后重试。");
        }
    }

    public static async Task<(bool Success, string Message, List<CountdownPlan> Plans)> ImportPlansAsync()
    {
        try
        {
            PickOptions options = new()
            {
                PickerTitle = "选择方案 JSON 文件",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "application/json", "text/json", "*/*" } },
                    { DevicePlatform.WinUI, new[] { ".json" } },
                    { DevicePlatform.iOS, new[] { "public.json", "public.text" } },
                    { DevicePlatform.MacCatalyst, new[] { "public.json", "public.text" } }
                })
            };

            FileResult? result = await FilePicker.Default.PickAsync(options);
            if (result is null)
            {
                return (false, "已取消导入。", new List<CountdownPlan>());
            }

            await using Stream stream = await result.OpenReadAsync();
            List<CountdownPlan>? plans = await JsonSerializer.DeserializeAsync<List<CountdownPlan>>(stream);

            if (plans is null)
            {
                return (false, "导入文件格式无效。", new List<CountdownPlan>());
            }

            List<CountdownPlan> validPlans = plans
                .Where(plan => !string.IsNullOrWhiteSpace(plan.Name) && plan.Minutes > 0)
                .ToList();

            if (validPlans.Count == 0)
            {
                return (false, "没有读取到有效方案。", new List<CountdownPlan>());
            }

            return (true, "方案文件读取成功。", validPlans);
        }
        catch (JsonException)
        {
            return (false, "导入文件不是有效的 JSON。", new List<CountdownPlan>());
        }
        catch (Exception)
        {
            return (false, "导入失败，请稍后重试。", new List<CountdownPlan>());
        }
    }
}
