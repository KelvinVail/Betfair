namespace Betfair.Api.Requests;

internal class EventTypesRequest
{
    [JsonPropertyName("filter")]
    public ApiMarketFilter Filter { get; set; } = new ();

    [JsonPropertyName("maxResults")]
    public int MaxResults { get; set; } = 1000;
}
