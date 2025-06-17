namespace Betfair.Api.Betting.Endpoints.ListClearedOrders.Requests;

internal class ClearedOrdersRequest
{
    [JsonPropertyName("betStatus")]
    public string BetStatus { get; set; } = string.Empty;

    [JsonPropertyName("eventTypeIds")]
    public List<string>? EventTypeIds { get; set; }

    [JsonPropertyName("eventIds")]
    public List<string>? EventIds { get; set; }

    [JsonPropertyName("marketIds")]
    public List<string>? MarketIds { get; set; }

    [JsonPropertyName("runnerIds")]
    public List<long>? RunnerIds { get; set; }

    [JsonPropertyName("betIds")]
    public List<string>? BetIds { get; set; }

    [JsonPropertyName("customerOrderRefs")]
    public List<string>? CustomerOrderRefs { get; set; }

    [JsonPropertyName("customerStrategyRefs")]
    public List<string>? CustomerStrategyRefs { get; set; }

    [JsonPropertyName("side")]
    public string? Side { get; set; }

    [JsonPropertyName("settledDateRange")]
    public DateRange? SettledDateRange { get; set; }

    [JsonPropertyName("groupBy")]
    public string? GroupBy { get; set; }

    [JsonPropertyName("includeItemDescription")]
    public bool IncludeItemDescription { get; set; }

    [JsonPropertyName("locale")]
    public string? Locale { get; set; }

    [JsonPropertyName("fromRecord")]
    public int FromRecord { get; set; }

    [JsonPropertyName("recordCount")]
    public int RecordCount { get; set; }
}
