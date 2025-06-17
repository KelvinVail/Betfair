namespace Betfair.Api.Betting.Endpoints.ListMarketBook.Responses;

/// <summary>
/// A list of KeyLineSelection objects describing the key line for the market.
/// </summary>
public class KeyLineDescription
{
    /// <summary>
    /// Gets the list of KeyLineSelection objects.
    /// </summary>
    [JsonPropertyName("keyLine")]
    public List<KeyLineSelection> KeyLine { get; init; } = new ();
}
