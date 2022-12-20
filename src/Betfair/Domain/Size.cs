using System.Diagnostics.CodeAnalysis;

namespace Betfair.Domain;

public sealed class Size : ValueObject
{
    private Size(decimal value) =>
        Value = value;

    public decimal Value { get; }

    public static bool operator >([NotNull]Size a, [NotNull]Size b) =>
        a.Value > b.Value;

    public static bool operator >=([NotNull]Size a, [NotNull]Size b) =>
        a.Value >= b.Value;

    public static bool operator <([NotNull]Size a, [NotNull]Size b) =>
        a.Value < b.Value;

    public static bool operator <=([NotNull]Size a, [NotNull]Size b) =>
        a.Value <= b.Value;

    public static Size operator +([NotNull]Size a, [NotNull]Size b) =>
        Of(a.Value + b.Value);

    public static Size operator -([NotNull]Size a, [NotNull]Size b) =>
        Of(a.Value - b.Value);

    public static Size operator *([NotNull]Size a, decimal b) =>
        Of(a.Value * b);

    public static Size operator /([NotNull]Size a, decimal b) =>
        Of(a.Value / b);

    public static Size Of(decimal value) =>
        value > 0 ? new Size(Math.Floor(value * 100) / 100) : new Size(0);

    public Size Add([NotNull]Size size) =>
        Of(Value + size.Value);

    public Size Subtract([NotNull]Size size) =>
        Of(Value - size.Value);

    public Size Multiply(decimal value) =>
        Of(Value * value);

    public Size Divide(decimal value) =>
        Of(Value / value);

    public override string ToString() =>
        $"{Value:#,0.00}";

    public int CompareTo([NotNull]Size other)
    {
        if (Value == other.Value) return 0;
        if (Value > other.Value) return 1;

        return -1;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}