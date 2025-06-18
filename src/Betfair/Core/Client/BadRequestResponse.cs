namespace Betfair.Core.Client;

internal class BetfairErrorResponse
{
    [JsonPropertyName("faultcode")]
    public string? FaultCode { get; set; }

    [JsonPropertyName("faultstring")]
    public string? FaultString { get; set; }

    [JsonPropertyName("detail")]
    public BetfairErrorDetail Detail { get; set; } = new ();
}
