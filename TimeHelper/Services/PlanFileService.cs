using System.Text.Json;
using TimeHelper.Models;

namespace TimeHelper.Services;

/// <summary>
/// Plan files.
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
            return (true, $"Plans exported to {filePath}");
        }
        catch (Exception)
        {
            return (false, "Export failed. Please try again.");
        }
    }

    public static async Task<(bool Success, string Message, List<CountdownPlan> Plans)> ImportPlansAsync()
    {
        try
        {
            PickOptions options = new()
            {
                PickerTitle = "Select a plan JSON file",
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
                return (false, "Import cancelled.", new List<CountdownPlan>());
            }

            await using Stream stream = await result.OpenReadAsync();
            List<CountdownPlan>? plans = await JsonSerializer.DeserializeAsync<List<CountdownPlan>>(stream);

            if (plans is null)
            {
                return (false, "The selected file is not valid.", new List<CountdownPlan>());
            }

            List<CountdownPlan> validPlans = plans
                .Where(plan => !string.IsNullOrWhiteSpace(plan.Name) && plan.Minutes > 0)
                .ToList();

            if (validPlans.Count == 0)
            {
                return (false, "No valid plans were found in this file.", new List<CountdownPlan>());
            }

            return (true, "Plans loaded successfully.", validPlans);
        }
        catch (JsonException)
        {
            return (false, "The selected file is not valid JSON.", new List<CountdownPlan>());
        }
        catch (Exception)
        {
            return (false, "Import failed. Please try again.", new List<CountdownPlan>());
        }
    }
}
