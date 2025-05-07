namespace Betfair.Api.Requests;

internal class EventsRequest
{
    [JsonPropertyName("filter")]
    public ApiMarketFilter Filter { get; set; } = new ();
}
