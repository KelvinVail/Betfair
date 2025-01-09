using Betfair.Extensions.Markets;

namespace Betfair.Extensions.ByteReaders;

internal static class ChangeMessageJsonReader
{
    internal static void ReadChangeMessage(this MarketCache market, ref BetfairJsonReader reader)
    {
        while (reader.Read())
        {
            if (!reader.PropertyName("op"u8)) continue;
            
            reader.Read();
            if (reader.Value("mcm"u8))
                market.ReadMarketChangeMessage(ref reader);
            else
                return;
        }
    }
}
