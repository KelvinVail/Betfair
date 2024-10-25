using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Betfair.Extensions.Markets;

internal class PriceJsonConverter : JsonConverter<Price>
{
    public override Price Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        Price.Of(reader.GetDouble());

    public override void Write([NotNull]Utf8JsonWriter writer, [NotNull]Price value, JsonSerializerOptions options) =>
        writer.WriteNumberValue(value.DecimalOdds);
}