namespace Betfair.Api.Betting.Enums;

internal class SnakeCaseEnumJsonConverter<T> : JsonConverter<T>
    where T : Enum
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var enumString = reader.GetString() !;
        var originalName = Enum.GetNames(typeToConvert).FirstOrDefault(n => ToUpperSnakeCase(n) == enumString);
        if (originalName is not null)
            return (T)Enum.Parse(typeToConvert, originalName, true);

        throw new JsonException($"Unable to convert \"{enumString}\" to Enum \"{typeToConvert}\".");
    }

    public override void Write([NotNull] Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(ToUpperSnakeCase(value.ToString()));
    }

    private static string ToUpperSnakeCase(string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        var builder = new System.Text.StringBuilder();
        for (var i = 0; i < value.Length; i++)
        {
            var c = value[i];
            if (char.IsUpper(c) && i > 0)
                builder.Append('_');
            builder.Append(char.ToUpperInvariant(c));
        }

        return builder.ToString();
    }
}
