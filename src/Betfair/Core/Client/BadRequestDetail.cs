namespace Betfair.Core.Client;

internal class BadRequestDetail
{
    [JsonPropertyName("apiNgException")]
    public BadRequestErrorCode ApiNgException { get; set; } = new ();
}
