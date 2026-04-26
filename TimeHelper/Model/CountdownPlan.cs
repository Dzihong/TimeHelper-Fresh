namespace TimeHelper.Models;

/// <summary>
/// Timer plan.
/// </summary>
public class CountdownPlan
{
    /// <summary>
    /// Plan name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Minutes.
    /// </summary>
    public int Minutes { get; set; }
}
