namespace Betfair.Api.Betting.Endpoints.ListTimeRanges.Requests;

internal class TimeRangesRequest
{
    [JsonPropertyName("filter")]
    public ApiMarketFilter Filter { get; set; } = new ();

    [JsonPropertyName("granularity")]
    public string Granularity { get; set; } = "DAYS";
}
