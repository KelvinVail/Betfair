using Betfair.Extensions.Markets;

namespace Betfair.Extensions.ByteReaders;

internal static class RunnerDefinitionsReader
{
    internal static void ReadRunners(this MarketCache market, ref BetfairJsonReader reader)
    {
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray) break;

            market.ReadRunner(ref reader);
        }
    }
}
