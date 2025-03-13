using System.Text.Json.Serialization;

namespace Stackr_Api.Models;

public class AggregateRanking
{
    [JsonPropertyName("Score")]
    public double Score { get; set; }

    [JsonPropertyName("Name")]
    public string Name { get; set; } = string.Empty;

    public int ItemId { get; set; }
    public int ListCount { get; set; }
    public int TotalRankings { get; set; }
} 