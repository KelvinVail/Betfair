namespace Betfair.Extensions.JsonReaders;

internal static class JsonReaderExtensions
{
    internal static bool PropertyValue(this ref Utf8JsonReader reader, ReadOnlySpan<byte> key, ReadOnlySpan<byte> value)
    {
        if (reader.TokenType != JsonTokenType.PropertyName || !reader.ValueSpan.SequenceEqual(key)) return false;
        reader.Read();
        return reader.ValueSpan.SequenceEqual(value);
    }

    internal static bool PropertyName(this ref Utf8JsonReader reader, ReadOnlySpan<byte> key) =>
        reader.TokenType == JsonTokenType.PropertyName && reader.ValueSpan.SequenceEqual(key);

    internal static bool Value(this ref Utf8JsonReader reader, ReadOnlySpan<byte> value) =>
        reader.ValueSpan.SequenceEqual(value);

    internal static bool EndOfObject(this ref Utf8JsonReader reader, ref int objectCount)
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
