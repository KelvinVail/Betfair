using Betfair.Extensions.Markets;
using Betfair.Extensions.Markets.Enums;

namespace Betfair.Extensions.JsonReaders;

internal static class MarketDefinitionJsonReader
{
    private static readonly Dictionary<byte, MarketStatus> _marketStatusMap = new ()
    {
        [(byte)'O'] = MarketStatus.Open,
        [(byte)'S'] = MarketStatus.Suspended,
        [(byte)'C'] = MarketStatus.Closed,
        [(byte)'I'] = MarketStatus.Inactive,
    };

    internal static void ReadMarketDefinition(this Market market, ref Utf8JsonReader reader)
    {
        if (!reader.PropertyName("marketDefinition"u8)) return;

        var objectCount = 0;
        while (reader.Read())
        {
            if (reader.EndOfObject(ref objectCount)) break;

            market.ReadStartTime(ref reader);
            market.ReadInPlayStatus(ref reader);
            market.ReadMarketStatus(ref reader);
            market.ReadMarketVersion(ref reader);
            market.ReadRunners(ref reader);
        }
    }

    private static void ReadStartTime(this Market market, ref Utf8JsonReader reader)
    {
        if (!reader.PropertyName("marketTime"u8)) return;

        reader.Read();
        market.StartTime = reader.GetDateTimeOffset();
    }

    private static void ReadInPlayStatus(this Market market, ref Utf8JsonReader reader)
    {
        if (!reader.PropertyName("inPlay"u8)) return;

        reader.Read();
        market.IsInPlay = reader.GetBoolean();
    }

    private static void ReadMarketStatus(this Market market, ref Utf8JsonReader reader)
    {
        if (!reader.PropertyName("status"u8)) return;

        reader.Read();
        market.Status = _marketStatusMap[reader.ValueSpan[0]];
    }

    private static void ReadMarketVersion(this Market market, ref Utf8JsonReader reader)
    {
        if (!reader.PropertyName("version"u8)) return;

        reader.Read();
        market.Version = reader.GetInt64();
    }
}
