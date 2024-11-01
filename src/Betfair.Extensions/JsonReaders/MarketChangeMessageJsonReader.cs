using Betfair.Extensions.Markets;

namespace Betfair.Extensions.JsonReaders;

internal static class MarketChangeMessageJsonReader
{
    internal static void ReadMarketChangeMessage(this Market market, ref Utf8JsonReader reader)
    {
        if (!reader.PropertyValue("op"u8, "mcm"u8)) return;

        var objectCount = 0;
        long pt = 0;
        while (reader.Read())
        {
            if (reader.EndOfObject(ref objectCount)) break;
            if (reader.PropertyName("pt"u8))
            {
                reader.Read();
                pt = reader.GetInt64();
            }

            market.ReadMarketChange(ref reader, pt);
        }
    }
}
