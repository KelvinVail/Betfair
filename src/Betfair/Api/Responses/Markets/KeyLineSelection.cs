namespace Betfair.Api.Responses.Markets;

/// <summary>
/// Description of a markets key line selection, comprising the selectionId and handicap of the team it is applied to.
/// </summary>
public class KeyLineSelection
{
    /// <summary>
    /// Gets the selection ID of the runner in the key line handicap.
    /// </summary>
    [JsonPropertyName("selectionId")]
    public long SelectionId { get; internal set; }

    /// <summary>
    /// Gets the handicap value of the key line.
    /// </summary>
    [JsonPropertyName("handicap")]
    public double Handicap { get; internal set; }
}
