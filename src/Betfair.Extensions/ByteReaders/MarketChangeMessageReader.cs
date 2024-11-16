using Betfair.Extensions.Markets;

namespace Betfair.Extensions.ByteReaders;

internal static class MarketChangeMessageReader
{
    private static readonly byte[] _publishTime = "pt"u8.ToArray();
    private static readonly byte[] _marketChange = "mc"u8.ToArray();

    internal static void ReadMarketChangeMessage(
        this Market market,
        ref BetfairJsonReader reader)
    {
        var objectCount = 0;
        long pt = 0;
        while (reader.Read())
        {
            if (reader.EndOfObject(ref objectCount)) break;

            var propertyName = reader.NextProperty();

            switch (propertyName)
            {
                case var _ when propertyName.SequenceEqual(_publishTime):
                    reader.Read();
                    pt = reader.GetInt64();
                    break;
                case var _ when propertyName.SequenceEqual(_marketChange):
                    market.ReadMarketChange(ref reader, pt);
                    break;
            }
        }
    }
}
