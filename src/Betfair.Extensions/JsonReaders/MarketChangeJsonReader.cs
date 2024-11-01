using Betfair.Extensions.Markets;

namespace Betfair.Extensions.JsonReaders;

internal static class MarketChangeJsonReader
{
    internal static void ReadMarketChange(this Market market, ref Utf8JsonReader reader, long publishTime)
    {
        if (!reader.PropertyName("mc"u8)) return;

        var objectCount = 0;
        while (reader.Read())
        {
            if (reader.EndOfObject(ref objectCount)) break;
            if (!market.IdMatches(ref reader)) return;

            market.SetTotalMatched(ref reader);
            market.ReadMarketDefinition(ref reader);
        }

        market.PublishTime = publishTime;
    }

    private static bool IdMatches(this Market market, ref Utf8JsonReader reader)
    {
        var marketIdMatched = true;
        if (!reader.PropertyName("id"u8))
            return marketIdMatched;

        reader.Read();
        if (!reader.ValueIs(market.Id))
            marketIdMatched = false;

        return marketIdMatched;
    }

    private static void SetTotalMatched(this Market market, ref Utf8JsonReader reader)
    {
        if (!reader.PropertyName("tv"u8)) return;

        reader.Read();
        market.TradedVolume = reader.GetDouble();
        reader.Read();
    }
}
