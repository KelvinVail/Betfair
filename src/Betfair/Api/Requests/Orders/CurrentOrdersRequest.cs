﻿namespace Betfair.Api.Requests.Orders;

internal class CurrentOrdersRequest
{
    [JsonPropertyName("betIds")]
    public List<string>? BetIds { get; set; }

    [JsonPropertyName("marketIds")]
    public List<string>? MarketIds { get; set; }

    [JsonPropertyName("orderProjection")]
    public string? OrderProjection { get; set; }

    [JsonPropertyName("customerOrderRefs")]
    public List<string>? CustomerOrderRefs { get; set; }

    [JsonPropertyName("customerStrategyRefs")]
    public List<string>? CustomerStrategyRefs { get; set; }

    [JsonPropertyName("dateRange")]
    public DateRange? DateRange { get; set; }

    [JsonPropertyName("orderBy")]
    public string? OrderBy { get; set; }

    [JsonPropertyName("sortDir")]
    public string? SortDir { get; set; }

    [JsonPropertyName("fromRecord")]
    public int FromRecord { get; set; }

    [JsonPropertyName("recordCount")]
    public int RecordCount { get; set; }
}
