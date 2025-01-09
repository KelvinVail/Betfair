using Betfair.Extensions.Markets;
using Betfair.Extensions.Markets.Enums;

namespace Betfair.Extensions.ByteReaders;

internal static class MarketDefinitionReader
{
    private static readonly byte[] _marketStartTime = "marketTime"u8.ToArray();
    private static readonly byte[] _inPlay = "inPlay"u8.ToArray();
    private static readonly byte[] _marketStatus = "status"u8.ToArray();
    private static readonly byte[] _marketVersion = "version"u8.ToArray();
    private static readonly byte[] _runners = "runners"u8.ToArray();
    private static readonly Dictionary<byte, MarketStatus> _marketStatusMap = new ()
    {
        [(byte)'O'] = MarketStatus.Open,
        [(byte)'S'] = MarketStatus.Suspended,
        [(byte)'C'] = MarketStatus.Closed,
        [(byte)'I'] = MarketStatus.Inactive,
    };

    internal static void ReadMarketDefinition(this MarketCache market, ref BetfairJsonReader reader)
    {
        var objectCount = 0;
        while (reader.Read())
        {
            if (reader.EndOfObject(ref objectCount)) break;
            if (reader.TokenType != JsonTokenType.PropertyName) continue;

            var propertyName = reader.ValueSpan;
            switch (propertyName)
            {
                case var _ when propertyName.SequenceEqual(_marketStartTime):
                    reader.Read();
                    market.StartTime = reader.GetDateTimeOffset();
                    break;
                case var _ when propertyName.SequenceEqual(_inPlay):
                    reader.Read();
                    market.IsInPlay = reader.GetBoolean();
                    break;
                case var _ when propertyName.SequenceEqual(_marketStatus):
                    reader.Read();
                    market.Status = _marketStatusMap[reader.ValueSpan[0]];
                    break;
                case var _ when propertyName.SequenceEqual(_marketVersion):
                    reader.Read();
                    market.Version = reader.GetInt64();
                    break;
                case var _ when propertyName.SequenceEqual(_runners):
                    market.ReadRunners(ref reader);
                    break;
                default:
                    // reader.Skip();
                    break;
            }
        }
    }
}
