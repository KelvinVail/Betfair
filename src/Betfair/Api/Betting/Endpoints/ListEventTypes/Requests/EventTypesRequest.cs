namespace Betfair.Api.Betting.Endpoints.ListEventTypes.Requests;

internal class EventTypesRequest
{
    [JsonPropertyName("filter")]
    public ApiMarketFilter Filter { get; set; } = new ();

    [JsonPropertyName("locale")]
    public string? Locale { get; set; }
}

