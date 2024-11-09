using Betfair.Extensions.Markets;
using Betfair.Extensions.Markets.Enums;

namespace Betfair.Extensions.JsonReaders;

internal static class MarketDefinitionReader
{
    private static readonly byte[] _marketStartTime = "marketTime"u8.ToArray();
    private static readonly byte[] _inPlay = "inPlay"u8.ToArray();
    private static readonly byte[] _marketStatus = "status"u8.ToArray();
    private static readonly byte[] _marketVersion = "version"u8.ToArray();
    private static readonly byte[] _runners = "runners"u8.ToArray();
    private static readonly Dictionary<byte, MarketStatus> _marketStatusMap = new()
    {
        [(byte)'O'] = MarketStatus.Open,
        [(byte)'S'] = MarketStatus.Suspended,
        [(byte)'C'] = MarketStatus.Closed,
        [(byte)'I'] = MarketStatus.Inactive,
    };

    internal static void ReadMarketDefinition(this Market market, ref Utf8JsonReader reader)
    {
        var objectCount = 0;
        while (reader.Read())
        {
            if (reader.EndOfObject(ref objectCount)) break;

            var propertyName = reader.NextProperty();
            switch (propertyName)
            {
                case var span when span.SequenceEqual(_marketStartTime):
                    reader.Read();
                    market.StartTime = reader.GetDateTimeOffset();
                    break;
                case var span when span.SequenceEqual(_inPlay):
                    reader.Read();
                    market.IsInPlay = reader.GetBoolean();
                    break;
                case var span when span.SequenceEqual(_marketStatus):
                    reader.Read();
                    market.Status = _marketStatusMap[reader.ValueSpan[0]];
                    break;
                case var span when span.SequenceEqual(_marketVersion):
                    reader.Read();
                    market.Version = reader.GetInt64();
                    break;
                case var span when span.SequenceEqual(_runners):
                    market.ReadRunners(ref reader);
                    break;
                default:
                    reader.Skip();
                    break;
            }
        }
    }
}
