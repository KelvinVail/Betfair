using Betfair.Extensions.Markets;

namespace Betfair.Extensions.JsonReaders;

internal static class MarketChangeReader
{
    private static readonly byte[] _marketId = "id"u8.ToArray();
    private static readonly byte[] _tradedVolume = "tv"u8.ToArray();
    private static readonly byte[] _marketDefinition = "marketDefinition"u8.ToArray();
    private static readonly byte[] _runnerChanges = "rc"u8.ToArray();

    internal static void ReadMarketChange(this Market market, ref Utf8JsonReader reader, long publishTime)
    {
        var objectCount = 0;
        while (reader.Read())
        {
            if (reader.EndOfObject(ref objectCount)) break;

            var propertyName = reader.NextProperty();
            switch (propertyName)
            {
                case var _ when propertyName.SequenceEqual(_marketId):
                    reader.Read();
                    if (!reader.ValueSpan.SequenceEqual(market.MarketIdUtf8))
                        return;
                    break;
                case var _ when propertyName.SequenceEqual(_tradedVolume):
                    reader.Read();
                    market.TradedVolume = reader.GetDouble();
                    break;
                case var _ when propertyName.SequenceEqual(_marketDefinition):
                    market.ReadMarketDefinition(ref reader);
                    break;
                case var _ when propertyName.SequenceEqual(_runnerChanges):
                    market.ReadRunnerChanges(ref reader);
                    break;
                default:
                    reader.Skip();
                    break;
            }
        }

        market.PublishTime = publishTime;
    }
}
