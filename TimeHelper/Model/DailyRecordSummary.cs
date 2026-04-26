namespace TimeHelper.Models;

/// <summary>
/// Day summary.
/// </summary>
public class DailyRecordSummary
{
    /// <summary>
    /// Date.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Count.
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Total minutes.
    /// </summary>
    public int TotalMinutes { get; set; }
}
