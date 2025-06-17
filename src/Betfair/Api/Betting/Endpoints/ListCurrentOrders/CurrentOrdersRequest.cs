using Betfair.Api.Betting.Endpoints.ListCurrentOrders.Enums;
using Betfair.Api.Betting.Enums;

namespace Betfair.Api.Betting.Endpoints.ListCurrentOrders;

internal class CurrentOrdersRequest
{
    [JsonPropertyName("betIds")]
    public List<string>? BetIds { get; set; }

    [JsonPropertyName("marketIds")]
    public List<string>? MarketIds { get; set; }

    [JsonPropertyName("orderProjection")]
    public OrderProjection? OrderProjection { get; set; }

    [JsonPropertyName("customerOrderRefs")]
    public List<string>? CustomerOrderRefs { get; set; }

    [JsonPropertyName("customerStrategyRefs")]
    public List<string>? CustomerStrategyRefs { get; set; }

    [JsonPropertyName("dateRange")]
    public DateRange? DateRange { get; set; }

    [JsonPropertyName("orderBy")]
    public OrderBy? OrderBy { get; set; }

    [JsonPropertyName("sortDir")]
    public SortDir? SortDir { get; set; }

    [JsonPropertyName("fromRecord")]
    public int FromRecord { get; set; }

    [JsonPropertyName("recordCount")]
    public int RecordCount { get; set; } = 1000;
}
