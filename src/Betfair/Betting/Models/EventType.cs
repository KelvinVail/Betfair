namespace Betfair.Betting.Models;

public class EventType : ValueObject
{
    public string Id { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Id;
    }
}
