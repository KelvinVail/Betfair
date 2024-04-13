namespace Betfair.Core;

public sealed class Side
{
    private Side(string value) => Value = value;

    /// <summary>
    /// Gets the side of Back.
    /// To back a team, horse or outcome is to bet on the selection to win.
    /// For LINE markets a Back bet refers to a SELL line.
    /// A SELL line will win if the outcome is LESS THAN the taken line (price).
    /// </summary>
    /// <value>The side of Back.</value>
    public static Side Back { get; } = new ("BACK");

    /// <summary>
    /// Gets the side of Lay.
    /// To lay a team, horse, or outcome is to bet on the selection to lose.
    /// For LINE markets a Lay bet refers to a BUY line.
    /// A BUY line will win if the outcome is MORE THAN the taken line (price).
    /// </summary>
    /// <value>The side of Lay.</value>
    public static Side Lay { get; } = new ("LAY");

    public string Value { get; }

    public override string ToString() => Value;
}
