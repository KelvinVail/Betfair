namespace Betfair;

internal class UpperCaseEnumJsonConverter<T> : JsonConverter<T>
    where T : Enum
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return (T)Enum.Parse(typeToConvert, reader.GetString() !, true);
    }

    public override void Write([NotNull]Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString().ToUpperInvariant());
    }
}
