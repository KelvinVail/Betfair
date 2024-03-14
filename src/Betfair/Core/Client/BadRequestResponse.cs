namespace Betfair.Core.Client;

internal class BadRequestResponse
{
    [JsonPropertyName("detail")]
    public BadRequestDetail Detail { get; set; } = new ();
}
