using Betfair.Extensions.Markets;

namespace Betfair.Extensions.JsonReaders;

internal static class ChangeMessageJsonReader
{
    internal static void ReadChangeMessage(this Market market, ref Utf8JsonReader reader)
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
