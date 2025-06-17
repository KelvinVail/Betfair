namespace Betfair.Api.Betting.Endpoints.ListEvents.Requests;

internal class EventsRequest
{
    [JsonPropertyName("filter")]
    public ApiMarketFilter Filter { get; set; } = new ();

    [JsonPropertyName("locale")]
    public string? Locale { get; set; }
}

