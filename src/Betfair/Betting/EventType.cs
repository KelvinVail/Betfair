namespace Betfair.Betting;

public class EventType : ValueObject
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Id;
    }
}
