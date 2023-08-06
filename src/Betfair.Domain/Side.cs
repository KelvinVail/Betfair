namespace Betfair.Domain;

public class Side : ValueObject
{
    private Side(string value) =>
        Value = value;

    public static Side Back { get; } = new ("Back");

    public static Side Lay { get; } = new ("Lay");

    public string Value { get; }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Value;
    }
}
