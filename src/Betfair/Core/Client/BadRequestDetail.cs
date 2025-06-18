namespace Betfair.Core.Client;

internal class BetfairErrorDetail
{
    [JsonPropertyName("APINGException")]
    public BetfairApiNgError APINGException { get; set; } = new ();

    [JsonPropertyName("exceptionname")]
    public string? ExceptionName { get; set; }
}
