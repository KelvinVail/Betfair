namespace Betfair.Extensions.ByteReaders;

internal static class ByteReaderExtensions
{
    internal static bool PropertyValue(this ref BetfairJsonReader reader, ReadOnlySpan<byte> key, ReadOnlySpan<byte> value)
    {
        if (reader.TokenType != JsonTokenType.PropertyName || !reader.ValueSpan.SequenceEqual(key)) return false;
        reader.Read();
        return reader.ValueSpan.SequenceEqual(value);
    }

    internal static bool PropertyName(this ref BetfairJsonReader reader, ReadOnlySpan<byte> key) =>
        reader.TokenType == JsonTokenType.PropertyName && reader.ValueSpan.SequenceEqual(key);

    internal static ReadOnlySpan<byte> NextProperty(this ref BetfairJsonReader reader)
    {
        if (reader.TokenType == JsonTokenType.PropertyName)
            return reader.ValueSpan;

        while (reader.Read())
        {
            if (reader.TokenType != JsonTokenType.PropertyName) continue;

            return reader.ValueSpan;
        }

        return "end"u8;
    }

    internal static bool Value(this ref BetfairJsonReader reader, ReadOnlySpan<byte> value) =>
        reader.ValueSpan.SequenceEqual(value);

    internal static bool EndOfObject(this ref BetfairJsonReader reader, ref int objectCount)
    {
        if (reader.TokenType == JsonTokenType.StartObject)
        {
            objectCount++;
        }
        else if (reader.TokenType == JsonTokenType.EndObject)
        {
            objectCount--;
            if (objectCount == 0) return true;
        }

        return false;
    }
}
