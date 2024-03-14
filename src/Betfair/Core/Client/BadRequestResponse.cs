namespace Betfair.Core.Client;

[JsonSerializable(typeof(BadRequestResponse))]
internal class BadRequestResponse
{
    [JsonPropertyName("detail")]
    public BadRequestDetail Detail { get; set; } = new ();
}
