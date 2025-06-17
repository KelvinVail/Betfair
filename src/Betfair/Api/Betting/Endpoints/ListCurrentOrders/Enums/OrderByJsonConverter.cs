namespace Betfair.Api.Betting.Endpoints.ListCurrentOrders.Enums;

internal class OrderByJsonConverter : JsonConverter<OrderBy>
{
    public override OrderBy Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return value switch
        {
            "BY_MARKET" => OrderBy.Market,
            "BY_MATCH_TIME" => OrderBy.MatchTime,
            "BY_PLACE_TIME" => OrderBy.PlaceTime,
            "BY_SETTLED_TIME" => OrderBy.SettledTime,
            "BY_VOID_TIME" => OrderBy.VoidTime,
            _ => throw new JsonException($"Unknown OrderBy value: {value}")
        };
    }

    public override void Write(Utf8JsonWriter writer, OrderBy value, JsonSerializerOptions options)
    {
        var stringValue = value switch
        {
            OrderBy.Market => "BY_MARKET",
            OrderBy.MatchTime => "BY_MATCH_TIME",
            OrderBy.PlaceTime => "BY_PLACE_TIME",
            OrderBy.SettledTime => "BY_SETTLED_TIME",
            OrderBy.VoidTime => "BY_VOID_TIME",
            _ => throw new JsonException($"Unknown OrderBy value: {value}")
        };

        writer.WriteStringValue(stringValue);
    }
}
