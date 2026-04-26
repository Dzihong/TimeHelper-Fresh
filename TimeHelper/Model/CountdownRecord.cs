namespace TimeHelper.Models;

/// <summary>
/// Timer record.
/// </summary>
public class CountdownRecord
{
    /// <summary>
    /// Done time.
    /// </summary>
    public DateTime CompletedAt { get; set; }

    /// <summary>
    /// Done minutes.
    /// </summary>
    public int Minutes { get; set; }

    /// <summary>
    /// Plan name.
    /// </summary>
    public string PlanName { get; set; } = string.Empty;
}
