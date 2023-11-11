namespace Betfair.Responses;

public sealed class MarketEvent
{
    public string Id { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string Timezone { get; init; } = string.Empty;

    public DateTimeOffset OpenDate { get; init; }
}
