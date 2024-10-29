using System.Text;

namespace Betfair.Extensions.Contracts;

public sealed class LazyString
{
    private readonly byte[] _value;

    public LazyString(ReadOnlySpan<byte> value) =>
        _value = value.ToArray();

    public string Value => Encoding.UTF8.GetString(_value);

    public static implicit operator string(LazyString lazyString) => lazyString.Value;

    public static implicit operator LazyString(string value) => new (Encoding.UTF8.GetBytes(value));

    public static implicit operator ReadOnlySpan<byte>(LazyString lazyString) => lazyString._value;

    public static implicit operator LazyString(ReadOnlySpan<byte> value) => new(value);

    public override string ToString() => Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override bool Equals(object obj)
    {
        if  (obj is LazyString lazyString) return Value == lazyString.Value;
        if (obj is string str) return Value == str;
        return false;
    }

    public static bool operator ==(LazyString left, LazyString right) =>
        left.Value == right.Value;

    public static bool operator !=(LazyString left, LazyString right) =>
        left.Value != right.Value;

    public static bool operator ==(LazyString left, string right) =>
        left.Value == right;

    public static bool operator !=(LazyString left, string right) =>
        left.Value != right;

    public static bool operator ==(string left, LazyString right) =>
        left == right.Value;

    public static bool operator !=(string left, LazyString right) =>
        left != right.Value;

    public static bool operator ==(LazyString left, ReadOnlySpan<byte> right) =>
        left._value.SequenceEqual(right.ToArray());

    public static bool operator !=(LazyString left, ReadOnlySpan<byte> right) =>
        !left._value.SequenceEqual(right.ToArray());

    public static bool operator ==(ReadOnlySpan<byte> left, LazyString right) =>
        left.SequenceEqual(right._value);

    public static bool operator !=(ReadOnlySpan<byte> left, LazyString right) =>
        !left.SequenceEqual(right._value);

    public static bool operator ==(LazyString left, byte[] right) =>
        left._value.SequenceEqual(right);

    public static bool operator !=(LazyString left, byte[] right) =>
        !left._value.SequenceEqual(right);

    public static bool operator ==(byte[] left, LazyString right) =>
        left.SequenceEqual(right._value);

    public static bool operator !=(byte[] left, LazyString right) =>
        !left.SequenceEqual(right._value);

    public static bool operator ==(LazyString left, ReadOnlyMemory<byte> right) =>
        left._value.AsMemory().Span.SequenceEqual(right.Span);

    public static bool operator !=(LazyString left, ReadOnlyMemory<byte> right) =>
        !left._value.AsMemory().Span.SequenceEqual(right.Span);

    public static bool operator ==(ReadOnlyMemory<byte> left, LazyString right) =>
        left.Span.SequenceEqual(right._value.AsMemory().Span);

    public static bool operator !=(ReadOnlyMemory<byte> left, LazyString right) =>
        !left.Span.SequenceEqual(right._value.AsMemory().Span);
}
